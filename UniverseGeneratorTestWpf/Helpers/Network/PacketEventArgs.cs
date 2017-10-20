using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniverseGeneratorTestWpf.Helpers.Network
{
    public class PacketEventArgs:EventArgs
    {
        public BinaryReader Reader;
        public PacketEventArgs(BinaryReader reader)
        {
            Reader = reader;
        }
    }
}
