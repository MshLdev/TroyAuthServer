#nullable disable
using System.Net.Sockets;


namespace TroyAuthServer
{
    class Client
    {
        public Socket socket;                                        //Client Socket we will use to comunicate(CASUES NULLABLE WARNING)
        public byte[] buffer = new byte[Auth.BUFFER_SIZE];           //Client message buffer
        public int buffSize = 0;                                     //last packet Size


        public uint         ID;                                      //ID generated for the time of connection
        public float         NullBuffTime = 0;                      //Time in seconds the client's buffer was 0, if client
                                                                    //will not send us any bytes in 'NULL_TIME'miliseconds we need to dump it!
        public Auth.STATUS  Status;                                 //To determine if any actions needed                    
        public Auth.REQUEST lastRequest;                            //Just for logging


        public string login;                                        //Login to access the Database for Authorization
        public string password;                                     //MD5 hash of a passwprd to access the Database for Authorization
        public string session;                                      //SessionID to send back to the User
        


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
                Printer.Write("\nClient[" + ID + "] Timedout", ConsoleColor.DarkGreen);
            }
        }


        public void request(Auth.REQUEST request)
        {
             switch(request) 
            {
                case Auth.REQUEST.PACKET_SESSION_REQUEST:
                    Status = Auth.STATUS.STATUS_REQUESTED;
                    break;
                //Packet = NULL, dump the fucker
                case (uint)Auth.REQUEST.PACKET_EMPTY_REQUEST:
                    Logger.logEmptyEvent();
                    Status = Auth.STATUS.STATUS_SERVED;
                    break;
                //Packet size doesnt match the declared value, dump the fucker
                case Auth.REQUEST.PACKET_DAMAGED_REQUEST:
                    Logger.logDamagedEvent(" from client.cs/Req");
                    Status = Auth.STATUS.STATUS_SERVED;
                    break;
                //Invalid packet, dump the fucker
                case Auth.REQUEST.PACKET_WRONG_REQUEST:
                    Logger.logWrongEvent(" |" + lastRequest);
                    Status = Auth.STATUS.STATUS_SERVED;
                    break;
            }
            //Printer.Write("Exited request with stauts -> " + Status, ConsoleColor.Yellow);
        }
    }
}