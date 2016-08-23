using EMK.LightGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Sector
    {
        public Sector NorthGate { get; set; }
        public Sector SouthGate { get; set; }
        public Sector EastGate { get; set; }
        public Sector WestGate { get; set; }
        //public int X, Y;
        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }
        public Point3D Position { get; set; }
        public int DangerLevel { get; set; }
        public string Name { get; set; }

        public Sector(Point3D position, string name, Sector north=null,Sector south=null, Sector west=null, Sector east=null)
        {
            Position = position;
            Name = name;
            NorthGate = north;
            SouthGate = south;
            WestGate = west;
            EastGate = east;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Sector s = obj as Sector;
            if (s == null) return false;
            return this.Position.Equals(s.Position);
        }
    }
}
