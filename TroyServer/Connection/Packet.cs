using System.Text;
using System.Security.Cryptography;



namespace TroyServer
{
    class Packet
    {
        //Whenever a new Packet has been recieved to the client
        public static Server.REQUEST clientRecived(ref Client current,ref dbController db)
        {
            //Packet was empty somehow
            //nothing left to do....
            if(current.buffSize < 10)   
            {
                current.request(Server.REQUEST.PACKET_EMPTY_REQUEST);
                return Server.REQUEST.PACKET_EMPTY_REQUEST;
            }
                
            //Copy just the lenght of the last packet to our Buffer
            //So we only work on the actual Packet
            byte[] recBuf = new byte[current.buffSize];
            Array.Copy(current.buffer, recBuf, current.buffSize);

            
            //First uint32 in header is always packetSize
            uint sizeDeclared = getUint32AtIndex(ref recBuf, 0);
            //Make sure packet size matches the Size Client
            //Intended!!
            if(current.buffSize != sizeDeclared)
            {
                current.request(Server.REQUEST.PACKET_DAMAGED_REQUEST);
                return Server.REQUEST.PACKET_DAMAGED_REQUEST;
            }
                


            //Second uint32 in header is always Type
            Server.REQUEST request = (Server.REQUEST)getUint32AtIndex(ref recBuf, 4);   
            current.lastRequest = request;
            //Printer.Write("::Packet Debug::\n[" + current.buffSize + "/" + sizeDeclared + " = size]\n[" + request + " = " + (Auth.REQUEST)request + "]", ConsoleColor.DarkMagenta);



            //Serve the request if valid        
            switch(request) 
            {
                //Client requested Log in
                //We need to: 1) check his session, 2)generate new one for db, 3)prepare Character Object for sending/Game Service
                case Server.REQUEST.PACKET_LOGIN_REQUEST:
                    current.request(request);
                    Printer.Write("Client wants to Login, there is no logic to handle this for now, se we just authorize them for now, lets debug connection first", ConsoleColor.DarkCyan);
                    current.Status = Server.STATUS.STATUS_LOGGED;
                    return request;
                    
                //Request didnt match Any Enum provided
                //in Auth Class...
                default:
                    current.request(Server.REQUEST.PACKET_WRONG_REQUEST);
                    return request;
            }
        }



        //Take out
        //Login and Password out of the Packet
        public static void getCredentials(ref byte[] buff, ref Client current)
        {
            //1 byte to int we can take directly from buffer
            int loginSize  = buff[10];
            int passSize   = buff[11];

            //Login starts at index 12 of Auth Packet
            current.login     = Encoding.UTF8.GetString(buff, 12, loginSize);
            //Passwords are Hashed on client site for saftey
            current.password  = Encoding.UTF8.GetString(buff, 12 + loginSize, passSize);
            //Printer.Write("[" + current.login   + " = Login]\n[" + current.password + " = Password]\n::Packet Debug::\n", ConsoleColor.DarkMagenta);
        }



        public static string MD5Hash(string message)
        {
            byte[] hash      = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(message));
            string strHash   = BitConverter.ToString(hash);
            // without dashes
            strHash = strHash.Replace("-", "");
            //lowercase
            strHash = strHash.ToLower();
            return strHash;
        }

        public static uint getUint32AtIndex(ref byte[] buff, int index)
        {
            try
            {
                return BitConverter.ToUInt32(buff, index);
            }
            catch(Exception e)
            {
                Logger.logDamagedEvent("Packet was unecpected ended, propably wrong request...\n [BitConverter::" + e.Message + "]");
                return 999999;
            }
        }
    }
}