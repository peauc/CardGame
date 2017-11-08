namespace CardGame.src.Server
{
    using System;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    public class PlayerHandler : SimpleChannelInboundHandler<String>
    {
        private static GameManager gm = new GameManager();

        public PlayerHandler()
        {
            Console.WriteLine("Player handler is starting");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);

            Console.WriteLine("Client is connecting"); 
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            Console.WriteLine("Client is disconnecting");
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, String msg)
        {
            Console.WriteLine(msg);
        }
    }
}
