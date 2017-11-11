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
            }
            Parsing.Parser Prs = new Parsing.Parser(connection.Channel);
            for (;;)
            {
                Console.WriteLine("Bucle");
                while(Prs.ShouldParse()) 
                    Prs.Parse();
                CardGame.Protocol.Message m = null;
                while ((m = Networking.BufferedPackets.GetMessage()) != null)
                {
                    Console.WriteLine("Bucle2");
                    if (m.Type == CardGame.Protocol.Message.Types.Type.Prompt)
                    {
                        var messages = m.Prompt.ToDisplay;
                        Console.WriteLine(messages);
                        foreach (String s in messages)
                        {
                            Console.WriteLine(s);
                        }
                    }
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
