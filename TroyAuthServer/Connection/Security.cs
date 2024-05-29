using System.Net;
using System.Net.Sockets;

namespace TroyAuthServer
{
    class Adresses
    {
        public string       AdressIp;
        public float        firstRequest;
        public float        lastRequest;
        public int          numRequests;
        public float        reqpersec;
        public int          redemptionTime;
        public int          numWrongLogin;
        public float        shadowTime;

        public Adresses(string ip)
        {
            AdressIp        = ip;
            firstRequest    = 1;
            numRequests     = 1;
            reqpersec       = 0;
            lastRequest     = 0;
            redemptionTime  = 5;
            numWrongLogin   = 0;
            shadowTime      = 0;
        }
    }



    class Security
    {
        static public List<Adresses>    recentConnections       = new List<Adresses>();
        //static public List<string>      blackListAdresses       = new List<string>();
        static public float             securityTick            = 1f;



        public static bool verifyAdress(string user)
        {   
            //if (blackListAdresses.Contains(user))     // Instead of black list we can just set the recent adres to super low last req value
            //This user was susspended for a while!
                //return false;

            foreach (Adresses addr in recentConnections.ToList())
            {
                //Check if user recentley tried to connect
                if (addr.AdressIp == user)
                {
                    //Console.Write("resent user" + user + "\n");
                    addr.lastRequest = 0;
                    addr.numRequests ++;
                    //Console.Write($"{addr.firstRequest}\n\n");
                    addr.reqpersec = addr.numRequests/addr.firstRequest;

                    if (addr.shadowTime > 0)
                    {
                        return false;
                    }
                    //Console.Write($"{addr.reqpersec} = {addr.numRequests}/{addr.firstRequest} \n");
                    //WARNING
                    //If the attack was started very slow, the avrage packec will allow for short very hard attack
                    //so addicionally check if there was over 40 request in last 5 seconds!
                    if(addr.reqpersec > 8 || addr.numRequests > 40) // This is suss behavour, blacklist the guy!
                        {
                            addr.shadowTime = 10;
                            return false;
                        }
                    //User didnt overstayed yet...
                    return true;
                }
            }
            //This is a fresh user!!!!
            //Console.Write("Adding new User - " + user + "\n");
            Adresses freshAdress = new Adresses(user);
            recentConnections.Add(freshAdress);
            //Console.Write("num conn - " + recentConnections.Count + "\n");
            return true;
        }


        public static void SecurityTick(float delta)
        {
            securityTick -= delta;
            if (securityTick > 0)
                return;
            
            lock(recentConnections)
            {
                foreach (Adresses addr in recentConnections.ToList())
                {
                    addr.firstRequest += 1;
                    ///Console.Write("Incremented into -> " + addr.firstRequest + "\n");
                    addr.lastRequest  += 1;
                    addr.shadowTime   -= 1;
                    //Console.Write("Num recent = " + recentConnections.Count + ", redemption sec = "+ addr.redemptionTime +" -- rps ="+ addr.reqpersec +" \n");
                    if (addr.lastRequest > addr.redemptionTime)
                        recentConnections.Remove(addr);
                }
            }
            //Console.Write("Num recent = " + recentConnections.Count + "\n");
            securityTick = 1;
        }


        public static void wrongLogin(string user)
        {
            foreach (Adresses addr in recentConnections.ToList())
                if (addr.AdressIp == user)
                {
                    addr.numWrongLogin++;
                    if(addr.numWrongLogin > 8)
                    {
                        addr.shadowTime = 60;
                        addr.redemptionTime = 60;   //60 seconds timedout if there was too many wrong login attempts
                    }
                }
                    
        }
    }
    
}