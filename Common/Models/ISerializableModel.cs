using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public interface ISerializableModel
    {
        void WriteBinary(BinaryWriter bw);
        void ReadBinary(BinaryReader br);
    }
}
