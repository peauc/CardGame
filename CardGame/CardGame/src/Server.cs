namespace Server
{
    using System;
    using DotNetty.Handlers.Logging;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    internal class ServerClass
    {
        private static void StartServer()
        {
            var EventLoopGroup = new MultithreadEventLoopGroup(1);
            try
            {
                ServerBootstrap b = new ServerBootstrap();
                b.Group(EventLoopGroup)
                 .Channel<TcpServerSocketChannel>()
                 .Option(ChannelOption.SoBacklog, 100)
                 .Handler(new LoggingHandler("LSTN"))
                 .ChildHandler(new ServerInitializer());
                Console.WriteLine("Input server's port ( enter set default values) ");
                int Port = 8090;
                String tmp = Console.ReadLine();
                if (tmp.Trim().Length != 0)
                    if (!int.TryParse(tmp, out Port))
                        Port = 8090;    
                b.BindAsync(Port).Wait();
                Console.WriteLine("Finished biding");
                for (;;) {}     
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