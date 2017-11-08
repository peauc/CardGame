namespace CardGame
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using DotNetty.Handlers.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    internal class Server
    {
        private static void StartServer()
        {
            Console.WriteLine("StartServer");
            var EventLoopGroup = new MultithreadEventLoopGroup(1);
            try
            {
                ServerBootstrap b = new ServerBootstrap();
                b.Group(EventLoopGroup)
                 .Channel<TcpServerSocketChannel>()
                 .Option(ChannelOption.SoBacklog, 100)
                 .Handler(new LoggingHandler("LSTN"))
                 .ChildHandler(new CardGame.src.Server.ServerInitializer());
                var Channel = b.BindAsync(4242);
                Console.ReadKey();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private static void Main(string[] args)
        {
            StartServer();
            System.Console.WriteLine("Hello World!");
            System.Console.WriteLine("Hello");
        }
    }
}