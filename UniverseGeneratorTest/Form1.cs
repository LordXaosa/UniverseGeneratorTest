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
using Common;

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
        //Dictionary<Point3D, Sector> universe = new Dictionary<Point3D, Sector>();
        //ConcurrentDictionary<Point3D, Sector> universe = new ConcurrentDictionary<Point3D, Sector>();

        Bitmap img;

        //bool[,] generated = new bool[100, 100];

        private bool avoidDanger = true;

        Universe universe = new Universe();
        private double chanced = 0.48d;
        Stopwatch sw = new Stopwatch();
        Random rnd;
        private async void startBt_Click(object sender, EventArgs e)
        {
            timer1.Start();
            sw.Reset();
            sw.Start();
            img = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();
            rnd = new Random(465845);
            await Task.Run(() =>
            {
                /*universe = new Universe();
                GenerateUniverse();
                
                DrawUniverse();
                FindPath();*/
                PerlinNoise pn = new PerlinNoise(8973454);
                img = new Bitmap(500, 500);
                Graphics g = Graphics.FromImage(img);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, 500, 500);
                for(int i = 0; i <500;i++)
                {
                    for(int j = 0; j < 500; j++)
                    {
                        if (GetSector(pn,i,j) > chanced)
                        {
                            img.SetPixel(i, j, Color.White);
                        }
                        else
                            img.SetPixel(i, j, Color.Black);
                    }
                }
            });
            FileStream fs = new FileStream("image.png", FileMode.Create);
            img.Save(fs, ImageFormat.Png);
            fs.Close();
            universePanel.Invalidate();
            sw.Stop();
            timer1.Stop();
        }

        private void universePanel_Paint(object sender, PaintEventArgs e)
        {
            if (img != null)
            {
                Graphics g = e.Graphics;
                var ratioX = (double)universePanel.ClientRectangle.Width / img.Width;
                var ratioY = (double)universePanel.ClientRectangle.Height / img.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(img.Width * ratio);
                var newHeight = (int)(img.Height * ratio);

                g.DrawImage(img, 0, 0, newWidth, newHeight);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeLbl.Text = sw.Elapsed.ToString();
        }

        void GenerateUniverse()
        {
            universe.GenerateUniverse(int.Parse(cyclesTb.Text), avoidDanger, chanced);
        }
        void DrawUniverse()
        {
            int minY = (int)universe.Sectors.Min(p => p.Value.Position.Y);
            int minX = (int)universe.Sectors.Min(p => p.Value.Position.X);
            int maxY = (int)universe.Sectors.Max(p => p.Value.Position.Y);
            int maxX = (int)universe.Sectors.Max(p => p.Value.Position.X);
            //img = new Bitmap(((Math.Abs(minX) + Math.Abs(maxX)) * 44), ((Math.Abs(minY) + Math.Abs(maxY)) * 44));
            img = new Bitmap(40000, 7500);
            //int offsetX = (Math.Abs(minX) - Math.Abs(maxX)) * 8;//img.Width / 2
            //int offsetY = (Math.Abs(minY) - Math.Abs(maxY)) * 8;//img.Height / 2
            int offsetX = 0;// img.Width / 2;
            int offsetY = 0;// img.Height / 2;
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Graphics g = Graphics.FromImage(img);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, img.Width, img.Height);
            foreach (KeyValuePair<Point3D, Sector> item in universe.Sectors)
            {
                int x = item.Value.X * 40 - 8 + img.Width / 2;
                int y = item.Value.Y * 40 - 8 + img.Height / 2;
                if (item.Value.X == 0 && item.Value.Y == 0)
                {
                    g.DrawRectangle(new Pen(new SolidBrush(Color.Green)), x + offsetX, y + offsetY, 16, 16);
                }
                else
                {
                    if (item.Value.DangerLevel == 0)
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Black)), x + offsetX, y + offsetY, 16,
                            16);
                    else if (item.Value.DangerLevel <= 30)
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Orange)), x + offsetX, y + offsetY, 16,
                            16);
                    else if (item.Value.DangerLevel <= 60)
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Red)), x + offsetX, y + offsetY, 16,
                            16);
                    else
                        g.DrawRectangle(new Pen(new SolidBrush(Color.Brown)), x + offsetX, y + offsetY, 16,
                            16);
                }
                if (item.Value.NorthGate != null)
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 8 + offsetX, y + offsetY,
                        x + 8 + offsetX, y - 12 + offsetY);
                }
                if (item.Value.SouthGate != null)
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 8 + offsetX, y + 16 + offsetY,
                        x + 8 + offsetX, y + 30 + offsetY);
                }
                if (item.Value.WestGate != null)
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + offsetX, y + 8 + offsetY,
                        x - 12 + offsetX, y + 8 + offsetY);
                }
                if (item.Value.EastGate != null)
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.Blue)), x + 16 + offsetX, y + 8 + offsetY,
                        x + 28 + offsetX, y + 8 + offsetY);
                }
            }
        }

        void FindPath()
        {
            Point3D startpos = new Point3D(0, 0, 0);
            Node start = universe.Graph.NodesDictionary[startpos];
            Node end = universe.Graph.Nodes[rnd.Next(0, universe.Graph.Nodes.Count)];
            Node[] path = universe.FindPath(start, end);
            if (path != null)
            {
                DrawPath(path);
            }
        }
        void DrawPath(Node[] path)
        {
            Graphics g = Graphics.FromImage(img);
            int count = 0;
            int minY = (int)universe.Sectors.Min(p => p.Value.Position.Y);
            int minX = (int)universe.Sectors.Min(p => p.Value.Position.X);
            int maxY = (int)universe.Sectors.Max(p => p.Value.Position.Y);
            int maxX = (int)universe.Sectors.Max(p => p.Value.Position.X);
            //int offsetX = (Math.Abs(minX) - Math.Abs(maxX)) * 8;//img.Width / 2
            //int offsetY = (Math.Abs(minY) - Math.Abs(maxY)) * 8;
            int offsetX = 0;// img.Width / 2;
            int offsetY = 0;// img.Height / 2;
            foreach (Node node in path)
            {
                int x = (int)node.X * 40 - 8 + img.Width / 2;
                int y = (int)node.Y * 40 - 8 + img.Height / 2;
                System.Drawing.Font f = new Font(FontFamily.GenericSerif, 20);
                g.DrawString(count++.ToString(), f, new SolidBrush(Color.Black), x + offsetX,
                    y + offsetY);
                g.DrawRectangle(new Pen(new SolidBrush(Color.Blue)), x + 4 + offsetX,
                    y + 4 + offsetY, 8, 8);
            }
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
    }
}
