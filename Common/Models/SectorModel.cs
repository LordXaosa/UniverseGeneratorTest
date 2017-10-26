
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Common.Models
{
    public class SectorModel : NotifyPropertyChanged, ISerializableModel
    {
        public SectorModel NorthGate { get; set; }
        public SectorModel SouthGate { get; set; }
        public SectorModel EastGate { get; set; }
        public SectorModel WestGate { get; set; }
        private Point3D _northGatePos;
        private Point3D _southGatePos;
        private Point3D _westGatePos;
        private Point3D _eastGatePos;
        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }
        public Point3D Position { get; set; }
        public int DangerLevel { get; set; }
        public string Name { get; set; }
        public RaceEnum Race { get; set; }
        private bool _isRoute;
        public bool IsRoute
        {
            get { return _isRoute; }
            set
            {
                _isRoute = value;
                RaisePropertyChanged();
            }
        }

        private bool _isRevealed;
        public bool IsRevealed
        {
            get { return _isRevealed; }
            set
            {
                _isRevealed = value;
                RaisePropertyChanged();
            }
        }

        public SectorModel()
        { }
        public SectorModel(BinaryReader br)
        {
            ReadBinary(br);
        }
        public SectorModel(Point3D position, string name, SectorModel north = null, SectorModel south = null, SectorModel west = null, SectorModel east = null)
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
            SectorModel s = obj as SectorModel;
            if (s == null) return false;
            return Position.Equals(s.Position);
        }

        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Name);
            bw.Write((byte)DangerLevel);
            bw.Write((byte)Race);
            bw.Write((int)Position.X);
            bw.Write((int)Position.Y);
            int flag = 0;
            flag = NorthGate != null ? 1 : 0;
            flag = flag + (SouthGate != null ? 1 << 1 : 0 << 1);
            flag = flag + (WestGate != null ? 1 << 2 : 0 << 2);
            flag = flag + (EastGate != null ? 1 << 3 : 0 << 3);
            bw.Write((byte)flag);
            if (NorthGate != null)
            {
                bw.Write((int)NorthGate.Position.X);
                bw.Write((int)NorthGate.Position.Y);
            }
            if (SouthGate != null)
            {
                bw.Write((int)SouthGate.Position.X);
                bw.Write((int)SouthGate.Position.Y);
            }
            if (WestGate != null)
            {
                bw.Write((int)WestGate.Position.X);
                bw.Write((int)WestGate.Position.Y);
            }
            if (EastGate != null)
            {
                bw.Write((int)EastGate.Position.X);
                bw.Write((int)EastGate.Position.Y);
            }
        }

        public void ReadBinary(BinaryReader br)
        {
            Name = br.ReadString();
            DangerLevel = br.ReadByte();
            Race = (RaceEnum)br.ReadByte();
            int x = br.ReadInt32();
            int y = br.ReadInt32();
            Position = new Point3D(x, y, 0);
            int flag = br.ReadByte();
            if (flag % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                _northGatePos = new Point3D(x, y, 0);
            }
            if ((flag >> 1) % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                _southGatePos = new Point3D(x, y, 0);
            }
            if ((flag >> 2) % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                _westGatePos = new Point3D(x, y, 0);
            }
            if ((flag >> 3) % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                _eastGatePos = new Point3D(x, y, 0);
            }
        }

        public void SetLinks(ConcurrentDictionary<Point3D, SectorModel> universe)
        {
            SectorModel other;
            if (_northGatePos != null)
            {
                other = universe[_northGatePos];
                NorthGate = other;
                other.SouthGate = this;
            }
            if (_southGatePos != null)
            {
                other = universe[_southGatePos];
                SouthGate = other;
                other.NorthGate = this;
            }
            if (_westGatePos != null)
            {
                other = universe[_westGatePos];
                WestGate = other;
                other.EastGate = this;
            }
            if (_eastGatePos != null)
            {
                other = universe[_eastGatePos];
                EastGate = other;
                other.WestGate = this;
            }
        }
    }

}
