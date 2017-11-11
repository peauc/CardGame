using System;
using DotNetty.Transport.Channels;
using CardGame.Protocol;

namespace Client.Networking
{
    public class ClientHandler : SimpleChannelInboundHandler<Message>
    {
        public ClientHandler()
        {
            Console.WriteLine("ClientHandler");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message m)
        {
            if (m.Type == CardGame.Protocol.Message.Types.Type.Reply)
            {
                String messages = m.Reply.Message;
                Console.WriteLine("[SERVER] " + messages);
            }
            if (m.Type == CardGame.Protocol.Message.Types.Type.Prompt)
            {
                var messages = m.Prompt.ToDisplay;
                foreach (String s in messages)
                {
                    Console.WriteLine("[SERVER] " + s);
                }
            }
            if (m.Type == CardGame.Protocol.Message.Types.Type.Hand)
            {
                foreach (string s in Utils.HandToString(m.Hand))
                {
                    Console.WriteLine(s);
                }
            }
        }
    }
}
