using System.Net.Sockets;
using System.Text;


namespace TroyServer
{
    /// <summary>
    /// THIS IS COPY FROM AUTH SERVER, NEEDS TO BE REWRTITEN
    /// </summary>
    static class Auth
        {
        private static readonly Socket ParentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public static void connect()
        {
            int attempts = 0;

            while (!ParentSocket.Connected)
            {
                try
                {
                    attempts++;
                    Printer.TypeLine("\r\rParent Connection attempt " + attempts, ConsoleColor.Yellow);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ParentSocket.Connect("localhost", Server.PORT);
                }
                catch (SocketException) 
                {
                    //Console.Clear();
                }
            }

            Printer.Write("\nConnected to the parent", ConsoleColor.Green);
            RequestLoop();
        }

        private static void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");

            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }

        /// <summary>
        /// Close socket and exit program.
        /// </summary>
        private static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ParentSocket.Shutdown(SocketShutdown.Both);
            ParentSocket.Close();
            Environment.Exit(0);
        }

        private static void SendRequest()
        {
            Console.Write("Send a request: ");

            string? request = Console.ReadLine();
            if (request == null)
                request = "COULD NOT GET LINE";


            SendString(request);
            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ParentSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ParentSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);
        }
    }
}