namespace Server
{
    using System;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using CardGame.Protocol;

    public class PlayerHandler : SimpleChannelInboundHandler<Message>
    {
        private static GameManager gm = new GameManager();

        public PlayerHandler()
        {
            Console.WriteLine("Player handler is starting");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            Game.Player P = new Game.Player(context, "");
            gm.AddPlayerToGame(P);
            Console.WriteLine("Client has connecting");
            Message m = new Message()
            {
                Type = Message.Types.Type.Prompt,
                Prompt = new Prompt()
                {
                    ToDisplay = { "Welcome to our super Coinche server hosted by peau_c and samuel_r", "Remember to chose a nickname by typing \"NAME yournickname\"" }
                }
            };
            context.WriteAndFlushAsync(m);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            Console.WriteLine("Client is disconnecting");
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            Game.Game G;
            Game.Player P;

            if ((P = gm.FindPlayerByContext(ctx)) == null)
            {
                Console.WriteLine("Unknown Player");
                return;
            }
            if ((G = gm.FindGameByPlayer(P)) == null)
            {
                Console.WriteLine("Unknown game");
                return;
            }
            G.HandlePlay(msg, P);
            Console.WriteLine(msg);

        }
    }
}
