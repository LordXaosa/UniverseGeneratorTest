using Common.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        bool isAuth = false;
        string host;
        UniverseModel universe;
        public ClientHandler(TcpClient client, List<ClientHandler> clients, UniverseModel universe)
        {
            this.clients = clients;
            this.client = client;
            this.universe = universe;
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
                            PacketsEnum packet = (PacketsEnum)br.ReadInt32();
                            switch (packet)
                            {
                                case PacketsEnum.Ping:
                                    Console.WriteLine("Ping-pong from "+host);
                                    bw.Write((int)PacketsEnum.Ping);//pong
                                    break;
                                case PacketsEnum.Login:
                                    Console.WriteLine("Login from " + host);
                                    string login = br.ReadString();
                                    string password = br.ReadString();
                                    if (login == "admin" && password == "pass")
                                    {
                                        isAuth = true;
                                        //bw.Write((int)PacketsEnum.Login);
                                        bw.Write(true);
                                    }
                                    else
                                    {
                                        //bw.Write((int)PacketsEnum.Login);
                                        bw.Write(false);
                                        client.Close();
                                    }
                                    break;
                                case PacketsEnum.GetUniverse:
                                    if (isAuth)
                                    {
                                        Console.WriteLine("Authorized request. Sending universe data.");
                                        List<SectorModel> list = universe.Sectors.Values.ToList();
                                        //bw.Write((int)PacketsEnum.GetUniverse);
                                        bw.Write(list.Count);
                                        foreach (var s in list)
                                        {
                                            s.WriteBinary(bw);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Non authorized request. Disconnecting client.");
                                        client.Close();
                                    }
                                    break;
                                default:
                                    bw.Write($"Нам пришёл левый пакет со значением: {(int)packet}");
                                    foreach (var c in clients)
                                    {
                                        if (c != this)
                                            c.SendToClient($"Какой то мудак нам прислал херню со значением {(int)packet}");
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
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
