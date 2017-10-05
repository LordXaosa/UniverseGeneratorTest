using Common;
using EMK.LightGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UniverseGeneratorTestWpf
{
    public class MainViewModel : NotifyPropertyChanged
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

        private List<Sector> _sectors;
        public List<Sector> Sectors
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

        public ICommand FindWay { get; set; }

        public MainViewModel()
        {
            SelectedSectors = new List<Sector>();
            universe = new Universe();
            FindWay = new Command(FindWayCmd, ()=>SelectedSectors?.Count == 2);
            GenerateUniverse();
        }
        
        async void GenerateUniverse()
        {
            await universe.GenerateUniverse(10000, true, 0.0d);
            MinX = universe.MinX;
            MinY = universe.MinY;
            MaxX = universe.MaxX;
            MaxY = universe.MaxY;
            Sectors = universe.Sectors.Values.ToList();
        }

        void FindWayCmd()
        {
            Sectors.AsParallel().ForAll(p => p.IsRoute = false);
            Point3D start = SelectedSectors[0].Position;
            Point3D end = SelectedSectors[1].Position;
            List<Sector> sectorsToHighlight = universe.FindPath(start, end);
            List<Point3D> positions = sectorsToHighlight.Select(p => p.Position).ToList();
            foreach(Point3D pos in positions)
            {
                universe.Sectors[pos].IsRoute = true;
            }
        }
    }
}
