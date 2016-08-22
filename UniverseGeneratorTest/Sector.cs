using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniverseGeneratorTest
{
    public class Sector
    {
        public Sector NorthGate;
        public Sector SouthGate;
        public Sector EastGate;
        public Sector WestGate;
        public int X, Y;
        public int DangerLevel;
        public string Name;

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ Y.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            Sector s = obj as Sector;
            if (s == null) return false;
            return X==s.X && Y==s.Y;
        }
    }
}
