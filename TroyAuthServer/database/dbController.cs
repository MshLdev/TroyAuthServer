using MySqlConnector;
using Dapper;

namespace TroyAuthServer
{
    class dbController
    {



        MySqlConnection connection          = new MySqlConnection("Server=localhost;User ID=" + Auth.serverDbLogin +";Password=" + Auth.serverDbPass + ";Database=db_Troy");
        public bool connectionoccupied      = false;    //if we get more requests per second, some of them will have to wait for their turn

        //Make connection to the database
        public bool connect()
        {
            try
            {
                //Console.WriteLine("Trying to establish connection with the database as " + Auth.serverDbLogin + "...");
                connection.Open();
            }
            catch (Exception e)
            {
                Printer.Write("I was not Able to connect to Database!\n(***" + e.Message + "***)\n", ConsoleColor.Red);
                Logger.systemfileLog("I was not Able to connect to Database!\n(***" + e.Message + "***)\n");
                return false;
            }
            //Connection was estabelished
            return true;
        }


        //Close the connection and cleanup
        public void close()
        {
            try
            {
                //Console.WriteLine("Trying to close DB...");
                connection.Close();
                connection.Dispose();
            }
            catch (Exception e)
            {
                Printer.Write("I was not Able to close Database!\n(***" + e.Message + "***)\n", ConsoleColor.Red);
            }
        }


        public string getUserSession(string login, string password)
        {
            string query = "SELECT * FROM users where Login = '"+ login +"' AND Password = '" + password + "';";
            List<DBOuser> result = connection.Query<DBOuser>(query).ToList();
            if (result.Count == 1)
            {
                return result[0].SessionKey;
            }
            else
            {
                return "AUTH.ERROR->[2137] -- Wrong Login or Password ";
            }
        }
        

        public bool SetupDB()
        {
            //Try to establish connection with the db
            while(true)
            {
                if(connect())
                {
                    Printer.Write(Auth.serverDbVersion + " Online, Connection Opened",ConsoleColor.Green );
                    return true;
                }
                else
                {
                    Printer.Write("Retry?? (Y/N)",ConsoleColor.Yellow );
                    string? answer = Console.ReadLine();
                    if(answer != null && answer.ToLower() == "y")
                    {
                        Console.WriteLine("Retrying Database Setup...");
                        continue;
                    }
                        
                    else
                        return false;
                }
            }
        }
    }
}