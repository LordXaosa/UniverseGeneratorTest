using System;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Init client. Connectiong...");
            ClientHandler client = new ClientHandler("localhost", 5123);
        }
    }
}
