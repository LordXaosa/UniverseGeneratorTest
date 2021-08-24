using System;
using System.Collections.Generic;
using Common;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port 5123...");
            Main m = new Main(5123);
            Console.WriteLine("Do you like to generate new universe(1) or load form file(2)?");
            string s = Console.ReadLine();
            if (s == "1")
            {
                Console.WriteLine("Generating universe...");
                m.GenerateNew();
            }
            else
            {
                Console.WriteLine("Loading universe...");
                m.LoadUniverse();
            }
            Console.WriteLine($"Universe loaded. Total {m.Universe.Sectors.Count} sectors.");
            m.Listen();
            Console.WriteLine("Server listening...");
            while(true)
            {
                Console.ReadLine();
            }
        }
    }
}
