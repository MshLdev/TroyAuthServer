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
    }
}