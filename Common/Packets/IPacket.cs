using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Packets
{
    public interface IPacket
    {
        void WritePacket(BinaryWriter bw);
        void ReadPacket(BinaryReader br);
    }
}
