﻿using LibNoise;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        /*private Graph _graph;
        public Graph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
            }
        }*/
        Random rnd;
        MarkovNameGenerator ng = new MarkovNameGenerator(Words.WordsCatalogue, 0, 5);

        public Universe()
        {
            Sectors = new ConcurrentDictionary<Point3D, Sector>();
            //Graph = new Graph();
        }

        public int MaxY { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MinX { get; set; }

        public Task GenerateUniverse(int cycles, bool avoidDanger, double chanced)
        {
            return Task.Run(() =>
            {
                //Graph = new Graph();
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
                    //Node n = new Node(pos);
                    //_graph.AddNode(n);
                    queue.Enqueue(s);
                }
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (queue.Count != 0 && cycles-- > 0)
                {
                    cycles = GenerateUniverse(queue, cycles, chanced, noise, biome);
                }
                sw.Stop();
                sw.Reset();
                sw.Start();
                //AddNodes(Graph, Sectors.Values.ToList(), avoidDanger);
                sw.Stop();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                MaxX = (int)Sectors.Values.AsParallel().Max(p => p.Position.X);
                MaxY = (int)Sectors.Values.AsParallel().Max(p => p.Position.Y); ;
                MinX = (int)Sectors.Values.AsParallel().Min(p => p.Position.X); ;
                MinY = (int)Sectors.Values.AsParallel().Min(p => p.Position.Y); ;
            });
        }



        public List<Sector> FindPath(Sector start, Sector end, bool ignoreWeight)
        {
            AStar a = new AStar(Sectors);
            //a.ChoosenHeuristic = AStar.EuclidianHeuristic;
            if (start != null && end != null)
            {
                if (a.SearchPath(start, end, ignoreWeight))
                {
                    return new List<Sector>(a.PathByNodes);
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

        private int GenerateUniverse(ConcurrentQueue<Sector> queue, int cycles, double chanced, IModule noise, IModule biome)
        {
            int result = 0;
            Parallel.For(0, queue.Count, (i) =>
            {
                result = cycles--;
                if (result <= 0)
                    return;
                Sector sector;
                queue.TryDequeue(out sector);
                var v = noise.GetValue(sector.X, 0, sector.Y - 1);
                if (v > chanced)
                {
                    Point3D npos = new Point3D(sector.X, sector.Y - 1, 0);
                    Sector north = new Sector(npos, ng.GetName(npos.GetHashCode()), south: sector);
                    if (Sectors.TryAdd(npos, north))
                    {
                        north.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(north);
                        sector.NorthGate = north;
                        queue.Enqueue(north);
                    }
                    else
                    {
                        north = Sectors[npos];
                        sector.NorthGate = north;
                        north.SouthGate = sector;
                    }
                }
                v = noise.GetValue(sector.X, 0, sector.Y + 1);
                if (v > chanced)
                {
                    Point3D spos = new Point3D(sector.Position.X, sector.Position.Y + 1, 0);
                    Sector south = new Sector(spos, ng.GetName(spos.GetHashCode()), north: sector);
                    if (Sectors.TryAdd(spos, south))
                    {
                        south.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(south);
                        sector.SouthGate = south;
                        queue.Enqueue(south);
                    }
                    else
                    {
                        south = Sectors[spos];
                        sector.SouthGate = south;
                        south.NorthGate = sector;
                    }
                }
                v = noise.GetValue(sector.X - 1, 0, sector.Y);
                if (v > chanced)
                {
                    Point3D wpos = new Point3D(sector.Position.X - 1, sector.Position.Y, 0);
                    Sector west = new Sector(wpos, ng.GetName(wpos.GetHashCode()), east: sector);
                    if (Sectors.TryAdd(wpos, west))
                    {
                        west.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(west);
                        sector.WestGate = west;
                        queue.Enqueue(west);
                    }
                    else
                    {
                        west = Sectors[wpos];
                        sector.WestGate = west;
                        west.EastGate = sector;
                    }
                }
                v = noise.GetValue(sector.X + 1, 0, sector.Y);
                if (v > chanced)
                {
                    Point3D epos = new Point3D(sector.Position.X + 1, sector.Position.Y, 0);
                    Sector east = new Sector(epos, ng.GetName(epos.GetHashCode()), west: sector);
                    if (Sectors.TryAdd(epos, east))
                    {
                        east.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(east);
                        sector.EastGate = east;
                        queue.Enqueue(east);
                    }
                    else
                    {
                        east = Sectors[epos];
                        sector.EastGate = east;
                        east.WestGate = sector;
                    }
                }
            });
            return result;
        }
    }
}

