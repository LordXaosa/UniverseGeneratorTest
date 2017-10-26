using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UniverseModel:ISerializableModel
    {
        public ConcurrentDictionary<Point3D, SectorModel> Sectors { get; set; }
        public int MaxY { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MinX { get; set; }

        public UniverseModel()
        {
            Sectors = new ConcurrentDictionary<Point3D, SectorModel>();
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
            Sectors = new ConcurrentDictionary<Point3D, SectorModel>();
            for (int i = 0; i<count;i++)
            {
                SectorModel s = new SectorModel(br);
                Sectors.TryAdd(s.Position, s);
            }
            foreach(var item in Sectors)
            {
                item.Value.SetLinks(Sectors);
            }
        }
    }
}
