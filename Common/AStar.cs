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
using Common.Models;
using Priority_Queue;
using System;
using System.Collections.Concurrent;

namespace Common
{
    /// <summary>
    /// A heuristic is a function that associates a value with a node to gauge it considering the node to reach.
    /// </summary>
    //public delegate double Heuristic(Node NodeToEvaluate, Node TargetNode);
    public delegate double Heuristic(SectorModel SectorToEvaluate, SectorModel TargetSector);

    /// <summary>
    /// Class to search the best path between two nodes on a graph.
    /// </summary>
    public class AStar
    {
        //SortableList<Track> _Open;
        FastPriorityQueue<Track> _Open;
        ConcurrentDictionary<Track, Track> _open, _closed;
        Track _LeafToGoBackUp;
        int _NbIterations = -1;
        //ConcurrentDictionary<Point3D, SectorModel> _sectors;

        public static double EuclidianDistance(SectorModel S1, SectorModel S2)
        {
            return Math.Sqrt(SquareEuclidianDistance(S1, S2));
        }

        public static double SquareEuclidianDistance(SectorModel S1, SectorModel S2)
        {
            if (S1 == null || S2 == null) throw new ArgumentNullException();
            double DX = S1.Position.X - S2.Position.X;
            double DY = S1.Position.Y - S2.Position.Y;
            double DZ = S1.Position.Z - S2.Position.Z;
            return DX * DX + DY * DY + DZ * DZ;
        }
        public static double ManhattanDistance(SectorModel S1, SectorModel S2)
        {
            if (S1 == null || S2 == null) throw new ArgumentNullException();
            double DX = S1.Position.X - S2.Position.X;
            double DY = S1.Position.Y - S2.Position.Y;
            double DZ = S1.Position.Z - S2.Position.Z;
            return Math.Abs(DX) + Math.Abs(DY) + Math.Abs(DZ);
        }

        public static double MaxDistanceAlongAxis(SectorModel S1, SectorModel S2)
        {
            if (S1 == null || S2 == null) throw new ArgumentNullException();
            double DX = Math.Abs(S1.Position.X - S2.Position.X);
            double DY = Math.Abs(S1.Position.Y - S2.Position.Y);
            double DZ = Math.Abs(S1.Position.Z - S2.Position.Z);
            return Math.Max(DX, Math.Max(DY, DZ));
        }

        /// <summary>
        /// Heuristic based on the euclidian distance : Sqrt(Dx²+Dy²+Dz²)
        /// </summary>
        public static Heuristic EuclidianHeuristic
        { get { return new Heuristic(EuclidianDistance); } }

        /// <summary>
        /// Heuristic based on the maximum distance : Max(|Dx|, |Dy|, |Dz|)
        /// </summary>
        public static Heuristic MaxAlongAxisHeuristic
        { get { return new Heuristic(MaxDistanceAlongAxis); } }

        /// <summary>
        /// Heuristic based on the manhattan distance : |Dx|+|Dy|+|Dz|
        /// </summary>
        public static Heuristic ManhattanHeuristic
        { get { return new Heuristic(ManhattanDistance); } }

        /// <summary>
        /// Gets/Sets the heuristic that AStar will use.
        /// It must be homogeneous to arc's cost.
        /// </summary>
        public Heuristic ChoosenHeuristic
        {
            get { return Track.ChoosenHeuristic; }
            set { Track.ChoosenHeuristic = value; }
        }

        /// <summary>
        /// This value must belong to [0; 1] and it determines the influence of the heuristic on the algorithm.
        /// If this influence value is set to 0, then the search will behave in accordance with the Dijkstra algorithm.
        /// If this value is set to 1, then the cost to come to the current node will not be used whereas only the heuristic will be taken into account.
        /// </summary>
        /// <exception cref="ArgumentException">Value must belong to [0;1].</exception>
        public double DijkstraHeuristicBalance
        {
            get { return Track.DijkstraHeuristicBalance; }
            set
            {
                if (value < 0 || value > 1) throw new ArgumentException("DijkstraHeuristicBalance value must belong to [0;1].");
                Track.DijkstraHeuristicBalance = value;
            }
        }

        /// <summary>
        /// AStar Constructor.
        /// </summary>
        /// <param name="G">The graph on which AStar will perform the search.</param>
        public AStar(int sectorsCount)//ConcurrentDictionary<Point3D, SectorModel> sectors)
        {
            //_sectors = sectors;
            //_Open = new SortableList<Track>();
            _Open = new FastPriorityQueue<Track>(sectorsCount);
            _open = new ConcurrentDictionary<Track, Track>();
            _closed = new ConcurrentDictionary<Track, Track>();
            ChoosenHeuristic = EuclidianHeuristic;
            DijkstraHeuristicBalance = 0.5;
        }

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(SectorModel startSector, SectorModel endSector, bool ignoreWeight)
        {
            //lock (_Graph)
            //{
            Initialize(startSector, endSector);
            while (NextStep(ignoreWeight)) { }
            return PathFound;
            //}
        }

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// Initializes AStar before performing search steps manually with NextStep.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="StartNode">The node from which the path must start.</param>
        /// <param name="EndNode">The node to which the path must end.</param>
        public void Initialize(SectorModel startSector, SectorModel endSector)
        {
            if (startSector == null || endSector == null) throw new ArgumentNullException();
            //_Closed.Clear();
            _Open.Clear();
            _open.Clear();
            _closed.Clear();
            //Track.Target = EndNode;
            Track start = new Track(startSector, endSector);
            _Open.Enqueue(start, (float)start.Evaluation);
            _open.TryAdd(start, start);
            _NbIterations = 0;
            _LeafToGoBackUp = null;
        }

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// The algorithm must have been initialize before.
        /// </summary>
        /// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
        /// <returns>'true' unless the search ended.</returns>
        public bool NextStep(bool ignoreWeight)
        {
            if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_Open.Count == 0) return false;
            _NbIterations++;

            //int IndexMin = _Open.IndexOfMin();
            Track BestTrack = _Open.Dequeue();
            Track s;
            //_Open.RemoveAt(IndexMin);
            _open.TryRemove(BestTrack, out s);
            if (BestTrack.Succeed)
            {
                _LeafToGoBackUp = BestTrack;
                _Open.Clear();
                _open.Clear();
            }
            else
            {
                Propagate(BestTrack, ignoreWeight);
                _closed.TryAdd(BestTrack, BestTrack);
            }
            return _Open.Count > 0;
        }

        private void Propagate(Track TrackToPropagate, bool ignoreWeight)
        {
            if (TrackToPropagate.EndSector.NorthGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.NorthGate, ignoreWeight);
            if (TrackToPropagate.EndSector.SouthGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.SouthGate, ignoreWeight);
            if (TrackToPropagate.EndSector.WestGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.WestGate, ignoreWeight);
            if (TrackToPropagate.EndSector.EastGate != null)
                Propagate(TrackToPropagate, TrackToPropagate.EndSector.EastGate, ignoreWeight);
        }

        void Propagate(Track TrackToPropagate, SectorModel nextSector, bool ignoreWeight)
        {
            Track Successor = new Track(TrackToPropagate, nextSector, ignoreWeight);
            Track s;
            if (_open.ContainsKey(Successor))
            {
                if (Successor.Cost >= _open[Successor].Cost) return;
                /*int PosNO = _Open.IndexOf(Successor, SameNodesReached);
                if (PosNO > 0)
                {
                    _Open.RemoveAt(PosNO);
                    _open.TryRemove(Successor, out s);
                }*/
            }
            if (_closed.ContainsKey(Successor))
            {
                if (Successor.Cost >= _closed[Successor].Cost) return;
                _closed.TryRemove(Successor, out s);
            }
            if (_open.TryAdd(Successor, Successor))
            {
                _Open.Enqueue(Successor, (float)Successor.Evaluation);
            }
        }

        /// <summary>
        /// To know if the search has been initialized.
        /// </summary>
        public bool Initialized { get { return _NbIterations >= 0; } }

        /// <summary>
        /// To know if the search has been started.
        /// </summary>
        public bool SearchStarted { get { return _NbIterations > 0; } }

        /// <summary>
        /// To know if the search has ended.
        /// </summary>
        public bool SearchEnded { get { return SearchStarted && _Open.Count == 0; } }

        /// <summary>
        /// To know if a path has been found.
        /// </summary>
        public bool PathFound { get { return _LeafToGoBackUp != null; } }

        /// <summary>
        /// Use for a 'step by step' search only.
        /// Gets the number of the current step.
        /// -1 if the search has not been initialized.
        /// 0 if it has not been started.
        /// </summary>
        public int StepCounter { get { return _NbIterations; } }

        private void CheckSearchHasEnded()
        {
            if (!SearchEnded) throw new InvalidOperationException("You cannot get a result unless the search has ended.");
        }

        /// <summary>
        /// Returns information on the result.
        /// </summary>
        /// <param name="NbArcsOfPath">The number of arcs in the result path / -1 if no result.</param>
        /// <param name="CostOfPath">The cost of the result path / -1 if no result.</param>
        /// <returns>'true' if the search succeeded / 'false' if it failed.</returns>
        public bool ResultInformation(out int NbArcsOfPath, out double CostOfPath)
        {
            CheckSearchHasEnded();
            if (!PathFound)
            {
                NbArcsOfPath = -1;
                CostOfPath = -1;
                return false;
            }
            else
            {
                NbArcsOfPath = _LeafToGoBackUp.NbArcsVisited;
                CostOfPath = _LeafToGoBackUp.Cost;
                return true;
            }
        }

        /// <summary>
        /// Gets the array of nodes representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public SectorModel[] PathByNodes
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                return GoBackUpNodes(_LeafToGoBackUp);
            }
        }

        private SectorModel[] GoBackUpNodes(Track T)
        {
            int Nb = T.NbArcsVisited;
            SectorModel[] Path = new SectorModel[Nb + 1];
            for (int i = Nb; i >= 0; i--, T = T.Queue)
                Path[i] = T.EndSector;
            return Path;
        }

        /// <summary>
        /// Gets the array of points representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public Point3D[] PathByCoordinates
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                int Nb = _LeafToGoBackUp.NbArcsVisited;
                Point3D[] Path = new Point3D[Nb + 1];
                Track Cur = _LeafToGoBackUp;
                for (int i = Nb; i >= 0; i--, Cur = Cur.Queue)
                    Path[i] = Cur.EndSector.Position;
                return Path;
            }
        }
    }
}

