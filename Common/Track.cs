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
using Priority_Queue;
using System;


namespace Common
{
    /// <summary>
    /// A track is a succession of nodes which have been visited.
    /// Thus when it leads to the target node, it is easy to return the result path.
    /// These objects are contained in Open and Closed lists.
    /// </summary>
    internal class Track : FastPriorityQueueNode, IComparable<Track>
	{
        private Sector _target = null;
        private static double _Coeff = 0.5;
		private static Heuristic _ChoosenHeuristic = AStar.EuclidianHeuristic;

        public Sector Target { set { _target = value; } get { return _target; } }

        public Sector EndSector;
		public Track Queue;

		public static double DijkstraHeuristicBalance
		{
			get { return _Coeff; }
			set
			{
				if ( value<0 || value>1 ) throw new ArgumentException(
@"The coefficient which balances the respective influences of Dijkstra and the Heuristic must belong to [0; 1].
-> 0 will minimize the number of nodes explored but will not take the real cost into account.
-> 0.5 will minimize the cost without developing more nodes than necessary.
-> 1 will only consider the real cost without estimating the remaining cost.");
				_Coeff = value;
			}
		}

		public static Heuristic ChoosenHeuristic
		{
			set { _ChoosenHeuristic = value; }
			get { return _ChoosenHeuristic; }
		}

		private int _NbArcsVisited;
		public int NbArcsVisited { get { return _NbArcsVisited; } }

		private double _Cost;
		public double Cost { get { return _Cost; } }

		virtual public double Evaluation
		{
			get
			{
				return _Coeff*_Cost+(1-_Coeff)*_ChoosenHeuristic(EndSector, _target);
			}
		}
		public bool Succeed { get { return EndSector.Equals(_target); } }

		public Track(Sector currentNode, Sector targetNode)
		{
            _target = targetNode;
            if ( _target==null ) throw new InvalidOperationException("You must specify a target Node for the Track class.");
			_Cost = 0;
			_NbArcsVisited = 0;
			Queue = null;
            EndSector = currentNode;
		}

		public Track(Track PreviousTrack, Sector nextSector, bool ignoreWeight)
		{
            _target = PreviousTrack.Target;
            if (_target == null) throw new InvalidOperationException("You must specify a target Node for the Track class.");
			Queue = PreviousTrack;
			_Cost = Queue.Cost + (!ignoreWeight ? nextSector.DangerLevel: 1);
			_NbArcsVisited = Queue._NbArcsVisited + 1;
            EndSector = nextSector;
		}

		public int CompareTo(Track Objet)
		{
			Track OtherTrack = Objet;
			return Evaluation.CompareTo(OtherTrack.Evaluation);
		}

		public static bool SameEndNode(object O1, object O2)
		{
			Track P1 = (Track)O1;
			Track P2 = (Track)O2;
			return P1.EndSector.Equals(P2.EndSector);
		}

        public override bool Equals(object obj)
        {
            Track t = (Track)obj;
            return t != null && t.EndSector.Equals(EndSector);
        }

        public override int GetHashCode()
        {
            return EndSector.GetHashCode();
        }
    }
}
