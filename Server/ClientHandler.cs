using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientHandler
    {
        TcpClient client;
        List<ClientHandler> clients;
        string host;
        public ClientHandler(TcpClient client, List<ClientHandler> clients)
        {
            this.clients = clients;
            this.client = client;

            IPEndPoint clientIp = (IPEndPoint)client.Client.RemoteEndPoint;
            host = clientIp.Address.ToString() + ":" + clientIp.Port;

            Task.Factory.StartNew(ProcessClient);
        }
        public void SendToClient(string data)
        {
            using (BinaryWriter bw = new BinaryWriter(client.GetStream(), Encoding.UTF8, true))
            {
                bw.Write(data);
            }
        }
        void ProcessClient()
        {
            try
            {
                while (client.Connected)
                {
                    using (BinaryReader br = new BinaryReader(client.GetStream(), Encoding.UTF8, true))
                    {
                        using (BinaryWriter bw = new BinaryWriter(client.GetStream(), Encoding.UTF8, true))
                        {
                            int packetId = br.ReadInt32();
                            switch (packetId)
                            {
                                case 0:
                                    string s = br.ReadString();
                                    bw.Write($"Нам пришла строка: {s}");
                                    break;
                                case 1:
                                    double d = br.ReadDouble();
                                    bw.Write($"Нам пришёл double: {d}");
                                    break;
                                case 2:
                                    bool b = br.ReadBoolean();
                                    bw.Write($"Нам пришёл bool: {b}");
                                    break;
                                default:
                                    bw.Write($"Нам пришёл левый пакет со значением: {packetId}");
                                    foreach (var c in clients)
                                    {
                                        if (c != this)
                                            c.SendToClient($"Какой то мудак нам прислал херню со значением {packetId}");
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                try { client.Close(); }
                catch { }
            }
            finally
            {
                clients.Remove(this);
                Console.WriteLine($"Client disconnected from {host}");
                foreach (var c in clients)
                {
                    if (c != this)
                    {
                        c.SendToClient($"От нас отсоединился {host}. Покасики!");
                    }
                }
            }
        }
    }
}
