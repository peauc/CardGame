using System;
using Client.Networking;

namespace Client
{
    class Program
    {
        static public void Main(string[] args)
        {
            
            Connection connection = new Connection();
            try
            {
                connection.Connect().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            Parsing.Parser Prs = new Parsing.Parser(connection.Channel);
            while (Prs.ShouldParse())
            {
                Prs.Parse();
            }
        }
    }
}
