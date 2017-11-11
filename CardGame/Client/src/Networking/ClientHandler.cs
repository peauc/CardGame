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
            Message m = new Message
            {
                Type = Message.Types.Type.Prompt,
                Prompt = new Prompt() {
                    ToDisplay = {"Hello Server"}
                }
         };
            context.WriteAndFlushAsync(m);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            Console.WriteLine(msg.ToString());

            BufferedPackets.AddMessage(msg);
        }
    }
}
