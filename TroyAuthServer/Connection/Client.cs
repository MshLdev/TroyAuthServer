#nullable disable
using System.Net.Sockets;


namespace TroyAuthServer
{
    class Client
    {
        public Socket socket;                                        //Client Socket we will use to comunicate(CASUES NULLABLE WARNING)
        public byte[] buffer = new byte[Auth.BUFFER_SIZE];           //Client message buffer
        public int buffSize = 0;                                     //last packet Size


        public int      clientStatus;                                //Current Client status, will be needed for SSL in the future!!!
        public uint     clientID;                                    //ID generated for the time of connection



        public string login;                                        //Login to access the Database for Authorization
        public string password;                                     //MD5 hash of a passwprd to access the Database for Authorization
        public string session;                                      //SessionID to send back to the User
        public uint lastRequest;


        public Client(uint id)
        { 
            clientStatus = Auth.STATUS_UNVERIFIED; 
            clientID = id;
        }

    }
}