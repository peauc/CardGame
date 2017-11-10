﻿using System;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Channels;
using DotNetty.Codecs.Protobuf;
using CardGame.Protocol;

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
            Message m = new Message()
            {
                Type = Message.Types.Type.Prompt,
                Prompt = new Prompt()
                {
                    ToDisplay = { "Hello new little packet" },
                }
            };
            context.WriteAndFlushAsync(m);
        }
        protected override void InitChannel(TcpSocketChannel channel)
        {
            IChannelPipeline pipe = channel.Pipeline;

            pipe.AddLast(new ProtobufVarint32FrameDecoder());
            pipe.AddLast(new ProtobufDecoder(Message.Parser));
            pipe.AddLast(new ProtobufVarint32LengthFieldPrepender());
            pipe.AddLast(new ProtobufEncoder());
            pipe.AddLast(new PlayerHandler());
        }
    }
}
