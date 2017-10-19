using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class ClientHandler
    {
        TcpClient client;
        public ClientHandler(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            Task.Factory.StartNew(ProcessClient);
            ProcessUserInput();
        }
        public void SendToServer(byte[] data)
        {
            using (BinaryWriter bw = new BinaryWriter(client.GetStream(), Encoding.UTF8, true))
            {
                bw.Write(data);
            }
        }
        void ProcessUserInput()
        {
            while(true)
            {
                string s = Console.ReadLine();
                int.TryParse(s, out int i);
                byte[] arr = BitConverter.GetBytes(i);
                SendToServer(arr);
            }
        }
        void ProcessClient()
        {
            while (client.Connected)
            {
                using (BinaryReader br = new BinaryReader(client.GetStream(), Encoding.UTF8, true))
                {
                    Console.WriteLine(br.ReadString());
                }
            }
        }
    }
}
