using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        public UniverseModel Universe { get; set; }

        UniverseLogic universe;
        public Main(int port)
        {
            clients = new List<ClientHandler>();
            server = new TcpListener(IPAddress.Any, port);
            accept = true;
            universe = new UniverseLogic();
        }

        public void LoadUniverse()
        {
            Universe = new UniverseModel();
            List<SectorModel> list = new List<SectorModel>();
            using (BinaryReader br = new BinaryReader(File.Open("Universe.dat", FileMode.Open)))
            {
                Universe.ReadBinary(br);
            }
        }
        public void GenerateNew()
        {
            Universe = new UniverseModel();
            Random r = new Random();
            universe.GenerateUniverse(Universe, 1000, true, 0.00d, r.Next()).Wait();
        }

        public async Task Listen()
        {
            server.Start();
            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                if (client != null)
                {
                    client.Client.ReceiveTimeout = 60000;
                    IPEndPoint clientIp = (IPEndPoint) client.Client.RemoteEndPoint;
                    Console.WriteLine($"Client connected from {clientIp.Address.ToString()} on port {clientIp.Port}");
                    foreach (var cl in clients)
                    {
                        cl.SendToClient(
                            $"К нам пришёл {clientIp.Address.ToString()} на порту {clientIp.Port}. Встречаем!");
                    }

                    ClientHandler c = new ClientHandler(client, clients, Universe);
                    clients.Add(c);
                }
            }
        }
    }
}
