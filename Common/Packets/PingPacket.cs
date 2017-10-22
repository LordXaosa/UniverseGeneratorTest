using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Packets
{
    public class PingPacket : IPacket
    {
        public void ReadPacket(BinaryReader br)
        {
            //br.ReadInt32();
        }

        public void WritePacket(BinaryWriter bw)
        {
            bw.Write((int)Entities.PacketsEnum.Ping);
        }
    }
}
