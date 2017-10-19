using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Main
    {
        TcpListener server;
        List<ClientHandler> clients;
        bool accept;
        public Main(int port)
        {
            clients = new List<ClientHandler>();
            server = new TcpListener(IPAddress.Any, port);
            accept = true;
        }

        public void Listen()
        {
            Task.Factory.StartNew(() =>
            {
                server.Start();
                while (true)
                {
                    var clientTask = server.AcceptTcpClientAsync();
                    if (clientTask.Result != null)
                    {
                        var client = clientTask.Result;
                        client.Client.ReceiveTimeout = 10000;
                        IPEndPoint clientIp = (IPEndPoint)client.Client.RemoteEndPoint;
                        Console.WriteLine($"Client connected from {clientIp.Address.ToString()} on port {clientIp.Port}");
                        foreach(var cl in clients)
                        {
                            cl.SendToClient($"К нам пришёл {clientIp.Address.ToString()} на порту {clientIp.Port}. Встречаем!");
                        }
                        ClientHandler c = new ClientHandler(client, clients);
                        clients.Add(c);
                    }
                }
            });
        }
    }
}
