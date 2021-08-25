
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Common.Entities.Pathfinding;

namespace Common.Models
{
    public class SectorGraphNode : NotifyPropertyChanged
    {
        public SectorGraphNode NorthGate { get; set; }
        public SectorGraphNode SouthGate { get; set; }
        public SectorGraphNode EastGate { get; set; }
        public SectorGraphNode WestGate { get; set; }
        internal Point2D NorthGatePos;
        internal Point2D SouthGatePos;
        internal Point2D WestGatePos;
        internal Point2D EastGatePos;
        public int X => (int)Position.X;
        public int Y => (int)Position.Y;
        public Point2D Position { get; }
        public int DangerLevel { get; set; }
        
        

        

        public SectorGraphNode(Point2D position, SectorGraphNode north = null, SectorGraphNode south = null, SectorGraphNode west = null, SectorGraphNode east = null)
        {
            Position = position;
            NorthGate = north;
            SouthGate = south;
            WestGate = west;
            EastGate = east;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            SectorGraphNode s = obj as SectorGraphNode;
            if (s == null) return false;
            return Position.Equals(s.Position);
        }

        
    }

}
