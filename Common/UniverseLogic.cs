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
                universe.Sectors.Values.ToList().ForEach(p =>
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
            AStar a = new AStar(universe.Sectors.Count);
            if (start != null && end != null)
            {
                if (a.SearchPath(start, end, ignoreWeight))
                {
                    return new List<SectorModel>(a.PathByNodes);
                }
            }
            return null;
        }

        public List<SectorModel> FindRadius(SectorModel start, int maxHops)
        {
            AStarForDistance a = new AStarForDistance();
            a.FindSectors(start, maxHops);
            return a.FoundSectors;
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
            //Parallel.For(0, queue.Count, (i) =>
            for(int i = 0; i < queue.Count; i++)
            {
                result = cycles--;
                if (result <= 0)
                    break;
                SectorModel sector;
                queue.TryDequeue(out sector);
                var sectorChance = noise.GetValue(sector.X, 0, sector.Y);
                var gateChance = 1.6d;
                double v;
                Point3D npos = new Point3D(sector.X, sector.Y - 1, 0);
                //if (!universe.Sectors.ContainsKey(npos))
                {
                    v = noise.GetValue(sector.X, 0, sector.Y - 1);
                    if (v > chanced)
                    {
                        SectorModel north = new SectorModel(npos, ng.GetName(npos.GetHashCode()),
                            south: sector);
                        if (universe.Sectors.TryAdd(npos, north))
                        {
                            north.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                            RandomDangerLever(north);
                            sector.NorthGate = north;
                            queue.Enqueue(north);
                        }
                        else
                        {
                            if (sectorChance+v > gateChance)
                            {
                                north = universe.Sectors[npos];
                                sector.NorthGate = north;
                                north.SouthGate = sector;
                            }
                        }
                    }
                }

                Point3D spos = new Point3D(sector.Position.X, sector.Position.Y + 1, 0);
                //if (!universe.Sectors.ContainsKey(spos))
                {
                    v = noise.GetValue(sector.X, 0, sector.Y + 1);
                    if (v > chanced)
                    {
                        SectorModel south = new SectorModel(spos, ng.GetName(spos.GetHashCode()),
                            north: sector);
                        if (universe.Sectors.TryAdd(spos, south))
                        {
                            south.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                            RandomDangerLever(south);
                            sector.SouthGate = south;
                            queue.Enqueue(south);
                        }
                        else
                        {
                            if (sectorChance+v > gateChance)
                            {
                                south = universe.Sectors[spos];
                                sector.SouthGate = south;
                                south.NorthGate = sector;
                            }
                        }
                    }
                }

                Point3D wpos = new Point3D(sector.Position.X - 1, sector.Position.Y, 0);
                //if (!universe.Sectors.ContainsKey(wpos))
                {
                    v = noise.GetValue(sector.X - 1, 0, sector.Y);
                    if (v > chanced)
                    {
                        SectorModel west = new SectorModel(wpos, ng.GetName(wpos.GetHashCode()),
                            east: sector);
                        if (universe.Sectors.TryAdd(wpos, west))
                        {
                            west.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                            RandomDangerLever(west);
                            sector.WestGate = west;
                            queue.Enqueue(west);
                        }
                        else
                        {
                            if (sectorChance+v > gateChance)
                            {
                                west = universe.Sectors[wpos];
                                sector.WestGate = west;
                                west.EastGate = sector;
                            }
                        }
                    }
                }

                Point3D epos = new Point3D(sector.Position.X + 1, sector.Position.Y, 0);
                //if (!universe.Sectors.ContainsKey(epos))
                {
                    v = noise.GetValue(sector.X + 1, 0, sector.Y);
                    if (v > chanced)
                    {
                        SectorModel east = new SectorModel(epos, ng.GetName(epos.GetHashCode()),
                            west: sector);
                        if (universe.Sectors.TryAdd(epos, east))
                        {
                            east.Race = GetRace(biome.GetValue(sector.X, 0, sector.Y - 1));
                            RandomDangerLever(east);
                            sector.EastGate = east;
                            queue.Enqueue(east);
                        }
                        else
                        {
                            if (sectorChance+v > gateChance)
                            {
                                east = universe.Sectors[epos];
                                sector.EastGate = east;
                                east.WestGate = sector;
                            }
                        }
                    }
                }
            }//);
            return result;
        }
    }
}

