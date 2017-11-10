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

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            Console.WriteLine("ChannelRead0");
            Console.WriteLine(msg.ToString());
            BufferedPackets.AddMessage(msg);
        }
    }
}
