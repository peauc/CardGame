using System;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Client.Networking
{
    public class Connection
    {
        private static MultithreadEventLoopGroup _EventLoopGroup;
        private Bootstrap _Bootstrap;
        private int Port {get;set;}
        private string Hostname {get;set;}
        public IChannel Channel { get; set;}

        public Connection() 
        {
            Console.WriteLine("Creating new connection");
            Port = 8090;
            Hostname = "127.0.0.1";
        }

        public async Task Connect()
        {
            _Bootstrap = new Bootstrap();
            _EventLoopGroup = new MultithreadEventLoopGroup(1);
            _Bootstrap
                .Group(_EventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Handler(new ClientInitializer());
            try
            {
                Console.WriteLine("Please input hostname, enter set default value");
                string tmp = Console.In.ReadLine();
                if (tmp.Trim().Length != 0)
                    Hostname = tmp;
                tmp = Console.In.ReadLine();
                Console.WriteLine("Please input port, enter set default value");
                if (tmp.Trim().Length != 0 && int.TryParse(tmp, out int tmp2))
                    Port = tmp2;
                Channel = await _Bootstrap.ConnectAsync(Hostname, Port);
            }
            catch 
            {
                Console.WriteLine("Cannot connect, please retry");
                throw;
            }

        }
    }
}
