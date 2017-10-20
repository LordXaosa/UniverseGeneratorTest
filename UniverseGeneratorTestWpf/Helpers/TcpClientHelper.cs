using Common;
using Common.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UniverseGeneratorTestWpf.Helpers.Network;

namespace UniverseGeneratorTestWpf.Helpers
{
    public class TcpClientHelper
    {
        TcpClient client;
        bool _isAuth = false;
        public bool IsAuth { get => _isAuth; set => _isAuth = value; }
        public delegate void PacketRecievedHandler(object sender, PacketEventArgs args);
        public event PacketRecievedHandler PacketRecieved;
        public TcpClientHelper(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            Task.Factory.StartNew(ProcessClient);
        }
        public void SendToServer(byte[] data)
        {
            using (BinaryWriter bw = new BinaryWriter(client.GetStream(), Encoding.UTF8, true))
            {
                bw.Write(data);
            }
        }

        public void SetAuth(bool isAuth)
        {
            _isAuth = isAuth;
        }
        public BinaryWriter GetWriter()
        {
            return new BinaryWriter(client.GetStream(), Encoding.UTF8, true);
        }
        public BinaryReader GetReader()
        {
            return new BinaryReader(client.GetStream(), Encoding.UTF8, true);
        }
        void ProcessClient()
        {
            while (client.Connected)
            {
                using (BinaryReader br = new BinaryReader(client.GetStream(), Encoding.UTF8, true))
                {
                    PacketRecieved?.Invoke(this, new PacketEventArgs(br));
                }
            }
        }
    }
}
