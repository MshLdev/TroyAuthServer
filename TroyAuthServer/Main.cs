using System.Net;
using System.Net.Sockets;
using Microsoft.VisualBasic;

namespace TroyAuthServer
{
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Client> clients = new List<Client>();
        private static uint connectionCount = 0;
        private static dbController db = new dbController();



        static void Main()
        {   
            Console.Clear();
            Auth.loadCfg();
            Console.Title = "TroyAuthServer v"+ Auth.SERVER_VERSION + " --- (" + Auth.serverName + ")";
            
            //Setup Socket infrastructure
            if (SetupServer())
            //Block the server
                Console.ReadLine(); 
                //Parent.connect(); for now connection with Parent might not be needed, this doesnt really help by any means tbh. hmmm 
            //CleanUp
            Console.WriteLine("Closing, Cleaning and quiting...");
            CloseAllSockets();
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
            //Console.WriteLine("Setting up server Socket...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Auth.PORT));
            //Console.WriteLine("Listening...");
            serverSocket.Listen(0);
            //Console.WriteLine("Callback setup...");
            serverSocket.BeginAccept(AcceptCallback, null);
            //Connect to db
            if(!SetupDB())
                return false;

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
            {
                client.socket.Shutdown(SocketShutdown.Both);
                client.socket.Close();
            }
            //ERROR - Closing this casuing Exeption, since at this point all the resources are already released!!!!!
            //serverSocket.Close();
            db.close();
        }


        private static void AcceptCallback(IAsyncResult AR)
        {
            Client nClient = new Client(connectionCount);
            connectionCount ++;
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

           clients.Add(nClient);
           nClient.socket.BeginReceive(nClient.buffer, 0, Auth.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, nClient);
           serverSocket.BeginAccept(AcceptCallback, null);
        }


        private static void ReceiveCallback(IAsyncResult AR)
        {
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
                Printer.Write("Client forcefully disconnected", ConsoleColor.Yellow);
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                current.socket.Close(); 
                clients.Remove(current);
                return;
            }


            ///Here we have to manage the connections, packets will be handled
            ///By Packet Object, Here we only manage Key aspects
            switch(Packet.clientRecived(ref current, ref db)) 
            {

                // Client requested something
                //Packet class aready responded
                //nothing else needs to be done at this time
                case (uint)Auth.REQUEST.PACKET_SESSION_REQUEST:
                    Logger.logServedEvent(current.session);
                    closeConn(current);
                    break;


                //Propably the Client closed the socket
                //on his site, we will confirm this in the next callBack to make sure
                case (uint)Auth.REQUEST.PACKET_EMPTY_REQUEST:
                    Logger.logEmptyEvent();
                    closeConn(current);
                    break;


                //Packet size doesnt match the declared value
                case (uint)Auth.REQUEST.PACKET_DAMAGED_REQUEST:
                    Logger.logDamagedEvent("");
                    closeConn(current);
                    break;


                //Client sent invalid packet
                //or packet was damaged 
                //thats sus... -.-
                case (uint)Auth.REQUEST.PACKET_WRONG_REQUEST:
                    Logger.logWrongEvent(" |" + current.lastRequest);
                    closeConn(current);
                    break;
            }
        }

        private static void closeConn(Client cnn)
        {
            // Always Shutdown before closing
            cnn.socket.Shutdown(SocketShutdown.Both);
            cnn.socket.Close();
            clients.Remove(cnn);
            //Printer.Write("\nClient[" + cnn.clientID + "] closed connection", ConsoleColor.DarkGreen);
        }
    }
}
