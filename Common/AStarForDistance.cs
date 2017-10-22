using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class AStarForDistance
    {
        HashSet<SectorModel> visitedSectors;
        Queue<Track> tracks;
        public List<SectorModel> FoundSectors { get; set; }
        public AStarForDistance()
        {
            visitedSectors = new HashSet<SectorModel>();
            tracks = new Queue<Track>();
            
        }

        public void FindSectors(SectorModel start, int hops)
        {
            tracks.Enqueue(new Track(start, null));
            FoundSectors = new List<SectorModel>();
            FoundSectors.Add(start);
            while (NextStep(hops)) { }
        }
        public bool NextStep(int maxHops)
        {
            Track current = tracks.Dequeue();
            Propagate(current, maxHops);
            return tracks.Count > 0;
        }
        private void Propagate(Track TrackToPropagate, int maxHops)
        {
            if (TrackToPropagate.EndSector.NorthGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.NorthGate, maxHops);
            if (TrackToPropagate.EndSector.SouthGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.SouthGate, maxHops);
            if (TrackToPropagate.EndSector.WestGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.WestGate, maxHops);
            if (TrackToPropagate.EndSector.EastGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.EastGate, maxHops);
        }

        void Propagate(Track TrackToPropagate, SectorModel nextSector, int maxHops)
        {
            if (visitedSectors.Contains(nextSector))
                return;
            visitedSectors.Add(nextSector);
            Track Successor = new Track(TrackToPropagate, nextSector, true);
            if (Successor.NbArcsVisited > maxHops)
                return;
            tracks.Enqueue(Successor);
            FoundSectors.Add(nextSector);
        }
    }
}
