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
using LibNoise;
using System.Runtime.InteropServices;
using UniverseGeneratorTest;

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
        //private double chanced = 0.34d;
        private double chanced = 0.0d;
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
                universe = new Universe();
                GenerateUniverse();

                DrawUniverse();
                //DrawControls();
                FindPath();

                //PerlinNoise pn = new PerlinNoise(8973454);
                //FastBillow noise = new FastBillow(8973454);
                //Perlin noise = new Perlin();
                /*Voronoi noise = new Voronoi();
                noise.Seed = 8973454;*/
                /*noise.NoiseQuality = NoiseQuality.High;
                noise.OctaveCount = 8;
                noise.Persistence = 1.4;
                noise.Lacunarity = 0.35;*/
                /*noise.Frequency = 0.015;
                img = new Bitmap(1000, 1000);
                Graphics g = Graphics.FromImage(img);
                g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 1000, 1000);
                for(int i = 0; i <1000;i++)
                {
                    for(int j = 0; j < 1000; j++)
                    {
                        //if (GetSector(pn,i,j) > chanced)
                        //if (LocationValue(i,j) > chanced)
                        //if(getXYNoise(i,j)!=0)
                        /*double value = noise.GetValue(i, 0, j);
                        value = System.Math.Min(value, 1.0d);
                        value = System.Math.Max(value, 0.0d);*/

                //Color c = Color.FromArgb((int)(value*255), Color.Black);
                //img.SetPixel(i, j, c);
                /*double v = (noise.GetValue(i, 0, j)+1)/2.0;

                //if (v>chanced)
                {
                    //img.SetPixel(i, j, Color.White);
                    if(v<0)
                    {
                        g.FillRectangle(new SolidBrush(Color.Black), i, j, 1, 1);
                    }
                    else if (v < 0.2)
                    {
                        g.FillRectangle(new SolidBrush(Color.DarkBlue), i, j, 1, 1);
                    }
                    else if(v < 0.4)
                    {
                        g.FillRectangle(new SolidBrush(Color.Blue), i, j, 1, 1);
                    }
                    else if (v < 0.6)
                    {
                        g.FillRectangle(new SolidBrush(Color.LightBlue), i, j, 1, 1);
                    }
                    else if (v < 0.8)
                    {
                        g.FillRectangle(new SolidBrush(Color.SandyBrown), i, j, 1, 1);
                    }
                    else if (v < 1.0)
                    {
                        g.FillRectangle(new SolidBrush(Color.Yellow), i, j, 1, 1);
                    }
                    else if (v < 1.2)
                    {
                        g.FillRectangle(new SolidBrush(Color.LightGreen), i, j, 1, 1);
                    }
                    else if (v < 1.4)
                    {
                        g.FillRectangle(new SolidBrush(Color.Green), i, j, 1, 1);
                    }
                    else if (v < 1.6)
                    {
                        g.FillRectangle(new SolidBrush(Color.DarkGreen), i, j, 1, 1);
                    }
                    else if (v < 1.8)
                    {
                        g.FillRectangle(new SolidBrush(Color.Brown), i, j, 1, 1);
                    }
                    else if (v>=1.8)
                    {
                        g.FillRectangle(new SolidBrush(Color.White), i, j, 1, 1);
                    }

                }
                /*else
                    img.SetPixel(i, j, Color.Black);*/
                //}
                //}
            });
            if (img != null)
            {
                FileStream fs = new FileStream("image.png", FileMode.Create);
                img.Save(fs, ImageFormat.Png);
                fs.Close();
            }
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
                var ratio = System.Math.Min(ratioX, ratioY);

                var newWidth = (int)(img.Width * ratio);
                var newHeight = (int)(img.Height * ratio);

                g.DrawImage(img, 0, 0, newWidth, newHeight);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeLbl.Text = sw.Elapsed.ToString() + " " + universe.Sectors.Count;
        }

        void GenerateUniverse()
        {
            universe.GenerateUniverse(int.Parse(cyclesTb.Text), avoidDanger, chanced);
        }

        void DrawControls()
        {
            int minY = (int)universe.Sectors.Min(p => p.Value.Position.Y);
            int minX = (int)universe.Sectors.Min(p => p.Value.Position.X);
            int maxY = (int)universe.Sectors.Max(p => p.Value.Position.Y);
            int maxX = (int)universe.Sectors.Max(p => p.Value.Position.X);
            int offsetX = System.Math.Abs(minX);
            int offsetY = System.Math.Abs(minY);
            universePanel.Controls.Clear();
            foreach (KeyValuePair<Point3D, Sector> item in universe.Sectors)
            {
                Panel panel = new Panel();
                panel.BorderStyle = BorderStyle.Fixed3D;
                if (item.Value.DangerLevel == 0)
                    panel.BackColor = Color.Black;
                else if (item.Value.DangerLevel <= 30)
                    panel.BackColor = Color.Orange;
                else if (item.Value.DangerLevel <= 60)
                    panel.BackColor = Color.Red;
                else
                    panel.BackColor = Color.Brown;
                panel.Top = (int)(item.Key.Y + offsetY) * 16 - universePanel.VerticalScroll.Value;
                panel.Left = (int)(item.Key.X + offsetX) * 16 - universePanel.HorizontalScroll.Value;
                panel.Width = 8;
                panel.Height = 8;
                this.Invoke(new Action(() =>
                {
                    universePanel.Controls.Add(panel);
                }));
            }

        }
        void DrawUniverse()
        {
            int minY = (int)universe.Sectors.Min(p => p.Value.Position.Y);
            int minX = (int)universe.Sectors.Min(p => p.Value.Position.X);
            int maxY = (int)universe.Sectors.Max(p => p.Value.Position.Y);
            int maxX = (int)universe.Sectors.Max(p => p.Value.Position.X);
            //img = new Bitmap(((Math.Abs(minX) + Math.Abs(maxX)) * 44), ((Math.Abs(minY) + Math.Abs(maxY)) * 44));
            //img = new Bitmap(40000, 7500);
            img = new Bitmap(15000, 15000);
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
                Sector s = item.Value;
                if (s.Race == Race.Argon)
                {
                    g.FillRectangle(new SolidBrush(Color.Blue), x + offsetX + 5, y + offsetY + 5, 7, 7);
                }
                else if (s.Race == Race.Paranid)
                {
                    g.FillRectangle(new SolidBrush(Color.DarkRed), x + offsetX + 5, y + offsetY + 5, 7, 7);
                }
                else if (s.Race == Race.Teladi)
                {
                    g.FillRectangle(new SolidBrush(Color.DarkGreen), x + offsetX + 5, y + offsetY + 5, 7, 7);
                }
                else if (s.Race == Race.Boron)
                {
                    g.FillRectangle(new SolidBrush(Color.Navy), x + offsetX + 5, y + offsetY + 5, 7, 7);
                }
                else if (s.Race == Race.Split)
                {
                    g.FillRectangle(new SolidBrush(Color.Gold), x + offsetX + 5, y + offsetY + 5, 7, 7);
                }
                else if (s.Race == Race.Pirate)
                {
                    g.FillRectangle(new SolidBrush(Color.Red), x + offsetX + 5, y + offsetY + 5, 7, 7);
                }
                else if (s.Race == Race.None)
                {
                    g.FillRectangle(new SolidBrush(Color.Violet), x + offsetX + 5, y + offsetY + 5, 7, 7);
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
            if (path != null && img != null)
            {
                DrawPath(path);
            }
        }
        void DrawPath(Node[] path)
        {
            Graphics g = Graphics.FromImage(img);
            int count = 0;
            int minY = universe.MinY;
            int minX = universe.MinX;
            int maxY = universe.MaxY;
            int maxX = universe.MaxX;
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

            v = System.Math.Min(1, System.Math.Max(0, v));
            return v;
        }
    }
}
