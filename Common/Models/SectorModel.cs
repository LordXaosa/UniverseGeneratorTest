using System.IO;
using Common.Entities.Pathfinding;

namespace Common.Models
{
    public class SectorModel : NotifyPropertyChanged, ISerializableModel
    {
        public SectorGraphNode Node { get; private set; }
        
        private bool _isRoute;
        public bool IsRoute
        {
            get => _isRoute;
            set
            {
                _isRoute = value;
                RaisePropertyChanged();
            }
        }
        public string Name { get; set; } 
        public RaceEnum Race { get; set; }
        private bool _isRevealed;
        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                _isRevealed = value;
                RaisePropertyChanged();
            }
        }

        public SectorModel(BinaryReader br)
        {
            ReadBinary(br);
        }

        public SectorModel(string name, SectorGraphNode node)
        {
            Name = name;
            Node = node;
        }
        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Name);
            bw.Write((int)Node.Position.X);
            bw.Write((int)Node.Position.Y);
            bw.Write((byte)Node.DangerLevel);
            bw.Write((byte)Race);
            int flag = 0;
            flag = Node.NorthGate != null ? 1 : 0;
            flag = flag + (Node.SouthGate != null ? 1 << 1 : 0 << 1);
            flag = flag + (Node.WestGate != null ? 1 << 2 : 0 << 2);
            flag = flag + (Node.EastGate != null ? 1 << 3 : 0 << 3);
            bw.Write((byte)flag);
            if (Node.NorthGate != null)
            {
                bw.Write((int)Node.NorthGate.Position.X);
                bw.Write((int)Node.NorthGate.Position.Y);
            }
            if (Node.SouthGate != null)
            {
                bw.Write((int)Node.SouthGate.Position.X);
                bw.Write((int)Node.SouthGate.Position.Y);
            }
            if (Node.WestGate != null)
            {
                bw.Write((int)Node.WestGate.Position.X);
                bw.Write((int)Node.WestGate.Position.Y);
            }
            if (Node.EastGate != null)
            {
                bw.Write((int)Node.EastGate.Position.X);
                bw.Write((int)Node.EastGate.Position.Y);
            }
        }

        public void ReadBinary(BinaryReader br)
        {
            Name = br.ReadString();
            int x = br.ReadInt32();
            int y = br.ReadInt32();
            Node = new SectorGraphNode(new Point2D(x, y));
            Node.DangerLevel = br.ReadByte();
            Race = (RaceEnum)br.ReadByte();
            int flag = br.ReadByte();
            if (flag % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                Node.NorthGatePos = new Point2D(x, y);
            }
            if ((flag >> 1) % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                Node.SouthGatePos = new Point2D(x, y);
            }
            if ((flag >> 2) % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                Node.WestGatePos = new Point2D(x, y);
            }
            if ((flag >> 3) % 2 == 1)
            {
                x = br.ReadInt32();
                y = br.ReadInt32();
                Node.EastGatePos = new Point2D(x, y);
            }
        }
    }
}