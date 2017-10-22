using Common.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Packets
{
    public class GetUniversePacket : IPacket
    {
        public List<SectorModel> Sectors { get; set; }
        bool _isServer;
        public GetUniversePacket()
        {
            _isServer = false;
        }
        public GetUniversePacket(List<SectorModel> sectors)
        {
            Sectors = sectors;
            _isServer = true;
        }
        public void ReadPacket(BinaryReader br)
        {
            if (!_isServer)
            {
                Sectors = new List<SectorModel>();
                int count = br.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Sectors.Add(SectorModel.Create(br));
                }
            }
        }

        public void WritePacket(BinaryWriter bw)
        {
            bw.Write((int)PacketsEnum.GetUniverse);
            if (_isServer)
            {
                if (Sectors == null)
                {
                    bw.Write(0);
                    return;
                }
                bw.Write(Sectors.Count);
                foreach (SectorModel item in Sectors)
                {
                    item.WriteBinary(bw);
                }
            }
        }
    }
}
