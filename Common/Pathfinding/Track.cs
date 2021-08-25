// Copyright 2003 Eric Marchesin - <eric.marchesin@laposte.net>
//
// This source file(s) may be redistributed by any means PROVIDING they
// are not sold for profit without the authors expressed written consent,
// and providing that this notice and the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using Common.Models;
using Priority_Queue;

namespace Common.Pathfinding
{
    /// <summary>
    /// A track is a succession of nodes which have been visited.
    /// Thus when it leads to the target node, it is easy to return the result path.
    /// These objects are contained in Open and Closed lists.
    /// </summary>
    internal class Track : FastPriorityQueueNode, IComparable<Track>
    {
        private static double _coeff = 0.5;
        private static Heuristic _chosenHeuristic = AStar.EuclidianHeuristic;

        public SectorGraphNode Target { get; set; }

        public readonly SectorGraphNode EndSectorGraph;
        public readonly Track Queue;

        public static double DijkstraHeuristicBalance
        {
            get => _coeff;
            set
            {
                if (value < 0 || value > 1) throw new ArgumentException(
  @"The coefficient which balances the respective influences of Dijkstra and the Heuristic must belong to [0; 1].
-> 0 will minimize the number of nodes explored but will not take the real cost into account.
-> 0.5 will minimize the cost without developing more nodes than necessary.
-> 1 will only consider the real cost without estimating the remaining cost.");
                _coeff = value;
            }
        }

        public static Heuristic ChosenHeuristic
        {
            set => _chosenHeuristic = value;
            get => _chosenHeuristic;
        }

        private readonly int _nbArcsVisited;
        public int NbArcsVisited => _nbArcsVisited;

        private readonly double _cost;
        public double Cost => _cost;

        public virtual double Evaluation => _coeff * _cost + (1 - _coeff) * _chosenHeuristic(EndSectorGraph, Target);
        public bool Succeed => EndSectorGraph.Equals(Target);

        public Track(SectorGraphNode currentNode, SectorGraphNode targetNode)
        {
            Target = targetNode;
            _cost = 0;
            _nbArcsVisited = 0;
            Queue = null;
            EndSectorGraph = currentNode;
        }

        public Track(Track previousTrack, SectorGraphNode nextSectorGraph, bool ignoreWeight)
        {
            Target = previousTrack.Target;
            Queue = previousTrack;
            _cost = Queue.Cost + (!ignoreWeight ? Math.Pow(nextSectorGraph.DangerLevel, 2) : 1);
            _nbArcsVisited = Queue._nbArcsVisited + 1;
            EndSectorGraph = nextSectorGraph;
        }

        public int CompareTo(Track objet)
        {
            Track OtherTrack = objet;
            return Evaluation.CompareTo(OtherTrack.Evaluation);
        }

        public static bool SameEndNode(object o1, object o2)
        {
            Track p1 = (Track)o1;
            Track p2 = (Track)o2;
            return p1.EndSectorGraph.Equals(p2.EndSectorGraph);
        }

        public override bool Equals(object obj)
        {
            Track t = (Track)obj;
            return t != null && t.EndSectorGraph.Equals(EndSectorGraph);
        }

        public override int GetHashCode()
        {
            return EndSectorGraph.GetHashCode();
        }
    }
}
