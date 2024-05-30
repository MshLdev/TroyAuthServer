#nullable disable
using System.Net.Sockets;


namespace TroyAuthServer
{
    class Client
    {
        public Socket socket;                                        //Client Socket we will use to comunicate(CASUES NULLABLE WARNING)
        public byte[] buffer = new byte[Auth.BUFFER_SIZE];           //Client message buffer
        public int buffSize = 0;                                     //last packet Size


        public uint         ID;                                     //ID generated for the time of connection
        public float        NullBuffTime = 0;                       //Time in seconds the client's buffer was 0, if client
                                                                    //will not send us any bytes in 'NULL_TIME'miliseconds we need to dump it!
        public Auth.STATUS  Status;                                 //To determine if any actions needed                    
        public Auth.REQUEST lastRequest;                            //Just for logging


        public string login;                                        //Login to access the Database for Authorization
        public string password;                                     //MD5 hash of a passwprd to access the Database for Authorization
        public string session;                                      //SessionID to send back to the User
        public bool isDbServerd = true;                            //The db was throwing (very rarley) exeptions on the very huge timeouts(it was a bug), so this is the kind of a mutex
                                                                    // kinda shit to make sure the db already served this client and doesnt try to work on deleted user!


        public Client(uint id)
        { 
            Status = Auth.STATUS.STATUS_NONE;
            ID = id;
        }


        public void timeOut(float time)
        {
            NullBuffTime += time;
            if (NullBuffTime >= Auth.TIME_FOR_REQUEST && Status != Auth.STATUS.STATUS_SERVED)
            {
                Status = Auth.STATUS.STATUS_SERVED;       //Client Timed out, so there is nothind for them here...
                Logger.numTimeouts ++;
                //Printer.Write("\nClient[" + ID + "] Timedout", ConsoleColor.DarkGreen);
            }
        }



        public bool trySend(byte[] data)
        {
            try
            {
                socket.Send(data);
                return true;
            }
            catch
            {
                Printer.Write("ABORT:: TRIED TO SEND ON DISPOSED SOCKET(we are going fast it seems....)", ConsoleColor.DarkYellow);
                return false;
            }
        }
                
                
        
        public void tryClose()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                //Printer.Write("\nClient[" + ID + "] closed connection [main.closeconn()]", ConsoleColor.DarkGreen);
            }
            catch
            {
                Printer.Write("Socket already've been closed...", ConsoleColor.DarkYellow);
            }
        }
    }
}