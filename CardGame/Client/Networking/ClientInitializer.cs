using System;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Client.Networking
{
    public class ClientInitializer : ChannelInitializer<TcpSocketChannel>
    {
        public ClientInitializer()
        {
            Console.WriteLine("Client Initializer");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }

        protected override void InitChannel(TcpSocketChannel channel)
        {
            IChannelPipeline pipe = channel.Pipeline;

            pipe.AddLast(new LoggingHandler());
            pipe.AddLast(new ClientHandler());
        }
    }
}
