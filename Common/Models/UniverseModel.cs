using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Entities.Pathfinding;

namespace Common.Models
{
    public class UniverseModel:ISerializableModel
    {
        public ConcurrentDictionary<Point2D, SectorModel> Sectors { get; set; }
        public int MaxY { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MinX { get; set; }

        public UniverseModel()
        {
            Sectors = new ConcurrentDictionary<Point2D, SectorModel>();
        }

        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(MaxX);
            bw.Write(MaxY);
            bw.Write(MinX);
            bw.Write(MinY);
            bw.Write(Sectors.Count);
            foreach (var s in Sectors)
            {
                s.Value.WriteBinary(bw);
            }
        }

        public void ReadBinary(BinaryReader br)
        {
            MaxX = br.ReadInt32();
            MaxY = br.ReadInt32();
            MinX = br.ReadInt32();
            MinY = br.ReadInt32();
            int count = br.ReadInt32();
            Sectors = new ConcurrentDictionary<Point2D, SectorModel>();
            for (int i = 0; i<count;i++)
            {
                SectorModel s = new SectorModel(br);
                Sectors.TryAdd(s.Node.Position, s);
            }
            foreach(var item in Sectors)
            {
                SetLinks(item.Value, Sectors);
            }
        }
        
        public void SetLinks(SectorModel sector, ConcurrentDictionary<Point2D, SectorModel> universe)
        {
            SectorModel other;
            if (sector.Node.NorthGatePos != null)
            {
                other = universe[sector.Node.NorthGatePos];
                sector.Node.NorthGate = other.Node;
                other.Node.SouthGate = sector.Node;
            }
            if (sector.Node.SouthGatePos != null)
            {
                other = universe[sector.Node.SouthGatePos];
                sector.Node.SouthGate = other.Node;
                other.Node.NorthGate = sector.Node;
            }
            if (sector.Node.WestGatePos != null)
            {
                other = universe[sector.Node.WestGatePos];
                sector.Node.WestGate = other.Node;
                other.Node.EastGate = sector.Node;
            }
            if (sector.Node.EastGatePos != null)
            {
                other = universe[sector.Node.EastGatePos];
                sector.Node.EastGate = other.Node;
                other.Node.WestGate = sector.Node;
            }
        }
    }
}
