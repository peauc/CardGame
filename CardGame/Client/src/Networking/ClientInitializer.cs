using System;
using DotNetty.Codecs.Protobuf;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Codecs.ProtocolBuffers;
using CardGame.Protocol;
using Google.ProtocolBuffers;

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
            pipe.AddLast(new ProtobufVarint32FrameDecoder());
            pipe.AddLast(handlers: new ProtobufDecoder(Message.Parser));

            pipe.AddLast(new ProtobufVarint32LengthFieldPrepender());
            pipe.AddLast(new ProtobufEncoder());
            pipe.AddLast(new ClientHandler());
        }
    }
}
