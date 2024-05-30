using Microsoft.VisualBasic;

namespace TroyAuthServer
{
    class Logger
    {
        public static uint numConnections                   = 0;                    //ammount of connections recieved in the lifetime
        public static int numRequest                        = 0;                    //ammount of requests in the last second
        public static int numRequestEmpty                   = 0;                    //ammount of empty requests in the last second
        public static int numRequestWrong                   = 0;                    //ammount of wrong requests in the last second
        public static int numTerminatedConnection           = 0;                    //ammount of connections closed frocefuly
        public static int numTimeouts                       = 0;                    //ammount of connections closed by time out
        public static int numOverload                       = 0;                    //ammount of connections that might be considered attacks
        static string path = "LOG/log.txt";

        
        public static void genericfileLog(string logmsg)
        {
            File.AppendAllLines(path, new string[]{logmsg});
        }

        public static void systemfileLog(string logmsg)
        {
            //string sysLog = "[System ::" + DateAndTime.Now.ToLongDateString() + " :: " +DateAndTime.Now.ToShortTimeString()+  "] ->" + logmsg;
            //System.IO.File.AppendAllLines(path, new string[]{logmsg});
        }


        public static void logServedEvent(string info)
        {
            //string logmsg = "[System ::" + DateAndTime.Now.ToLongDateString() + " :: " +DateAndTime.Now.ToShortTimeString()+  "] -> User Served Event. " + info;
            //genericfileLog(logmsg);

            if(Auth.isLogger == "false")
                return;

            Printer.TypeLine("[System::", ConsoleColor.DarkYellow);
            Printer.TypeLine(DateTime.Now.ToLongTimeString(), ConsoleColor.DarkBlue);
            Printer.TypeLine("]", ConsoleColor.DarkYellow);
            Printer.TypeLine("->User Served Event.\n", ConsoleColor.DarkGreen);
        }


        public static void logEmptyEvent()
        {
            //string logmsg = "[System ::" + DateAndTime.Now.ToLongDateString() + " :: " +DateAndTime.Now.ToShortTimeString()+  "] -> User Asked Empty Request.";
            //genericfileLog(logmsg);
            if(Auth.isLogger == "false")
                return;

            Printer.TypeLine("[System::", ConsoleColor.DarkYellow);
            Printer.TypeLine(DateAndTime.Now.ToLongTimeString(), ConsoleColor.DarkBlue);
            Printer.TypeLine("]", ConsoleColor.DarkYellow);
            Printer.TypeLine("->User Asked Empty Request.\n", ConsoleColor.Red);
        }


        public static void logWrongEvent(string info)
        {
            //string logmsg = "[[System ::" + DateAndTime.Now.ToLongDateString() + " :: " +DateAndTime.Now.ToShortTimeString()+  "] -> Request not listed on Auth Enum! REQ.ID == ." + info;
            //genericfileLog(logmsg);
            if(Auth.isLogger == "false")
                return;

            Printer.TypeLine("[System::", ConsoleColor.DarkYellow);
            Printer.TypeLine(DateAndTime.Now.ToLongTimeString(), ConsoleColor.DarkBlue);
            Printer.TypeLine("]", ConsoleColor.DarkYellow);
            Printer.TypeLine("->Request not listed on Auth Enum!.\n", ConsoleColor.Yellow);
        }


        public static void logDamagedEvent(string info)
        {
            //string logmsg = "[System ::" + DateAndTime.Now.ToLongDateString() + " :: " +DateAndTime.Now.ToShortTimeString()+  "] -> Packet was Damaged " + info;
            //genericfileLog(logmsg);
            if(Auth.isLogger == "false")
                return;

            Printer.TypeLine("[System::", ConsoleColor.DarkYellow);
            Printer.TypeLine(DateAndTime.Now.ToLongTimeString(), ConsoleColor.DarkBlue);
            Printer.TypeLine("]", ConsoleColor.DarkYellow);
            Printer.TypeLine("->Request was Damaged!!.\n" + info, ConsoleColor.Yellow);
        }

        public static void LogStatus(ref float interval)
        {
            if(interval < 0f)
             {
                if (numRequest > 0 ||  numOverload > 0)
                    genericfileLog($"[{DateAndTime.Now.ToLongTimeString()}]  -- num Requests = { numRequest}({ numRequest/5}/s),  POTENTIAL ATTACKS = { numOverload},  num Empty = { numRequestEmpty},  num Damaged = { numRequestWrong}");
                Printer.TypeLine($"[{DateAndTime.Now.ToLongTimeString()}]  --", ConsoleColor.DarkBlue);
                Printer.TypeLine($" num Requests = { numRequest}({ numRequest/5}/s)", ConsoleColor.DarkGreen);
                if ( numRequestEmpty > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Empty = { numRequestEmpty}", ConsoleColor.DarkRed);
                }

                if ( numRequestWrong > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Damaged = { numRequestWrong}", ConsoleColor.DarkRed);
                }

                if ( numTerminatedConnection > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Termination = { numTerminatedConnection}", ConsoleColor.Yellow);
                }

                if ( numTimeouts > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Timeouts = { numTimeouts}", ConsoleColor.Yellow);
                }

                if ( numOverload > 0)
                {
                     Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                     Printer.TypeLine($" POTENCIAL ATTACKS = { numOverload}", ConsoleColor.DarkRed);
                }
                 
                     Printer.TypeLine("\n", ConsoleColor.DarkRed);
                        
                  //Logger.genericfileLog(DateAndTime.Now.ToLongTimeString() + " total requests recived last second = " + numRequest + "\n");
                  interval                        = 5f;
                   numRequest              = 0;
                   numRequestEmpty         = 0;
                   numRequestWrong         = 0;
                   numTerminatedConnection = 0;
                   numTimeouts             = 0;
                   numOverload             = 0;
            }
        }
    }
}