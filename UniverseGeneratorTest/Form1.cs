using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EMK.Cartography;

namespace UniverseGeneratorTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Sector> universe = new List<Sector>();
        HashSet<Sector> universeHashSet = new HashSet<Sector>();

        Bitmap img = new Bitmap(1000, 1000);

        //bool[,] generated = new bool[100, 100];

        private bool avoidDanger = true;

        MarkovNameGenerator ng = new MarkovNameGenerator(Words.WordsCatalogue, 0, 5);
        private int chance = 55;
        private double chanced = 0.481d;
        Random rnd;
        private async void startBt_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                rnd = new Random(123456);
                PerlinNoise noise = new PerlinNoise(8973454);
                ng.Reset();
                universe.Clear();
                universeHashSet.Clear();
                //bool[,] generated = new bool[1000, 1000];
                Queue<Sector> queue = new Queue<Sector>();

                if (universe.Count == 0)
                {
                    Sector s = new Sector() {X = 0, Y = 0, Name = ng.NextName};
                    universe.Add(s);
                    universeHashSet.Add(s);
                    queue.Enqueue(s);
                }
                int cycles = int.Parse(cyclesTb.Text);
                for (int i = 0; i < cycles; i++)
                    //Parallel.For(0, cycles, (i) =>
                {
                    if (queue.Count > 0)
                    {
                        Sector sector = queue.Dequeue();
                        var v = GetSector(noise, sector.X, sector.Y-1);
                        //if (rnd.Next(100) < chance)
                        if (v > chanced)
                        {
                            Sector north = new Sector()
                            {
                                X = sector.X,
                                Y = sector.Y - 1,
                                SouthGate = sector,
                                Name = ng.NextName
                            };
                            if (universeHashSet.Add(north))
                            {
                                RandomDangerLever(north);
                                sector.NorthGate = north;
                                queue.Enqueue(north);
                                universe.Add(north);
                            }
                            else
                            {
                                north = universe.FirstOrDefault(p => p.X == sector.X && p.Y == sector.Y - 1);
                                if (north != null)
                                {
                                    sector.NorthGate = north;
                                    north.SouthGate = sector;
                                }
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = GetSector(noise, sector.X, sector.Y + 1);
                        if (v > chanced)
                        {
                            Sector south = new Sector()
                            {
                                X = sector.X,
                                Y = sector.Y + 1,
                                NorthGate = sector,
                                Name = ng.NextName
                            };
                            if (universeHashSet.Add(south))
                            {
                                RandomDangerLever(south);
                                sector.SouthGate = south;
                                queue.Enqueue(south);
                                universe.Add(south);
                            }
                            else
                            {
                                south = universe.FirstOrDefault(p => p.X == sector.X && p.Y == sector.Y + 1);
                                if (south != null)
                                {
                                    sector.SouthGate = south;
                                    south.NorthGate = sector;
                                }
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = GetSector(noise, sector.X - 1, sector.Y);
                        if (v > chanced)
                        {
                            Sector west = new Sector()
                            {
                                X = sector.X - 1,
                                Y = sector.Y,
                                EastGate = sector,
                                Name = ng.NextName
                            };
                            if (universeHashSet.Add(west))
                            {
                                RandomDangerLever(west);
                                sector.WestGate = west;
                                queue.Enqueue(west);
                                universe.Add(west);
                            }
                            else
                            {
                                west = universe.FirstOrDefault(p => p.X == sector.X - 1 && p.Y == sector.Y);
                                if (west != null)
                                {
                                    sector.WestGate = west;
                                    west.EastGate = sector;
                                }
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = GetSector(noise, sector.X + 1, sector.Y);
                        if (v > chanced)
                        {
                            Sector east = new Sector()
                            {
                                X = sector.X + 1,
                                Y = sector.Y,
                                WestGate = sector,
                                Name = ng.NextName
                            };
                            if (universeHashSet.Add(east))
                            {
                                RandomDangerLever(east);
                                sector.EastGate = east;
                                queue.Enqueue(east);
                                universe.Add(east);
                            }
                            else
                            {
                                east = universe.FirstOrDefault(p => p.X == sector.X + 1 && p.Y == sector.Y);
                                if (east != null)
                                {
                                    sector.EastGate = east;
                                    east.WestGate = sector;
                                }
                            }
                        }
                    }
                }
                img = new Bitmap(10000, 10000);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Graphics g = Graphics.FromImage(img);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, 10000, 10000);
                for (int i = 0; i < universe.Count; i++)
                {
                    int x = universe[i].X*40 - 8 + 500;
                    int y = universe[i].Y*40 - 8 + 500;
                    if (universe[i].X == 0 && universe[i].Y == 0)
                    {
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Green)), x + img.Width/2, y + img.Height/2, 16, 16);
                    }
                    else
                    {
                        if (universe[i].DangerLevel == 0)
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), x + img.Width/2, y + img.Height/2, 16,
                                16);
                        else if (universe[i].DangerLevel <= 30)
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Orange)), x + img.Width/2, y + img.Height/2, 16,
                                16);
                        else if (universe[i].DangerLevel <= 60)
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Red)), x + img.Width/2, y + img.Height/2, 16,
                                16);
                        else
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Brown)), x + img.Width/2, y + img.Height/2, 16,
                                16);
                    }
                    if (universe[i].NorthGate != null)
                    {
                        g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 8 + img.Width/2, y + img.Height/2,
                            x + 8 + img.Width/2, y - 24 + img.Height/2);
                    }
                    if (universe[i].EastGate != null)
                    {
                        g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 16 + img.Width/2, y + 8 + img.Height/2,
                            x + 40 + img.Width/2, y + 8 + img.Height/2);
                    }
                }
                Graph graph = new Graph();
                foreach (Sector sector in universe)
                {
                    Node n = new Node(sector.X, sector.Y, 0);
                    graph.AddNode(n);
                }
                foreach (Sector sector in universe)
                {
                    Arc arc;
                    Node sec = graph.Nodes.First(p => p.X == sector.X && p.Y == sector.Y);
                    if (sector.NorthGate != null)
                    {
                        Node n = graph.Nodes.First(p => p.X == sector.NorthGate.X && p.Y == sector.NorthGate.Y);
                        arc = new Arc(sec, n, avoidDanger ? sector.NorthGate.DangerLevel : 1);
                        graph.AddArc(arc);
                    }
                    if (sector.SouthGate != null)
                    {
                        Node s = graph.Nodes.First(p => p.X == sector.SouthGate.X && p.Y == sector.SouthGate.Y);
                        arc = new Arc(sec, s, avoidDanger ? sector.SouthGate.DangerLevel : 1);
                        graph.AddArc(arc);
                    }
                    if (sector.WestGate != null)
                    {
                        Node w = graph.Nodes.First(p => p.X == sector.WestGate.X && p.Y == sector.WestGate.Y);
                        arc = new Arc(sec, w, avoidDanger ? sector.WestGate.DangerLevel : 1);
                        graph.AddArc(arc);
                    }
                    if (sector.EastGate != null)
                    {
                        Node ea = graph.Nodes.First(p => p.X == sector.EastGate.X && p.Y == sector.EastGate.Y);
                        arc = new Arc(sec, ea, avoidDanger ? sector.EastGate.DangerLevel : 1);
                        graph.AddArc(arc);
                    }
                }
                AStar a = new AStar(graph);
                Node start = graph.Nodes.First(p => p.X == 0 && p.Y == 0);

                Node end = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];


                if (start != null && end != null)
                {
                    if (a.SearchPath(start, end))
                    {
                        int count = 0;
                        foreach (Node node in a.PathByNodes)
                        {
                            int x = (int) node.X*40 - 8 + 500;
                            int y = (int) node.Y*40 - 8 + 500;
                            System.Drawing.Font f = new Font(FontFamily.GenericSerif, 20);
                            g.DrawString(count++.ToString(), f, new SolidBrush(Color.Black), x + img.Width/2,
                                y + img.Height/2);
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue)), x + 4 + img.Width/2,
                                y + 4 + img.Height/2, 8, 8);
                        }
                    }
                }

                /*List<Sector> solution = MainClass.PrintSolution(astar.Solution,universe);

            for (int i = 0; i < solution.Count; i++)
            {
                int x = solution[i].X * 40 - 8 + 500;
                int y = solution[i].Y * 40 - 8 + 500;

                g.DrawRectangle(new Pen(new SolidBrush(Color.Red)), x, y, 16, 16);
            }*/
                sw.Stop();
                TimeSpan t = sw.Elapsed;
                universePanel.Invalidate();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            });
            FileStream fs = new FileStream("image.png", FileMode.Create);
            img.Save(fs, ImageFormat.Png);
            fs.Close();
        }

        private double GetSector(PerlinNoise noise, int x, int y)
        {
            double v =
                // First octave
                (noise.Noise(2*x, 2*y, -0.5) + 1)/2*0.7 +
                // Second octave
                (noise.Noise(4*x, 4*y, 0) + 1)/2*0.2 +
                // Third octave
                (noise.Noise(8*x, 8*y, +0.5) + 1)/2*0.1;

            v = Math.Min(1, Math.Max(0, v));
            return v;
        }

        public void RandomDangerLever(Sector sector)
        {
            if (rnd.Next(100) < 20)
            {
                sector.DangerLevel = rnd.Next(100);
            }
        }

        private void universePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(img, 0, 0, 1000, 1000);
        }
    }
}
