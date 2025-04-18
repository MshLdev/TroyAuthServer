using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;



namespace TroyAuthServer
{
    class Packet
    {
        //Whenever a new Packet has been recieved to the client
        public static Auth.REQUEST clientRecived(ref Client current,ref dbController db)
        {

            //Packet was empty somehow
            //nothing left to do....
            if(current.buffSize < 10)   
            {
                Logger.numRequestEmpty ++;
                current.Status = Auth.STATUS.STATUS_SERVED;
                return Auth.REQUEST.PACKET_EMPTY_REQUEST;
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
                Logger.numRequestWrong ++;
                current.Status = Auth.STATUS.STATUS_SERVED;
                return Auth.REQUEST.PACKET_DAMAGED_REQUEST;
            }
                


            //Second uint32 in header is always Type
            Auth.REQUEST request = (Auth.REQUEST)getUint32AtIndex(ref recBuf, 4);   
            current.lastRequest = request;
            //Printer.Write("::Packet Debug::\n[" + current.buffSize + "/" + sizeDeclared + " = size]\n[" + request + " = " + (Auth.REQUEST)request + "]", ConsoleColor.DarkMagenta);



            //Serve the request if valid        
            switch(request) 
            {
                //Client requested SessionID
                //we need to 1)recive login cridencials, 2)Ask Database if they are correct, and 3)send back current SessionID char(50)
                case Auth.REQUEST.PACKET_SESSION_REQUEST:
                    //Printer.Write(BitConverter.ToString(recBuf).Replace("-"," "), ConsoleColor.DarkMagenta);

                    current.isDbServerd = false; //Test mutex variable to make sure the client doesnt disapear in a meantine
                    current.Status = Auth.STATUS.STATUS_REQUESTED;
                    //1)
                    getCredentials(ref recBuf, ref current);
                    //2)
                    
                    lock (db)
                        current.session = db.getUserSession(current.login, current.password);
                    //If session shorter than 50 the login was failed and this is not secure!!
                    if(current.session.Length < 50 && current.socket.RemoteEndPoint != null)
                    {
                        string addr = ((IPEndPoint)current.socket.RemoteEndPoint).Address.ToString();
                        Security.wrongLogin(addr);
                    }
                    //3)
                    if(!current.trySend(Encoding.ASCII.GetBytes(current.session)))
                    {
                        //We must be disposed at this point, so no point accessing anything here
                        return request;
                    }
                        
                    //Correct session has 50 bytes
                    //if (current.session.Length == 50)
                        //Printer.Write("\nsessionID -> " + current.session, ConsoleColor.Green);
                    //else
                        //Printer.Write("\nsessionID -> " + current.session, ConsoleColor.Red);
                    current.Status = Auth.STATUS.STATUS_SERVED;
                    current.isDbServerd = true;
                    return request;
                    
                //Request didnt match Any Enum provided
                //in Auth Class...
                default:
                    Logger.numRequestWrong ++;
                    current.Status = Auth.STATUS.STATUS_SERVED;
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