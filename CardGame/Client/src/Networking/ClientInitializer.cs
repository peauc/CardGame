﻿using System;
using DotNetty.Codecs.Protobuf;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using CardGame.Protocol;

namespace Client.Networking
{
    public class ClientInitializer : ChannelInitializer<TcpSocketChannel>
    {
        public ClientInitializer()
        {
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }
        protected override void InitChannel(TcpSocketChannel channel)
        {
            IChannelPipeline pipe = channel.Pipeline;

            pipe.AddLast(new ProtobufVarint32FrameDecoder());
            pipe.AddLast(new ProtobufDecoder(Message.Parser));
            pipe.AddLast(new ProtobufVarint32LengthFieldPrepender());
            pipe.AddLast(new ProtobufEncoder());
            pipe.AddLast(new ClientHandler());
        }
    }
}
