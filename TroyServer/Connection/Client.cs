#nullable disable
using System.Net.Sockets;


namespace TroyServer
{
    class Client
    {
        public Socket socket;                                        //Client Socket we will use to comunicate(CASUES NULLABLE WARNING)
        public byte[] buffer = new byte[Server.BUFFER_SIZE];           //Client message buffer
        public int buffSize = 0;                                     //last packet Size


        public uint         ID;                                      //ID generated for the time of connection
        public float         NullBuffTime = 0;                      //Time in seconds the client's buffer was 0, if client
                                                                    //will not send us any bytes in 'NULL_TIME'miliseconds we need to dump it!
        public Server.STATUS  Status;                                 //To determine if any actions needed                    
        public Server.REQUEST lastRequest;                            //Just for logging


        public string login;                                        //Login to access the Database for Authorization
        public string password;                                     //MD5 hash of a passwprd to access the Database for Authorization
        public string session;                                      //SessionID to send back to the User
        


        public Client(uint id)
        { 
            Status = Server.STATUS.STATUS_NONE;
            ID = id;
        }


        public void timeOut(float time)
        {   
            if(!isPending())
                //Client Already Authorized
                return;

            NullBuffTime += time;
            if (NullBuffTime >= Server.TIME_FOR_REQUEST)
            {
                Status = Server.STATUS.STATUS_TIMED;       //Client Timed out, so there is nothind for them here...
                Printer.Write("\nClient[" + ID + "] Timedout", ConsoleColor.DarkGreen);
            }
        }


        public void request(Server.REQUEST request)
        {
             switch(request) 
            {
                case Server.REQUEST.PACKET_LOGIN_REQUEST:
                    Status = Server.STATUS.STATUS_REQUESTED;
                    break;
                //Packet = NULL, dump the fucker
                case (uint)Server.REQUEST.PACKET_EMPTY_REQUEST:
                    Logger.logEmptyEvent();
                    Status = Server.STATUS.STATUS_ERROR;
                    break;
                //Packet size doesnt match the declared value, dump the fucker
                case Server.REQUEST.PACKET_DAMAGED_REQUEST:
                    Logger.logDamagedEvent(" from client.cs/Req");
                    Status = Server.STATUS.STATUS_ERROR;
                    break;
                //Invalid packet, dump the fucker
                case Server.REQUEST.PACKET_WRONG_REQUEST:
                    Logger.logWrongEvent(" |" + lastRequest);
                    Status = Server.STATUS.STATUS_ERROR;
                    break;
            }
            //Printer.Write("Exited request with stauts -> " + Status, ConsoleColor.Yellow);
        }


        //Check if Connection is still allowed to be
        public bool isAuthorized() 
        {
            switch (Status)
            {
                case Server.STATUS.STATUS_EXIT:
                case Server.STATUS.STATUS_TIMED:
                case Server.STATUS.STATUS_ERROR:
                    return false;
                default:
                    return true;    
            }
        }

        //Check if Client is passed Authorization or not
        public bool isPending()
        {
            switch (Status)
            {
                case Server.STATUS.STATUS_NONE:
                case Server.STATUS.STATUS_REQUESTED:
                    return true;
                default:
                    return false;    
            }
        }
    }
}