using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniverseGeneratorTest
{
    public static class Extensions
    {
        public static long GetObjectSize(this object TestObject)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, TestObject);
            return ms.ToArray().LongLength;
        }
    }
}
