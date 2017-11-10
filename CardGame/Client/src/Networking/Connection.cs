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
            Port = 4242;
            Hostname = "localhost";
        }

        public async Task Connect()
        {
            _Bootstrap = new Bootstrap();
            _EventLoopGroup = new MultithreadEventLoopGroup(1);
            _Bootstrap
                .Group(_EventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Handler(handler: new ClientInitializer());
            Channel = await _Bootstrap.ConnectAsync(Hostname, Port);

        }
    }
}
