using System;
using Client.Networking;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Connection connection = new Connection();
            connection.Connect().Wait();
            Console.WriteLine("Connected and bye");
            for (;;)
            {
                Parsing.Parser Parser = new Parsing.Parser(connection.Channel);
                while(Parser.ShouldParse()) 
                    Parser.Parse();
                CardGame.Protocol.Message m = null;
                while ((m = Networking.BufferedPackets.GetMessage()) != null)
                {
                    if (m.Type == CardGame.Protocol.Message.Types.Type.Hand) 
                    {
                        String hand = Utils.HandToString(m.Hand);
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
