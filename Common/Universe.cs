using EMK.Cartography;
using EMK.LightGeometry;
using LibNoise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Universe
    {
        private Dictionary<Point3D, Sector> _sectors;
        public Dictionary<Point3D, Sector> Sectors
        {
            get { return _sectors; }
            set
            {
                _sectors = value;
            }
        }
        private Graph _graph;
        public Graph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
            }
        }
        Random rnd;
        MarkovNameGenerator ng = new MarkovNameGenerator(Words.WordsCatalogue, 0, 5);

        public Universe()
        {
            Sectors = new Dictionary<Point3D, Sector>();
            Graph = new Graph();
        }

        public void GenerateUniverse(int cycles, bool avoidDanger, double chanced)
        {
            Graph = new Graph();
            rnd = new Random(465845);
            //PerlinNoise noise = new PerlinNoise(8973454);
            FastBillow noise = new FastBillow(8973454);
            ng.Reset();
            Sectors.Clear();
            //universeHashSet.Clear();
            //bool[,] generated = new bool[1000, 1000];
            Queue<Sector> queue = new Queue<Sector>();
            //ConcurrentQueue<Sector> queue = new ConcurrentQueue<Sector>();
            if (Sectors.Count == 0)
            {
                Point3D pos = new Point3D(0, 0, 0);
                Sector s = new Sector(pos, ng.NextName);// {X = 0, Y = 0, Name = ng.NextName};
                Sectors.Add(pos, s);
                //universe.TryAdd(pos, s);
                Node n = new Node(pos);
                _graph.AddNode(n);
                //universeHashSet.Add(s);
                queue.Enqueue(s);
            }
            int maxX = 0;
            int maxY = 0;
            int minX = 0;
            int minY = 0;
            for (int i = 0; i < cycles; i++)
            //Parallel.For(0, cycles, (i) =>
            {
                if (queue.Count > 0)
                {
                    Arc arc;
                    Node n;
                    Sector sector = queue.Dequeue();
                    if (maxX < sector.X)
                    {
                        maxX = sector.X;
                    }
                    if (maxY < sector.Y)
                    {
                        maxY = sector.Y;
                    }
                    if (minX > sector.X)
                    {
                        minX = sector.X;
                    }
                    if (minY > sector.X)
                    {
                        minY = sector.X;
                    }
                    var v = noise.GetValue(sector.X, 0, sector.Y-1);//GetSector(noise, sector.X, sector.Y - 1);
                    Node currentNode = _graph.NodesDictionary[sector.Position];
                    //if (rnd.Next(100) < chance)
                    if (v > chanced)
                    {
                        Point3D npos = new Point3D(sector.X, sector.Y - 1, 0);
                        Sector north = new Sector(npos, ng.NextName, south: sector);
                        if (!Sectors.ContainsKey(npos))
                        {
                            RandomDangerLever(north);
                            sector.NorthGate = north;
                            queue.Enqueue(north);
                            Sectors.Add(npos, north);
                            n = new Node(npos);
                            _graph.AddNode(n);
                            arc = new Arc(currentNode, n, avoidDanger ? sector.NorthGate.DangerLevel : 1);
                            _graph.AddArc(arc);
                            arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                            _graph.AddArc(arc);
                        }
                        else
                        {
                            north = Sectors[npos];
                            sector.NorthGate = north;
                            north.SouthGate = sector;
                        }
                    }
                    //if (rnd.Next(100) < chance)
                    v = noise.GetValue(sector.X, 0, sector.Y+1);// GetSector(noise, sector.X, sector.Y + 1);
                    if (v > chanced)
                    {
                        Point3D spos = new Point3D(sector.Position.X, sector.Position.Y + 1, 0);
                        Sector south = new Sector(spos, ng.NextName, north: sector);
                        if (!Sectors.ContainsKey(spos))
                        {
                            RandomDangerLever(south);
                            sector.SouthGate = south;
                            queue.Enqueue(south);
                            Sectors.Add(spos, south);
                            n = new Node(spos);
                            _graph.AddNode(n);
                            arc = new Arc(currentNode, n, avoidDanger ? sector.SouthGate.DangerLevel : 1);
                            _graph.AddArc(arc);
                            arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                            _graph.AddArc(arc);
                        }
                        else
                        {
                            south = Sectors[spos];
                            sector.SouthGate = south;
                            south.NorthGate = sector;
                        }
                    }
                    //if (rnd.Next(100) < chance)
                    v = noise.GetValue(sector.X-1, 0, sector.Y);// GetSector(noise, sector.X - 1, sector.Y);
                    if (v > chanced)
                    {
                        Point3D wpos = new Point3D(sector.Position.X - 1, sector.Position.Y, 0);
                        Sector west = new Sector(wpos, ng.NextName, east: sector);
                        if (!Sectors.ContainsKey(wpos))
                        {
                            RandomDangerLever(west);
                            sector.WestGate = west;
                            queue.Enqueue(west);
                            Sectors.Add(wpos, west);
                            n = new Node(wpos);
                            _graph.AddNode(n);
                            arc = new Arc(currentNode, n, avoidDanger ? sector.WestGate.DangerLevel : 1);
                            _graph.AddArc(arc);
                            arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                            _graph.AddArc(arc);
                        }
                        else
                        {
                            west = Sectors[wpos];
                            sector.WestGate = west;
                            west.EastGate = sector;
                        }
                    }
                    //if (rnd.Next(100) < chance)
                    v = noise.GetValue(sector.X+1, 0, sector.Y);// GetSector(noise, sector.X + 1, sector.Y);
                    if (v > chanced)
                    {
                        Point3D epos = new Point3D(sector.Position.X + 1, sector.Position.Y, 0);
                        Sector east = new Sector(epos, ng.NextName, west: sector);
                        if (!Sectors.ContainsKey(epos))
                        {
                            RandomDangerLever(east);
                            sector.EastGate = east;
                            queue.Enqueue(east);
                            Sectors.Add(epos, east);
                            n = new Node(epos);
                            _graph.AddNode(n);
                            arc = new Arc(currentNode, n, avoidDanger ? sector.EastGate.DangerLevel : 1);
                            _graph.AddArc(arc);
                            arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                            _graph.AddArc(arc);
                        }
                        else
                        {
                            east = Sectors[epos];
                            sector.EastGate = east;
                            east.WestGate = sector;
                        }
                    }
                }
            }
        }

        public Node[] FindPath(Node start, Node end)
        {
            AStar a = new AStar(_graph);
            if (start != null && end != null)
            {
                if (a.SearchPath(start, end))
                {
                    return a.PathByNodes;
                }
            }
            return null;
        }

        private double GetSector(PerlinNoise noise, int x, int y)
        {
            double v =
                // First octave
                (noise.Noise(2 * x, 2 * y, -0.5) + 1) / 2 * 0.7 +
                // Second octave
                (noise.Noise(4 * x, 4 * y, 0) + 1) / 2 * 0.2 +
                // Third octave
                (noise.Noise(8 * x, 8 * y, +0.5) + 1) / 2 * 0.1;

            v = System.Math.Min(1, System.Math.Max(0, v));
            return v;
        }

        public void RandomDangerLever(Sector sector)
        {
            if (rnd.Next(100) < 20)
            {
                sector.DangerLevel = rnd.Next(100);
            }
        }
    }
}
