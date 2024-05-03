using System.Net.Sockets;
using System.Reflection.Metadata;

namespace TroyAuthServer
{
    static class Auth
    {
        public const float      SERVER_VERSION       = 0.1f;     //Server Version
        public const float      PACKET_VERSION       = 0.1f;     //Packet version the server is using
        public const float      TIME_FOR_REQUEST     = 0.15f;     //The maximum time the client has for sending request before dump
        public const int        CONFIG_SIZE            = 6;      //Size of configuration file        


        public const int PORT                   = 1100;     //Server Port to listen to for the clients
        public const int PORT_PARENT            = 2100;     //Server Port to connect to the Parent Service
        public const int BUFFER_SIZE            = 2048;     //Client Buffer Size 

        

        //Client Status for the Client Iterator
        public enum STATUS : uint
        {
            STATUS_NONE,                  //Client didnt sent anything yet
            STATUS_REQUESTED,             //Client requested, but was not served yet
            STATUS_SERVED                 //Client served, safe to close connection and dispose
        }

        

        //Packet Type from packet's Header
        public enum REQUEST : uint
        {
            PACKET_EMPTY_REQUEST        =       0,                  //Request was empty!!
            PACKET_WRONG_REQUEST        =       10,                 //Request doesnt match any Provided by Service!!
            PACKET_DAMAGED_REQUEST      =       100,                //Packet Size doesnt equal the intended size!
            PACKET_SESSION_REQUEST      =       696969,             //Client asks to make an Authorised connection with the single use Session Key
            PACKET_WRONG_PASSWORD       =       969696              //Client asks to make an Authorised connection with the single use Session Key
        }




        public static string serverName         = "";
        public static string serverLocation     = "";
        public static string serverDbVersion    = "";

        public static string serverDbLogin      = "";
        public static string serverDbPass       = "";
        public static string isLogger           = "";


        public static void loadCfg()
        {
            Printer.Write("\n\nTroyAuthServer v" + SERVER_VERSION + " Authorization Service for TroyServer.\n\n", ConsoleColor.DarkCyan);
            var lines = File.ReadAllLines("CFG/Auth.cfg");
            if(lines.Length != CONFIG_SIZE)
            {
                Printer.Write("*** Config size does not mach the server version!!, will be skipped ***", ConsoleColor.Red);
                return;
            }
            else
            {
                serverName      = lines[0].Substring(lines[0].IndexOf(":: ") + 3);
                serverLocation  = lines[1].Substring(lines[1].IndexOf(":: ") + 3);
                serverDbVersion = lines[2].Substring(lines[2].IndexOf(":: ") + 3);
                serverDbLogin   = lines[3].Substring(lines[2].IndexOf(":: ") + 3);
                serverDbPass    = lines[4].Substring(lines[2].IndexOf(":: ") + 3);
                isLogger        = lines[5].Substring(lines[2].IndexOf(":: ") + 3);
                Printer.Write("Config Loaded with [" + CONFIG_SIZE + "] arguments.", ConsoleColor.Green);
                Printer.Write("Starting up The " + serverName + "'@'" + serverLocation + "\n", ConsoleColor.Yellow);
            }
        }
    }
}