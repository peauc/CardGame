using System;
using System.Collections.Generic;
using CardGame.Protocol;

namespace Client.Networking
{
    public class BufferedPackets
    {
        static List<Message> List;

        public BufferedPackets()
        {
        }

        public static void AddMessage(Message m) 
        {
            List.Add(m);
        }

        public static Message GetMessage()
        {
            if (List.Count == 0)
                return (null);
            try
            {
                Message m = List[0];
                List.RemoveAt(0);
                return (m);
            }
            catch (Exception e) 
            {
                Console.Error.WriteLine(e.Message);
                return (null);
            }
        }
    }
}
