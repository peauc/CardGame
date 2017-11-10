using System;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Channels;
using DotNetty.Codecs;

namespace CardGame.src.Server
{
    public class ServerInitializer : ChannelInitializer<TcpSocketChannel>
    {
        public ServerInitializer()
        {
            Console.WriteLine("Server is being initialized");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            Console.WriteLine("ChannelActive");
        }
        protected override void InitChannel(TcpSocketChannel channel)
        {
            IChannelPipeline c = channel.Pipeline;

            c.AddLast(new LineBasedFrameDecoder(1024));
            c.AddLast(new StringDecoder());
            c.AddLast(new StringEncoder());
            c.AddLast(new PlayerHandler());
        }
    }
}
