#nullable disable
using System.Net.Sockets;


namespace TroyServer
{
    class Account
    {
        public int          accId       {get; set;}
        public string       Login       {get; set;}
        public string       Password    {get; set;}
        public string       Email       {get; set;}
        public string       SessionKey  {get; set;}
        public byte         Flags       {get; set;}
        public DateTime     Birth       {get;set;}

    }
}