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
        public UniverseModel Universe { get; set; }
        bool _isServer;
        public GetUniversePacket()
        {
            _isServer = false;
        }
        public GetUniversePacket(UniverseModel universe)
        {
            Universe = universe;
            _isServer = true;
        }
        public void ReadPacket(BinaryReader br)
        {
            if (!_isServer)
            {
                Universe = new UniverseModel();
                Universe.ReadBinary(br);
            }
        }

        public void WritePacket(BinaryWriter bw)
        {
            bw.Write((int)PacketsEnum.GetUniverse);
            if (_isServer)
            {
                Universe.WriteBinary(bw);
            }
        }
    }
}
