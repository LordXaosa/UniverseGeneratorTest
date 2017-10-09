﻿using Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using UniverseGeneratorTestWpf.Helpers;

namespace UniverseGeneratorTestWpf.ViewModels
{
    public class MainViewModel : UserWaitableViewModel
    {
        Universe universe;
        int _minX;
        public int MinX
        {
            get { return _minX; }
            set
            {
                _minX = value;
                RaisePropertyChanged();
            }
        }
        int _minY;
        public int MinY
        {
            get { return _minY; }
            set
            {
                _minY = value;
                RaisePropertyChanged();
            }
        }
        int _maxX;
        public int MaxX
        {
            get { return _maxX; }
            set
            {
                _maxX = value;
                RaisePropertyChanged();
            }
        }
        int _maxY;
        public int MaxY
        {
            get { return _maxY; }
            set
            {
                _maxY = value;
                RaisePropertyChanged();
            }
        }

        int _totalX;
        public int TotalX
        {
            get { return _totalX; }
            set
            {
                _totalX = value;
                RaisePropertyChanged();
            }
        }

        int _totalY;
        public int TotalY
        {
            get { return _totalY; }
            set
            {
                _totalY = value;
                RaisePropertyChanged();
            }
        }

        bool _searchFastest;
        public bool SearchFastest
        {
            get { return _searchFastest; }
            set
            {
                _searchFastest = value;
                RaisePropertyChanged();
            }
        }

        private GridLikeItemsSource _sectors;
        public GridLikeItemsSource Sectors
        {
            get { return _sectors; }
            set
            {
                _sectors = value;
                RaisePropertyChanged();
            }
        }

        private Sector _selectedSector;
        public Sector SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                _selectedSector = value;
                RaisePropertyChanged();
            }
        }

        private List<Sector> _selectedSectors;
        public List<Sector> SelectedSectors
        {
            get { return _selectedSectors; }
            set
            {
                _selectedSectors = value;
                RaisePropertyChanged();
            }
        }

        private int _pathSectorsCount;
        public int PathSectorsCount
        {
            get { return _pathSectorsCount; }
            set
            {
                _pathSectorsCount = value;
                RaisePropertyChanged();
            }
        }

        private int _cycles;
        public int Cycles
        {
            get { return _cycles; }
            set
            {
                _cycles = value;
                RaisePropertyChanged();
            }
        }

        public ICommand FindWay { get; set; }
        public ICommand GenerateMap { get; set; }

        public MainViewModel()
        {
            SelectedSectors = new List<Sector>();
            universe = new Universe();
            FindWay = new Command(FindWayCmd, ()=>SelectedSectors?.Count == 2);
            GenerateMap = new Command(GenerateUniverse);
            //GenerateUniverse();
        }
        
        async void GenerateUniverse()
        {
            IsInProgress = true;
            await universe.GenerateUniverse(Cycles, true, 0.00d);
            MinX = universe.MinX;
            MinY = universe.MinY;
            MaxX = universe.MaxX;
            MaxY = universe.MaxY;
            TotalX = -MinX + MaxX + 1;
            TotalY = -MinY + MaxY + 1;
            Sectors = new GridLikeItemsSource(universe.Sectors.Values.ToList(), MinX, MinY, MaxX, MaxY, 50);
            Sectors.Count = universe.Sectors.Count;
            IsInProgress = false;
        }

        void FindWayCmd()
        {
            universe.Sectors.AsParallel().ForAll(p => p.Value.IsRoute = false);
            List<Sector> sectorsToHighlight = universe.FindPath(SelectedSectors[0], SelectedSectors[1], SearchFastest);
            sectorsToHighlight.AsParallel().ForAll(p => p.IsRoute = true);
            PathSectorsCount = sectorsToHighlight.Count;
        }
    }
}
