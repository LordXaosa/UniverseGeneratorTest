using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UniverseModel
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
    }
}
