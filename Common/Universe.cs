using EMK.Cartography;
using EMK.LightGeometry;
using LibNoise;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Universe
    {
        private ConcurrentDictionary<Point3D, Sector> _sectors;
        public ConcurrentDictionary<Point3D, Sector> Sectors
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
            Sectors = new ConcurrentDictionary<Point3D, Sector>();
            Graph = new Graph();
        }

        public int MaxY { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MinX { get; set; }

        public Task GenerateUniverse(int cycles, bool avoidDanger, double chanced)
        {
            return Task.Run(() =>
            {
                Graph = new Graph();
                rnd = new Random(465845);
                //PerlinNoise noise = new PerlinNoise(8973454);
                FastBillow noise = new FastBillow(8973454);
                noise.NoiseQuality = NoiseQuality.High;
                noise.OctaveCount = 6;
                noise.Persistence = 0.6;
                noise.Lacunarity = 0.4;
                /*Perlin noise = new Perlin();
                noise.Seed = 8973454;
                noise.NoiseQuality = NoiseQuality.High;
                noise.OctaveCount = 10;
                noise.Persistence = 1.4;
                noise.Lacunarity = 0.35;
                noise.Frequency = 0.05;*/

                Voronoi biome = new Voronoi();
                biome.Seed = 8973454;
                biome.Frequency = 0.8;

                ng.Reset();
                Sectors.Clear();
                ConcurrentQueue<Sector> queue = new ConcurrentQueue<Sector>();
                if (Sectors.Count == 0)
                {
                    Point3D pos = new Point3D(0, 0, 0);
                    Sector s = new Sector(pos, ng.GetName(pos.GetHashCode()));
                    s.Race = GetRace(biome.GetValue(pos.X, 0, pos.Y));
                    Sectors.TryAdd(pos, s);
                    Node n = new Node(pos);
                    _graph.AddNode(n);
                    queue.Enqueue(s);
                }
                for (int i = 0; i < cycles; i++)
                //while(queue.Count>0)
                //Parallel.For(0, cycles, (i) =>
                {
                    if (queue.Count > 0)
                    {
                        Arc arc;
                        Node n;
                        Sector sector;
                        queue.TryDequeue(out sector);
                        //var v = (noise.GetValue(sector.X, 0, sector.Y - 1)+1)/2.0;//GetSector(noise, sector.X, sector.Y - 1);
                        var v = noise.GetValue(sector.X, 0, sector.Y - 1);//GetSector(noise, sector.X, sector.Y - 1);
                        Node currentNode = _graph.NodesDictionary[sector.Position];
                        //if (rnd.Next(100) < chance)
                        if (v > chanced)
                        {
                            Point3D npos = new Point3D(sector.X, sector.Y - 1, 0);
                            Sector north = new Sector(npos, ng.GetName(npos.GetHashCode()), south: sector);
                            //north.Race = GetRace(v);
                            if (Sectors.TryAdd(npos, north))
                            {
                                north.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                                RandomDangerLever(north);
                                sector.NorthGate = north;
                                queue.Enqueue(north);
                                
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

                                n = _graph.NodesDictionary[npos];
                                arc = new Arc(currentNode, n, avoidDanger ? sector.NorthGate.DangerLevel : 1);
                                _graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                _graph.AddArc(arc);
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = noise.GetValue(sector.X, 0, sector.Y + 1);// GetSector(noise, sector.X, sector.Y + 1);
                        if (v > chanced)
                        {
                            Point3D spos = new Point3D(sector.Position.X, sector.Position.Y + 1, 0);
                            Sector south = new Sector(spos, ng.GetName(spos.GetHashCode()), north: sector);
                            if (Sectors.TryAdd(spos, south))
                            {
                                //south.Race = GetRace(v);
                                south.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                                RandomDangerLever(south);
                                sector.SouthGate = south;
                                queue.Enqueue(south);
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

                                n = _graph.NodesDictionary[spos];
                                arc = new Arc(currentNode, n, avoidDanger ? sector.SouthGate.DangerLevel : 1);
                                _graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                _graph.AddArc(arc);
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = noise.GetValue(sector.X - 1, 0, sector.Y);// GetSector(noise, sector.X - 1, sector.Y);
                        if (v > chanced)
                        {
                            Point3D wpos = new Point3D(sector.Position.X - 1, sector.Position.Y, 0);
                            Sector west = new Sector(wpos, ng.GetName(wpos.GetHashCode()), east: sector);
                            //west.Race = GetRace(v);
                            if (Sectors.TryAdd(wpos, west))
                            {
                                west.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                                RandomDangerLever(west);
                                sector.WestGate = west;
                                queue.Enqueue(west);
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

                                n = _graph.NodesDictionary[wpos];
                                arc = new Arc(currentNode, n, avoidDanger ? sector.WestGate.DangerLevel : 1);
                                _graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                _graph.AddArc(arc);
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = noise.GetValue(sector.X + 1, 0, sector.Y);// GetSector(noise, sector.X + 1, sector.Y);
                        if (v > chanced)
                        {
                            Point3D epos = new Point3D(sector.Position.X + 1, sector.Position.Y, 0);
                            Sector east = new Sector(epos, ng.GetName(epos.GetHashCode()), west: sector);
                            //east.Race = GetRace(v);
                            if (Sectors.TryAdd(epos, east))
                            {
                                east.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                                RandomDangerLever(east);
                                sector.EastGate = east;
                                queue.Enqueue(east);
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

                                n = _graph.NodesDictionary[epos];
                                arc = new Arc(currentNode, n, avoidDanger ? sector.EastGate.DangerLevel : 1);
                                _graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                _graph.AddArc(arc);
                            }
                        }
                    }
                }
                MaxX = (int)Sectors.Values.AsParallel().Max(p => p.Position.X);
                MaxY = (int)Sectors.Values.AsParallel().Max(p => p.Position.Y); ;
                MinX = (int)Sectors.Values.AsParallel().Min(p => p.Position.X); ;
                MinY = (int)Sectors.Values.AsParallel().Min(p => p.Position.Y); ;
            });
        }

        public Node[] FindPath(Node start, Node end, bool ignoreWeight)
        {
            AStar a = new AStar(_graph);
            if (start != null && end != null)
            {
                if (a.SearchPath(start, end, ignoreWeight))
                {
                    return a.PathByNodes;
                }
            }
            return null;
        }

        public List<Sector> FindPath(Point3D start, Point3D end, bool ignoreWeight)
        {
            List<Sector> result = new List<Sector>();
            Node startNode = Graph.NodesDictionary[start];
            Node endNode = Graph.NodesDictionary[end];
            Node[] nodes = FindPath(startNode, endNode, ignoreWeight);
            foreach (Node n in nodes)
            {
                result.Add(Sectors[n.Position]);
            }
            return result;
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
            if (GetXYNoise(sector.X, sector.Y) > 100)
            {
                sector.DangerLevel = (int)GetXYNoise2(sector.X, sector.Y);
            }
            
        }

        public static uint BitRotate(uint x)
        {
            const int bits = 16;
            return (x << bits) | (x >> (32 - bits));
        }

        public static uint GetXYNoise(int x, int y)
        {
            UInt32 num = 8462134;
            for (uint i = 0; i < 16; i++)
            {
                num = num * 541 + (uint)x;
                num = BitRotate(num);
                num = num * 809 + (uint)y;
                num = BitRotate(num);
                num = num * 673 + (uint)i;
                num = BitRotate(num);
            }
            return num % 200;
        }

        public static uint GetXYNoise2(int x, int y)
        {
            UInt32 num = 5743537;
            for (uint i = 0; i < 16; i++)
            {
                num = num * 541 + (uint)x;
                num = BitRotate(num);
                num = num * 809 + (uint)y;
                num = BitRotate(num);
                num = num * 673 + (uint)i;
                num = BitRotate(num);
            }
            return num % 100;
        }

        public Race GetRace(double val)
        {
            if (val <= 0)
                return Race.Argon;
            if (val <= 0.1)
                return Race.Boron;
            if (val <= 0.2)
                return Race.Paranid;
            if (val <= 0.3)
                return Race.Split;
            if (val <= 0.4)
                return Race.Teladi;
            if (val <= 0.6)
                return Race.Pirate;
            if (val <= 0.8)
                return Race.Xenon;
            return Race.None;
        }        
    }
}

