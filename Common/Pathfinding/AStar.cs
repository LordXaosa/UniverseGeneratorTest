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
using System.Collections.Concurrent;
using Common.Entities.Pathfinding;
using Common.Models;
using Priority_Queue;

namespace Common.Pathfinding
{
    /// <summary>
    /// A heuristic is a function that associates a value with a node to gauge it considering the node to reach.
    /// </summary>
    //public delegate double Heuristic(Node NodeToEvaluate, Node TargetNode);
    public delegate double Heuristic(SectorGraphNode sectorGraphToEvaluate, SectorGraphNode targetSectorGraph);

    /// <summary>
    /// Class to search the best path between two nodes on a graph.
    /// </summary>
    public class AStar
    {
        //SortableList<Track> _Open;
        private readonly FastPriorityQueue<Track> _openQueue;
        private readonly ConcurrentDictionary<Track, Track> _open;
        private readonly ConcurrentDictionary<Track, Track> _closed;
        private Track _leafToGoBackUp;

        private int _nbIterations = -1;

        public static double EuclidianDistance(SectorGraphNode s1, SectorGraphNode s2)
        {
            return Math.Sqrt(SquareEuclidianDistance(s1, s2));
        }

        public static double SquareEuclidianDistance(SectorGraphNode s1, SectorGraphNode s2)
        {
            if (s1 == null || s2 == null) throw new ArgumentNullException();
            double dx = s1.Position.X - s2.Position.X;
            double dy = s1.Position.Y - s2.Position.Y;
            return dx * dx + dy * dy;
        }
        public static double ManhattanDistance(SectorGraphNode s1, SectorGraphNode s2)
        {
            if (s1 == null || s2 == null) throw new ArgumentNullException();
            double dx = s1.Position.X - s2.Position.X;
            double dy = s1.Position.Y - s2.Position.Y;
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static double MaxDistanceAlongAxis(SectorGraphNode s1, SectorGraphNode s2)
        {
            if (s1 == null || s2 == null) throw new ArgumentNullException();
            double dx = Math.Abs(s1.Position.X - s2.Position.X);
            double dy = Math.Abs(s1.Position.Y - s2.Position.Y);
            return Math.Max(dx, dy);
        }

        /// <summary>
        /// Heuristic based on the euclidian distance : Sqrt(DxІ+DyІ+DzІ)
        /// </summary>
        public static Heuristic EuclidianHeuristic => new Heuristic(EuclidianDistance);

        /// <summary>
        /// Heuristic based on the maximum distance : Max(|Dx|, |Dy|, |Dz|)
        /// </summary>
        public static Heuristic MaxAlongAxisHeuristic => new Heuristic(MaxDistanceAlongAxis);

        /// <summary>
        /// Heuristic based on the manhattan distance : |Dx|+|Dy|+|Dz|
        /// </summary>
        public static Heuristic ManhattanHeuristic => new Heuristic(ManhattanDistance);

        /// <summary>
        /// Gets/Sets the heuristic that AStar will use.
        /// It must be homogeneous to arc's cost.
        /// </summary>
        public Heuristic ChoosenHeuristic
        {
            get => Track.ChosenHeuristic;
            set => Track.ChosenHeuristic = value;
        }

        /// <summary>
        /// This value must belong to [0; 1] and it determines the influence of the heuristic on the algorithm.
        /// If this influence value is set to 0, then the search will behave in accordance with the Dijkstra algorithm.
        /// If this value is set to 1, then the cost to come to the current node will not be used whereas only the heuristic will be taken into account.
        /// </summary>
        /// <exception cref="ArgumentException">Value must belong to [0;1].</exception>
        public double DijkstraHeuristicBalance
        {
            get => Track.DijkstraHeuristicBalance;
            set
            {
                if (value < 0 || value > 1) throw new ArgumentException("DijkstraHeuristicBalance value must belong to [0;1].");
                Track.DijkstraHeuristicBalance = value;
            }
        }

        /// <summary>
        /// AStar Constructor.
        /// </summary>
        /// <param name="sectorsCount"></param>
        public AStar(int sectorsCount)//ConcurrentDictionary<Point2D, SectorModel> sectors)
        {
            //_sectors = sectors;
            //_Open = new SortableList<Track>();
            _openQueue = new FastPriorityQueue<Track>(sectorsCount);
            _open = new ConcurrentDictionary<Track, Track>();
            _closed = new ConcurrentDictionary<Track, Track>();
            ChoosenHeuristic = EuclidianHeuristic;
            DijkstraHeuristicBalance = 0.5;
        }

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="startSectorGraph">The node from which the path must start.</param>
        /// <param name="endSectorGraph">The node to which the path must end.</param>
        /// <param name="ignoreWeight">if true will ignore danger level and search shortest</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(SectorGraphNode startSectorGraph, SectorGraphNode endSectorGraph, bool ignoreWeight)
        {
            //lock (_Graph)
            //{
            Initialize(startSectorGraph, endSectorGraph);
            while (NextStep(ignoreWeight)) { }
            return PathFound;
            //}
        }

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// Initializes AStar before performing search steps manually with NextStep.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="startSectorGraph">The node from which the path must start.</param>
        /// <param name="endSectorGraph">The node to which the path must end.</param>
        private void Initialize(SectorGraphNode startSectorGraph, SectorGraphNode endSectorGraph)
        {
            if (startSectorGraph == null || endSectorGraph == null) throw new ArgumentNullException();
            _openQueue.Clear();
            _open.Clear();
            _closed.Clear();
            Track start = new Track(startSectorGraph, endSectorGraph);
            _openQueue.Enqueue(start, (float)start.Evaluation);
            _open.TryAdd(start, start);
            _nbIterations = 0;
            _leafToGoBackUp = null;
        }

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// The algorithm must have been initialize before.
        /// </summary>
        /// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
        /// <returns>'true' unless the search ended.</returns>
        private bool NextStep(bool ignoreWeight)
        {
            if (!Initialized) throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_openQueue.Count == 0) return false;
            _nbIterations++;

            Track bestTrack = _openQueue.Dequeue();
            _open.TryRemove(bestTrack, out var s);
            if (bestTrack.Succeed)
            {
                _leafToGoBackUp = bestTrack;
                _openQueue.Clear();
                _open.Clear();
            }
            else
            {
                Propagate(bestTrack, ignoreWeight);
                _closed.TryAdd(bestTrack, bestTrack);
            }
            return _openQueue.Count > 0;
        }

        private void Propagate(Track trackToPropagate, bool ignoreWeight)
        {
            if (trackToPropagate.EndSectorGraph.NorthGate != null)
                Propagate(trackToPropagate, trackToPropagate.EndSectorGraph.NorthGate, ignoreWeight);
            if (trackToPropagate.EndSectorGraph.SouthGate != null)
                Propagate(trackToPropagate, trackToPropagate.EndSectorGraph.SouthGate, ignoreWeight);
            if (trackToPropagate.EndSectorGraph.WestGate != null)
                Propagate(trackToPropagate, trackToPropagate.EndSectorGraph.WestGate, ignoreWeight);
            if (trackToPropagate.EndSectorGraph.EastGate != null)
                Propagate(trackToPropagate, trackToPropagate.EndSectorGraph.EastGate, ignoreWeight);
        }

        private void Propagate(Track trackToPropagate, SectorGraphNode nextSectorGraph, bool ignoreWeight)
        {
            Track successor = new Track(trackToPropagate, nextSectorGraph, ignoreWeight);
            if (_open.ContainsKey(successor))
            {
                if (successor.Cost >= _open[successor].Cost) return;
            }
            if (_closed.ContainsKey(successor))
            {
                if (successor.Cost >= _closed[successor].Cost) return;
                _closed.TryRemove(successor, out var s);
            }
            if (_open.TryAdd(successor, successor))
            {
                _openQueue.Enqueue(successor, (float)successor.Evaluation);
            }
        }

        /// <summary>
        /// To know if the search has been initialized.
        /// </summary>
        public bool Initialized => _nbIterations >= 0;

        /// <summary>
        /// To know if the search has been started.
        /// </summary>
        public bool SearchStarted => _nbIterations > 0;

        /// <summary>
        /// To know if the search has ended.
        /// </summary>
        public bool SearchEnded => SearchStarted && _openQueue.Count == 0;

        /// <summary>
        /// To know if a path has been found.
        /// </summary>
        public bool PathFound => _leafToGoBackUp != null;

        /// <summary>
        /// Use for a 'step by step' search only.
        /// Gets the number of the current step.
        /// -1 if the search has not been initialized.
        /// 0 if it has not been started.
        /// </summary>
        public int StepCounter => _nbIterations;

        private void CheckSearchHasEnded()
        {
            if (!SearchEnded) throw new InvalidOperationException("You cannot get a result unless the search has ended.");
        }

        /// <summary>
        /// Returns information on the result.
        /// </summary>
        /// <param name="nbArcsOfPath">The number of arcs in the result path / -1 if no result.</param>
        /// <param name="costOfPath">The cost of the result path / -1 if no result.</param>
        /// <returns>'true' if the search succeeded / 'false' if it failed.</returns>
        public bool ResultInformation(out int nbArcsOfPath, out double costOfPath)
        {
            CheckSearchHasEnded();
            if (!PathFound)
            {
                nbArcsOfPath = -1;
                costOfPath = -1;
                return false;
            }
            else
            {
                nbArcsOfPath = _leafToGoBackUp.NbArcsVisited;
                costOfPath = _leafToGoBackUp.Cost;
                return true;
            }
        }

        /// <summary>
        /// Gets the array of nodes representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public SectorGraphNode[] PathByNodes
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                return GoBackUpNodes(_leafToGoBackUp);
            }
        }

        private SectorGraphNode[] GoBackUpNodes(Track T)
        {
            int nb = T.NbArcsVisited;
            SectorGraphNode[] path = new SectorGraphNode[nb + 1];
            for (int i = nb; i >= 0; i--, T = T.Queue)
                path[i] = T.EndSectorGraph;
            return path;
        }

        /// <summary>
        /// Gets the array of points representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public Point2D[] PathByCoordinates
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                int nb = _leafToGoBackUp.NbArcsVisited;
                Point2D[] path = new Point2D[nb + 1];
                Track cur = _leafToGoBackUp;
                for (int i = nb; i >= 0; i--, cur = cur.Queue)
                    path[i] = cur.EndSectorGraph.Position;
                return path;
            }
        }
    }
}

