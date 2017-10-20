using Common.Models;
using LibNoise;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Common
{
    public class UniverseLogic
    {
        MarkovNameGenerator ng = new MarkovNameGenerator(Words.WordsCatalogue, 0, 5);

        public Task GenerateUniverse(UniverseModel universe, int cycles, bool avoidDanger, double chanced, int seed)
        {
            return Task.Run(() =>
            {
                //PerlinNoise noise = new PerlinNoise(8973454);
                FastBillow noise = new FastBillow(seed);
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
                biome.Seed = seed;
                biome.Frequency = 0.8;

                ng.Reset();
                universe.Sectors.Clear();
                ConcurrentQueue<SectorModel> queue = new ConcurrentQueue<SectorModel>();
                if (universe.Sectors.Count == 0)
                {
                    Point3D pos = new Point3D(0, 0, 0);
                    SectorModel s = new SectorModel(pos, ng.GetName(pos.GetHashCode()));
                    s.Race = GetRace(biome.GetValue(pos.X, 0, pos.Y));
                    universe.Sectors.TryAdd(pos, s);
                    queue.Enqueue(s);
                }
                while (queue.Count != 0 && cycles-- > 0)
                {
                    cycles = GenerateUniverse(universe, queue, cycles, chanced, noise, biome);
                }
                universe.Sectors.Values.AsParallel().ForAll(p =>
                {
                    if (p.Position.X > universe.MaxX)
                        universe.MaxX = (int)p.Position.X;
                    if (p.Position.Y > universe.MaxY)
                        universe.MaxY = (int)p.Position.Y;
                    if (p.Position.X < universe.MinX)
                        universe.MinX = (int)p.Position.X;
                    if (p.Position.Y < universe.MinY)
                        universe.MinY = (int)p.Position.Y;
                });
            });
        }
        public Task GenerateUniverse(UniverseModel universe, int cycles, bool avoidDanger, double chanced)
        {
            return GenerateUniverse(universe, cycles, avoidDanger, chanced, 8973454);
        }



        public List<SectorModel> FindPath(UniverseModel universe, SectorModel start, SectorModel end, bool ignoreWeight)
        {
            AStar a = new AStar(universe.Sectors);
            if (start != null && end != null)
            {
                if (a.SearchPath(start, end, ignoreWeight))
                {
                    return new List<SectorModel>(a.PathByNodes);
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

        public void RandomDangerLever(SectorModel sector)
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

        public RaceEnum GetRace(double val)
        {
            if (val <= 0)
                return RaceEnum.Argon;
            if (val <= 0.1)
                return RaceEnum.Boron;
            if (val <= 0.2)
                return RaceEnum.Paranid;
            if (val <= 0.3)
                return RaceEnum.Split;
            if (val <= 0.4)
                return RaceEnum.Teladi;
            if (val <= 0.6)
                return RaceEnum.Pirate;
            if (val <= 0.8)
                return RaceEnum.Xenon;
            return RaceEnum.None;
        }

        private int GenerateUniverse(UniverseModel universe, ConcurrentQueue<SectorModel> queue, int cycles, double chanced, IModule noise, IModule biome)
        {
            int result = 0;
            Parallel.For(0, queue.Count, (i) =>
            {
                result = cycles--;
                if (result <= 0)
                    return;
                SectorModel sector;
                queue.TryDequeue(out sector);
                var v = noise.GetValue(sector.X, 0, sector.Y - 1);
                if (v > chanced)
                {
                    Point3D npos = new Point3D(sector.X, sector.Y - 1, 0);
                    SectorModel north = new SectorModel(npos, ng.GetName(sector.GetHashCode() ^ npos.GetHashCode()), south: sector);
                    if (universe.Sectors.TryAdd(npos, north))
                    {
                        north.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(north);
                        sector.NorthGate = north;
                        queue.Enqueue(north);
                    }
                    else
                    {
                        north = universe.Sectors[npos];
                        sector.NorthGate = north;
                        north.SouthGate = sector;
                    }
                }
                v = noise.GetValue(sector.X, 0, sector.Y + 1);
                if (v > chanced)
                {
                    Point3D spos = new Point3D(sector.Position.X, sector.Position.Y + 1, 0);
                    SectorModel south = new SectorModel(spos, ng.GetName(sector.GetHashCode() ^ spos.GetHashCode()), north: sector);
                    if (universe.Sectors.TryAdd(spos, south))
                    {
                        south.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(south);
                        sector.SouthGate = south;
                        queue.Enqueue(south);
                    }
                    else
                    {
                        south = universe.Sectors[spos];
                        sector.SouthGate = south;
                        south.NorthGate = sector;
                    }
                }
                v = noise.GetValue(sector.X - 1, 0, sector.Y);
                if (v > chanced)
                {
                    Point3D wpos = new Point3D(sector.Position.X - 1, sector.Position.Y, 0);
                    SectorModel west = new SectorModel(wpos, ng.GetName(sector.GetHashCode() ^ wpos.GetHashCode()), east: sector);
                    if (universe.Sectors.TryAdd(wpos, west))
                    {
                        west.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(west);
                        sector.WestGate = west;
                        queue.Enqueue(west);
                    }
                    else
                    {
                        west = universe.Sectors[wpos];
                        sector.WestGate = west;
                        west.EastGate = sector;
                    }
                }
                v = noise.GetValue(sector.X + 1, 0, sector.Y);
                if (v > chanced)
                {
                    Point3D epos = new Point3D(sector.Position.X + 1, sector.Position.Y, 0);
                    SectorModel east = new SectorModel(epos, ng.GetName(sector.GetHashCode() ^ epos.GetHashCode()), west: sector);
                    if (universe.Sectors.TryAdd(epos, east))
                    {
                        east.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                        RandomDangerLever(east);
                        sector.EastGate = east;
                        queue.Enqueue(east);
                    }
                    else
                    {
                        east = universe.Sectors[epos];
                        sector.EastGate = east;
                        east.WestGate = sector;
                    }
                }
            });
            return result;
        }

        public void MakeUniverseFromList(UniverseModel universe, List<SectorModel> list)
        {
            universe.Sectors = new ConcurrentDictionary<Point3D, SectorModel>();
            Parallel.ForEach(list, (item) =>
             {
                 universe.Sectors.TryAdd(item.Position, item);
             });
        }
    }
}

