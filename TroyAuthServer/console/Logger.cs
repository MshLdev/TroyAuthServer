using Microsoft.VisualBasic;

namespace TroyAuthServer
{
    class Logger
    {

        //static string path = "LOG/log.txt";

        
        public static void genericfileLog(string logmsg)
        {
            //System.IO.File.AppendAllLines(path, new string[]{logmsg});
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
                Printer.TypeLine($"[{DateAndTime.Now.ToLongTimeString()}]  --", ConsoleColor.DarkBlue);
                Printer.TypeLine($" num Requests = {Program.numRequest}({Program.numRequest/5}/s)", ConsoleColor.DarkGreen);
                if (Program.numRequestEmpty > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Empty = {Program.numRequestEmpty}", ConsoleColor.DarkRed);
                }

                if (Program.numRequestWrong > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Damaged = {Program.numRequestWrong}", ConsoleColor.DarkRed);
                }

                if (Program.numTerminatedConnection > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Termination = {Program.numTerminatedConnection}", ConsoleColor.Yellow);
                }

                if (Program.numTimeouts > 0)
                {
                    Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                    Printer.TypeLine($" num Timeouts = {Program.numTimeouts}", ConsoleColor.Yellow);
                }

                if (Program.numOverload > 0)
                {
                     Printer.TypeLine($", ", ConsoleColor.DarkBlue);
                     Printer.TypeLine($" POTENTIAL ATTACKS = {Program.numOverload}", ConsoleColor.DarkRed);
                }
                 
                     Printer.TypeLine("\n", ConsoleColor.DarkRed);
                        
                  //Logger.genericfileLog(DateAndTime.Now.ToLongTimeString() + " total requests recived last second = " + numRequest + "\n");
                  interval                        = 5f;
                  Program.numRequest              = 0;
                  Program.numRequestEmpty         = 0;
                  Program.numRequestWrong         = 0;
                  Program.numTerminatedConnection = 0;
                  Program.numTimeouts             = 0;
                  Program.numOverload             = 0;
            }
        }
    }
}