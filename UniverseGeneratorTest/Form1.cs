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
using EMK.LightGeometry;
using System.Collections.Concurrent;

namespace UniverseGeneratorTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /*List<Sector> universe = new List<Sector>();
        HashSet<Sector> universeHashSet = new HashSet<Sector>();*/
        Dictionary<Point3D, Sector> universe = new Dictionary<Point3D, Sector>();
        //ConcurrentDictionary<Point3D, Sector> universe = new ConcurrentDictionary<Point3D, Sector>();

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
                //universeHashSet.Clear();
                //bool[,] generated = new bool[1000, 1000];
                Queue<Sector> queue = new Queue<Sector>();
                //ConcurrentQueue<Sector> queue = new ConcurrentQueue<Sector>();
                Graph graph = new Graph();
                if (universe.Count == 0)
                {
                    Point3D pos = new Point3D(0, 0, 0);
                    Sector s = new Sector(pos, ng.NextName);// {X = 0, Y = 0, Name = ng.NextName};
                    universe.Add(pos, s);
                    //universe.TryAdd(pos, s);
                    Node n = new Node(pos);
                    graph.AddNode(n);
                    //universeHashSet.Add(s);
                    queue.Enqueue(s);
                }
                int cycles = int.Parse(cyclesTb.Text);
                for (int i = 0; i < cycles; i++)
                //Parallel.For(0, cycles, (i) =>
                {
                    if (queue.Count > 0)
                    {
                        Arc arc;
                        Node n;
                        Sector sector = queue.Dequeue();
                        var v = GetSector(noise, sector.X, sector.Y - 1);
                        Node currentNode = graph.NodesDictionary[sector.Position];
                        //if (rnd.Next(100) < chance)
                        if (v > chanced)
                        {
                            Point3D npos = new Point3D(sector.X, sector.Y - 1, 0);
                            Sector north = new Sector(npos, ng.NextName, south: sector);
                            if (!universe.ContainsKey(npos))
                            {
                                RandomDangerLever(north);
                                sector.NorthGate = north;
                                queue.Enqueue(north);
                                universe.Add(npos, north);
                                n = new Node(npos);
                                graph.AddNode(n);
                                arc = new Arc(currentNode, n, avoidDanger ? sector.NorthGate.DangerLevel : 1);
                                graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                graph.AddArc(arc);
                            }
                            else
                            {
                                north = universe[npos];
                                sector.NorthGate = north;
                                north.SouthGate = sector;
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = GetSector(noise, sector.X, sector.Y + 1);
                        if (v > chanced)
                        {
                            Point3D spos = new Point3D(sector.Position.X, sector.Position.Y + 1, 0);
                            Sector south = new Sector(spos, ng.NextName, north: sector);
                            if (!universe.ContainsKey(spos))
                            {
                                RandomDangerLever(south);
                                sector.SouthGate = south;
                                queue.Enqueue(south);
                                universe.Add(spos, south);
                                n = new Node(spos);
                                graph.AddNode(n);
                                arc = new Arc(currentNode, n, avoidDanger ? sector.SouthGate.DangerLevel : 1);
                                graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                graph.AddArc(arc);
                            }
                            else
                            {
                                south = universe[spos];
                                sector.SouthGate = south;
                                south.NorthGate = sector;
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = GetSector(noise, sector.X - 1, sector.Y);
                        if (v > chanced)
                        {
                            Point3D wpos = new Point3D(sector.Position.X - 1, sector.Position.Y, 0);
                            Sector west = new Sector(wpos, ng.NextName, east: sector);
                            if (!universe.ContainsKey(wpos))
                            {
                                RandomDangerLever(west);
                                sector.WestGate = west;
                                queue.Enqueue(west);
                                universe.Add(wpos, west);
                                n = new Node(wpos);
                                graph.AddNode(n);
                                arc = new Arc(currentNode, n, avoidDanger ? sector.WestGate.DangerLevel : 1);
                                graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                graph.AddArc(arc);
                            }
                            else
                            {
                                west = universe[wpos];
                                sector.WestGate = west;
                                west.EastGate = sector;
                            }
                        }
                        //if (rnd.Next(100) < chance)
                        v = GetSector(noise, sector.X + 1, sector.Y);
                        if (v > chanced)
                        {
                            Point3D epos = new Point3D(sector.Position.X + 1, sector.Position.Y, 0);
                            Sector east = new Sector(epos, ng.NextName, west: sector);
                            if (!universe.ContainsKey(epos))
                            {
                                RandomDangerLever(east);
                                sector.EastGate = east;
                                queue.Enqueue(east);
                                universe.Add(epos, east);
                                n = new Node(epos);
                                graph.AddNode(n);
                                arc = new Arc(currentNode, n, avoidDanger ? sector.EastGate.DangerLevel : 1);
                                graph.AddArc(arc);
                                arc = new Arc(n, currentNode, avoidDanger ? sector.DangerLevel : 1);
                                graph.AddArc(arc);
                            }
                            else
                            {
                                east = universe[epos];
                                sector.EastGate = east;
                                east.WestGate = sector;
                            }
                        }
                    }
                }
                img = new Bitmap(10000, 10000);
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Graphics g = Graphics.FromImage(img);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, 10000, 10000);
                foreach (KeyValuePair<Point3D, Sector> item in universe)
                {
                    int x = item.Value.X * 40 - 8 + 500;
                    int y = item.Value.Y * 40 - 8 + 500;
                    if (item.Value.X == 0 && item.Value.Y == 0)
                    {
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Green)), x + img.Width / 2, y + img.Height / 2, 16, 16);
                    }
                    else
                    {
                        if (item.Value.DangerLevel == 0)
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), x + img.Width / 2, y + img.Height / 2, 16,
                                16);
                        else if (item.Value.DangerLevel <= 30)
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Orange)), x + img.Width / 2, y + img.Height / 2, 16,
                                16);
                        else if (item.Value.DangerLevel <= 60)
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Red)), x + img.Width / 2, y + img.Height / 2, 16,
                                16);
                        else
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Brown)), x + img.Width / 2, y + img.Height / 2, 16,
                                16);
                    }
                    if (item.Value.NorthGate != null)
                    {
                        g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 8 + img.Width / 2, y + img.Height / 2,
                            x + 8 + img.Width / 2, y - 12 + img.Height / 2);
                    }
                    if (item.Value.SouthGate != null)
                    {
                        g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 8 + img.Width / 2, y + 16 + img.Height / 2,
                            x + 8 + img.Width / 2, y + 30 + img.Height / 2);
                    }
                    if (item.Value.WestGate != null)
                    {
                        g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + img.Width / 2, y + 8 + img.Height / 2,
                            x - 12 + img.Width / 2, y + 8 + img.Height / 2);
                    }
                    if (item.Value.EastGate != null)
                    {
                        g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 16 + img.Width / 2, y + 8 + img.Height / 2,
                            x + 28 + img.Width / 2, y + 8 + img.Height / 2);
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
                            int x = (int)node.X * 40 - 8 + 500;
                            int y = (int)node.Y * 40 - 8 + 500;
                            System.Drawing.Font f = new Font(FontFamily.GenericSerif, 20);
                            g.DrawString(count++.ToString(), f, new SolidBrush(Color.Black), x + img.Width / 2,
                                y + img.Height / 2);
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue)), x + 4 + img.Width / 2,
                                y + 4 + img.Height / 2, 8, 8);
                        }
                    }
                }
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
                (noise.Noise(2 * x, 2 * y, -0.5) + 1) / 2 * 0.7 +
                // Second octave
                (noise.Noise(4 * x, 4 * y, 0) + 1) / 2 * 0.2 +
                // Third octave
                (noise.Noise(8 * x, 8 * y, +0.5) + 1) / 2 * 0.1;

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
