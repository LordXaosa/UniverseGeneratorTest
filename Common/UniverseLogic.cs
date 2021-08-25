using Common.Models;
using LibNoise;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Entities.Pathfinding;
using Common.Generators;
using Common.Pathfinding;

namespace Common
{
    public class UniverseLogic
    {
        MarkovNameGenerator ng = new MarkovNameGenerator(Words.WordsCatalogue, 0, 5);

        public Task GenerateUniverse(UniverseModel universe, int cycles, bool avoidDanger, double chanced, int seed)
        {
            return Task.Run(() =>
            {
                FastBillow noise = new FastBillow(seed);
                noise.NoiseQuality = NoiseQuality.High;
                noise.OctaveCount = 6;
                noise.Persistence = 0.6;
                noise.Lacunarity = 0.4;

                Voronoi biome = new Voronoi();
                biome.Seed = seed;
                biome.Frequency = 0.8;

                ng.Reset();
                universe.Sectors.Clear();
                ConcurrentQueue<SectorGraphNode> queue = new ConcurrentQueue<SectorGraphNode>();
                if (universe.Sectors.Count == 0)
                {
                    Point2D pos = new Point2D(0, 0);
                    var node = new SectorGraphNode(pos);
                    var sector = new SectorModel(ng.GetName(pos.GetHashCode()), node);
                    sector.Race = GetRace(biome.GetValue(pos.X, 0, pos.Y));
                    universe.Sectors.TryAdd(pos, sector);
                    queue.Enqueue(node);
                }

                while (queue.Count != 0 && cycles-- > 0)
                {
                    cycles = GenerateUniverse(universe, queue, cycles, chanced, noise, biome);
                }

                universe.Sectors.Values.ToList().ForEach(p =>
                {
                    if (p.Node.Position.X > universe.MaxX)
                        universe.MaxX = (int) p.Node.Position.X;
                    if (p.Node.Position.Y > universe.MaxY)
                        universe.MaxY = (int) p.Node.Position.Y;
                    if (p.Node.Position.X < universe.MinX)
                        universe.MinX = (int) p.Node.Position.X;
                    if (p.Node.Position.Y < universe.MinY)
                        universe.MinY = (int) p.Node.Position.Y;
                });
            });
        }

        public Task GenerateUniverse(UniverseModel universe, int cycles, bool avoidDanger, double chanced)
        {
            return GenerateUniverse(universe, cycles, avoidDanger, chanced, 8973454);
        }


        public List<SectorGraphNode> FindPath(UniverseModel universe, SectorGraphNode start, SectorGraphNode end,
            bool ignoreWeight)
        {
            AStar a = new AStar(universe.Sectors.Count);
            if (start != null && end != null)
            {
                if (a.SearchPath(start, end, ignoreWeight))
                {
                    return new List<SectorGraphNode>(a.PathByNodes);
                }
            }

            return null;
        }

        public List<SectorGraphNode> FindRadius(SectorGraphNode start, int maxHops)
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

        public void RandomDangerLever(SectorGraphNode sectorGraph)
        {
            if (GetXYNoise(sectorGraph.X, sectorGraph.Y) > 100)
            {
                sectorGraph.DangerLevel = (int) GetXYNoise2(sectorGraph.X, sectorGraph.Y);
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
                num = num * 541 + (uint) x;
                num = BitRotate(num);
                num = num * 809 + (uint) y;
                num = BitRotate(num);
                num = num * 673 + (uint) i;
                num = BitRotate(num);
            }

            return num % 200;
        }

        public static uint GetXYNoise2(int x, int y)
        {
            UInt32 num = 5743537;
            for (uint i = 0; i < 16; i++)
            {
                num = num * 541 + (uint) x;
                num = BitRotate(num);
                num = num * 809 + (uint) y;
                num = BitRotate(num);
                num = num * 673 + (uint) i;
                num = BitRotate(num);
            }

            return num % 100;
        }

        public RaceEnum GetRace(double val)
        {
            return val switch
            {
                <= 0 => RaceEnum.Argon,
                <= 0.1 => RaceEnum.Boron,
                <= 0.2 => RaceEnum.Paranid,
                <= 0.3 => RaceEnum.Split,
                <= 0.4 => RaceEnum.Teladi,
                <= 0.6 => RaceEnum.Pirate,
                <= 0.8 => RaceEnum.Xenon,
                _ => RaceEnum.None
            };
        }

        private int GenerateUniverse(UniverseModel universe, ConcurrentQueue<SectorGraphNode> queue, int cycles,
            double chanced, IModule noise, IModule biome)
        {
            int result = 0;
            var directions = Enum.GetValues(typeof(DirectionEnum));
            for (int i = 0; i < queue.Count; i++)
            {
                result = cycles--;
                if (result <= 0)
                    break;
                SectorGraphNode sectorGraph;
                queue.TryDequeue(out sectorGraph);
                var sectorChance = noise.GetValue(sectorGraph.X, 0, sectorGraph.Y);
                foreach (DirectionEnum direction in directions)
                {
                    GenerateSector(sectorGraph, direction, chanced, noise, biome, universe.Sectors, queue,
                        sectorChance);
                }
            }

            return result;
        }

        private void GenerateSector(SectorGraphNode parent, DirectionEnum direction, double chanced, IModule noise,
            IModule biome, ConcurrentDictionary<Point2D, SectorModel> dict,
            ConcurrentQueue<SectorGraphNode> queue, double sectorChance)
        {
            var gateChance = 1d;
            var x = direction switch
            {
                DirectionEnum.East => parent.X + 1,
                DirectionEnum.West => parent.X - 1,
                _ => parent.X
            };
            var y = direction switch
            {
                DirectionEnum.North => parent.Y - 1,
                DirectionEnum.South => parent.Y + 1,
                _ => parent.Y
            };
            Point2D newpos = new Point2D(x, y);
            var v = noise.GetValue(x, 0, y);
            if (v > chanced)
            {
                
                SectorGraphNode newSectorGraph = new SectorGraphNode(newpos);
                var sector = new SectorModel(ng.GetName(newpos.GetHashCode()), newSectorGraph);
                if (sectorChance + v > gateChance)
                {
                    if (dict.TryAdd(newpos, sector))
                    {
                        sector.Race = GetRace(biome.GetValue(x, 0, y));
                        RandomDangerLever(newSectorGraph);
                        switch (direction)
                        {
                            case DirectionEnum.North:
                                parent.NorthGate = newSectorGraph;
                                newSectorGraph.SouthGate = parent;
                                break;
                            case DirectionEnum.South:
                                parent.SouthGate = newSectorGraph;
                                newSectorGraph.NorthGate = parent;
                                break;
                            case DirectionEnum.East:
                                parent.EastGate = newSectorGraph;
                                newSectorGraph.WestGate = parent;
                                break;
                            case DirectionEnum.West:
                                parent.WestGate = newSectorGraph;
                                newSectorGraph.EastGate = parent;
                                break;
                        }

                        queue.Enqueue(newSectorGraph);
                    }
                    else
                    {
                        newSectorGraph = dict[newpos].Node;
                        switch (direction)
                        {
                            case DirectionEnum.North:
                                parent.NorthGate = newSectorGraph;
                                newSectorGraph.SouthGate = parent;
                                break;
                            case DirectionEnum.South:
                                parent.SouthGate = newSectorGraph;
                                newSectorGraph.NorthGate = parent;
                                break;
                            case DirectionEnum.East:
                                parent.EastGate = newSectorGraph;
                                newSectorGraph.WestGate = parent;
                                break;
                            case DirectionEnum.West:
                                parent.WestGate = newSectorGraph;
                                newSectorGraph.EastGate = parent;
                                break;
                        }
                    }
                }
            }
        }
    }
}