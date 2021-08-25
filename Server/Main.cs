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
        readonly TcpListener _server;
        readonly List<ClientHandler> _clients;
        bool _accept;
        public UniverseModel Universe { get; set; }

        readonly UniverseLogic _universe;

        public Main(int port)
        {
            _clients = new List<ClientHandler>();
            _server = new TcpListener(IPAddress.Any, port);
            _accept = true;
            _universe = new UniverseLogic();
        }

        public void LoadUniverse()
        {
            Universe = new UniverseModel();
            var list = new List<SectorModel>();
            using (BinaryReader br = new BinaryReader(File.Open("Universe.dat", FileMode.Open)))
            {
                Universe.ReadBinary(br);
            }
        }

        public void GenerateNew()
        {
            Universe = new UniverseModel();
            var r = new Random();
            _universe.GenerateUniverse(Universe, 1000, true, 0.00d, r.Next()).Wait();
        }

        public async Task Listen()
        {
            _server.Start();
            while (true)
            {
                var client = await _server.AcceptTcpClientAsync();
                client.Client.ReceiveTimeout = 60000;
                IPEndPoint clientIp = (IPEndPoint) client.Client.RemoteEndPoint;
                Console.WriteLine($"Client connected from {clientIp.Address.ToString()} on port {clientIp.Port}");
                foreach (var cl in _clients)
                {
                    /*cl.SendToClient(
                        $"К нам пришёл {clientIp.Address.ToString()} на порту {clientIp.Port}. Встречаем!");*/
                }

                ClientHandler c = new ClientHandler(client, _clients, Universe);
                _clients.Add(c);
            }
        }
    }
}