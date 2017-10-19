using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Main m = new Main(5123);
            Console.WriteLine("Starting server on port 5123...");
            m.Listen();
            Console.WriteLine("Server listening...");
            while(true)
            {
                Console.ReadLine();
            }
        }
    }
}
