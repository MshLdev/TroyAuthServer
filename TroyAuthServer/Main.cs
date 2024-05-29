using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;


namespace TroyAuthServer
{
    class Program
    {
        
        private static readonly Socket serverSocket         = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Client> clients        = new List<Client>();
        private static dbController db                      = new dbController();    
        private static bool terminate                       = false;                //Bool for terminating the Server, tries to make it safe and clean
        private static bool cleaned                         = false;                //Final close signal
        private static bool isWorking                       = false;                //Blocker for time of main loop cleanups

        public static uint numConnections                  = 0;                    //ammount of connections recieved in the lifetime
        public static int numRequest                       = 0;                    //ammount of requests in the last second
        public static int numRequestEmpty                   = 0;                    //ammount of empty requests in the last second
        public static int numRequestWrong                   = 0;                    //ammount of wrong requests in the last second
        public static int numTerminatedConnection           = 0;                    //ammount of connections closed frocefuly
        public static int numTimeouts                       = 0;                    //ammount of connections closed by time out
        public static int numOverload                       = 0;                    //ammount of connections that might be considered attacks
        static void Main()
        {   
            Console.Clear();
            Auth.loadCfg();
            Console.Title = "TroyAuthServer v"+ Auth.SERVER_VERSION + " --- (" + Auth.serverName + ")";
            
            //Setup Socket infrastructure
            if (SetupServer())
                ClientLoop();
                
                
            //Block the main program Thread
            Console.ReadLine();
            terminate = true;
            Printer.Write("Input Detected, waiting for the Cleanup....",ConsoleColor.DarkRed);
            while(cleaned == false)
                continue;
            Printer.Write("Server Closed, good bye!\n",ConsoleColor.Green );
        }


        private static bool SetupDB()
        {
            //Try to establish connection with the db
            while(true)
            {
                if(db.connect())
                {
                    Printer.Write(Auth.serverDbVersion + " Online, Connection Opened",ConsoleColor.Green );
                    return true;
                }
                else
                {
                    Printer.Write("Retry?? (Y/N)",ConsoleColor.Yellow );
                    string? answer = Console.ReadLine();
                    if(answer != null && answer.ToLower() == "y")
                    {
                        Console.WriteLine("Retrying Database Setup...");
                        continue;
                    }
                        
                    else
                        return false;
                }
            }
        }


        private static bool SetupServer()
        {
            //Connect to db
            if(!SetupDB())
                return false;
          
            //Console.WriteLine("Setting up server Socket...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Auth.PORT));
            //Console.WriteLine("Listening...");
            serverSocket.Listen(0);
            //Console.WriteLine("Callback setup...");
            serverSocket.BeginAccept(AcceptCallback, null);

            string info = "[System ::" + DateAndTime.Now.ToLongDateString() + " :: " +DateAndTime.Now.ToShortTimeString()+  "] -> ***  AuthService Server Online(PORT:"+ Auth.PORT +")  ***";
            Printer.Write(info + "\n", ConsoleColor.Green );
            Logger.genericfileLog(info);
            return true;
        }


        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        private static void CloseAllSockets()
        {
            //Console.WriteLine("Closing all sockets(" + clients.Count() +")...");
            foreach (Client client in clients)
                closeConn(client);
            //ERROR - Closing this casuing Exeption, since at this point all the resources are already released!!!!!
            //serverSocket.Close();
            db.close();
        }


        private static void AcceptCallback(IAsyncResult AR)
        {
            Client nClient = new Client(numConnections);
            numConnections ++;
            //Socket socket;

            try
            {
                //Console.WriteLine("Connection");
                nClient.socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            if(nClient.socket.RemoteEndPoint == null)
                return;
                
            //Get the remote adress, and check if there is any 
            //suss behaviour over there..
            string addr = ((IPEndPoint)nClient.socket.RemoteEndPoint).Address.ToString();
            if(!Security.verifyAdress(addr))
            {
                //Too many requests!!!
                numOverload ++;
                nClient.tryClose();
                serverSocket.BeginAccept(AcceptCallback, null);
                return;
            }
                

            while (isWorking)
                continue;
                //Console.Write("Thread busy, waiting for oportunity to get into the iterator!!\n");
           clients.Add(nClient);
           nClient.socket.BeginReceive(nClient.buffer, 0, Auth.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, nClient);
           serverSocket.BeginAccept(AcceptCallback, null);
        }


        private static void ReceiveCallback(IAsyncResult AR)
        {
            numRequest ++;
            Client? current = (Client?)AR.AsyncState;
            if(current == null)
            {
                Console.WriteLine("Fatal Error, the Socket was returned null at - Main.cs :: line 72 -- !!!!");
                return;
            }


            try
            {
                //Current packet size 
                current.buffSize = current.socket.EndReceive(AR);
            }
            catch (SocketException)
            {
                numTerminatedConnection ++;
                //Printer.Write("Client forcefully disconnected", ConsoleColor.Yellow);
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.Status = Auth.STATUS.STATUS_SERVED;
                return;
            }

            //Analize Packet
            Packet.clientRecived(ref current, ref db); 
        }


        //WARNING:::
        //Under high load the Close conn tries to close sockets
        //that have already been disposed...
        private static void closeConn(Client cnn)
        {
            cnn.tryClose();         //So my guess is that, when the client dc's forcefuly the socked is being closed by the Class itself, and thats why we get that error...
            clients.Remove(cnn);
            //Printer.Write("\nClient[" + cnn.ID + "] closed connection", ConsoleColor.DarkGreen);
            //Printer.Write("\nClient[" + cnn.ID + "] closed connection [main.closeconn()]", ConsoleColor.DarkGreen);
        }



          private static async void ClientLoop()
            {
                Timer clientloopTimer = new Timer();
                float requestInterval = 0.5f;

                while (true)
                {
                    //Do Timings and Logs
                    clientloopTimer.calculate();
                    requestInterval -= clientloopTimer.deltaTime;
                    Security.SecurityTick(clientloopTimer.deltaTime);
                    Logger.LogStatus(ref requestInterval);
                    //Check for termination
                    if(terminate)
                    {
                        Printer.Write("Stopping Main Loop...", ConsoleColor.DarkGreen);
                        break;
                    }
                    
                    
                    //Check every connection for Action needed + cleanup
                    isWorking = true;
                    await Task.Yield();
                    foreach (Client client in clients.ToList())
                    {
                        //Client can be disconnected now   
                        if(client.Status == Auth.STATUS.STATUS_SERVED && client.isDbServerd)  
                            closeConn(client);
                        client.timeOut(clientloopTimer.deltaTime);
                    }
                    isWorking = false;
                    await Task.Delay(50);
                }

                Printer.Write("Exiting LoopFunction...", ConsoleColor.DarkGreen);
                CloseAllSockets();
                cleaned = true;
            }
    }
}
