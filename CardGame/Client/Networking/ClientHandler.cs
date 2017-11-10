using System;
using DotNetty.Transport.Channels;

namespace Client.Networking
{
    public class ClientHandler : SimpleChannelInboundHandler<String>
    {
        public ClientHandler()
        {
            Console.WriteLine("ClientHandler");
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, string msg)
        {
            
        }
    }
}
