using System.Collections.Generic;
using Common.Models;

namespace Common.Pathfinding
{
    public class AStarForDistance
    {
        HashSet<SectorGraphNode> visitedSectors;
        Queue<Track> tracks;
        public List<SectorGraphNode> FoundSectors { get; set; }
        public AStarForDistance()
        {
            visitedSectors = new HashSet<SectorGraphNode>();
            tracks = new Queue<Track>();
            
        }

        public void FindSectors(SectorGraphNode start, int hops)
        {
            tracks.Enqueue(new Track(start, null));
            FoundSectors = new List<SectorGraphNode>();
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
            if (TrackToPropagate.EndSectorGraph.NorthGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSectorGraph.NorthGate, maxHops);
            if (TrackToPropagate.EndSectorGraph.SouthGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSectorGraph.SouthGate, maxHops);
            if (TrackToPropagate.EndSectorGraph.WestGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSectorGraph.WestGate, maxHops);
            if (TrackToPropagate.EndSectorGraph.EastGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSectorGraph.EastGate, maxHops);
        }

        void Propagate(Track TrackToPropagate, SectorGraphNode nextSectorGraph, int maxHops)
        {
            if (visitedSectors.Contains(nextSectorGraph))
                return;
            visitedSectors.Add(nextSectorGraph);
            Track Successor = new Track(TrackToPropagate, nextSectorGraph, true);
            if (Successor.NbArcsVisited > maxHops)
                return;
            tracks.Enqueue(Successor);
            FoundSectors.Add(nextSectorGraph);
        }
    }
}
