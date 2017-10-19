using Common;
using Common.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Input;
using UniverseGeneratorTestWpf.Helpers;

namespace UniverseGeneratorTestWpf.ViewModels
{
    public class MainViewModel : UserWaitableViewModel
    {
        UniverseLogic universe;
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

        private SectorModel _selectedSector;
        public SectorModel SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                _selectedSector = value;
                RaisePropertyChanged();
            }
        }

        private List<SectorModel> _selectedSectors;
        public List<SectorModel> SelectedSectors
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

        private UniverseModel _universe;
        public UniverseModel Universe
        {
            get { return _universe; }
            set
            {
                _universe = value;
                RaisePropertyChanged();
            }
        }

        public ICommand FindWay { get; set; }
        public ICommand GenerateMap { get; set; }
        public ICommand SerializeUniverse { get; set; }
        public ICommand DeserializeUniverse { get; set; }

        public MainViewModel()
        {
            SelectedSectors = new List<SectorModel>();
            universe = new UniverseLogic();
            FindWay = new Command(FindWayCmd, () => SelectedSectors?.Count == 2);
            GenerateMap = new Command(GenerateUniverse);
            SerializeUniverse = new Command(Serialize);
            DeserializeUniverse = new Command(Deserialize);
        }

        async void GenerateUniverse()
        {
            Universe = new UniverseModel();
            IsInProgress = true;
            await universe.GenerateUniverse(Universe, Cycles, true, 0.00d);
            SetSectors();
            IsInProgress = false;
        }

        void SetSectors()
        {
            MinX = Universe.MinX;
            MinY = Universe.MinY;
            MaxX = Universe.MaxX;
            MaxY = Universe.MaxY;
            TotalX = -MinX + MaxX + 1;
            TotalY = -MinY + MaxY + 1;
            Sectors = new GridLikeItemsSource(Universe.Sectors.Values.ToList(), MinX, MinY, MaxX, MaxY, 50);
            Sectors.Count = Universe.Sectors.Count;
        }

        async void FindWayCmd()
        {
            IsInProgress = true;
            await Task.Factory.StartNew(() =>
            {
                Universe.Sectors.AsParallel().ForAll(p => p.Value.IsRoute = false);
                List<SectorModel> sectorsToHighlight = universe.FindPath(Universe, SelectedSectors[0], SelectedSectors[1], SearchFastest);
                sectorsToHighlight.AsParallel().ForAll(p => p.IsRoute = true);
                PathSectorsCount = sectorsToHighlight.Count;
            });
            IsInProgress = false;
        }

        async void Serialize()
        {
            IsInProgress = true;
            await Task.Factory.StartNew(() =>
            {
                List<SectorModel> list = Universe.Sectors.Values.ToList();
                using (BinaryWriter bw = new BinaryWriter(File.Open("Universe.dat", FileMode.Create)))
                {
                    bw.Write(list.Count);
                    foreach (var s in list)
                    {
                        s.WriteBinary(bw);
                    }
                }
            });
            IsInProgress = false;
        }
        async void Deserialize()
        {
            IsInProgress = true;
            await Task.Factory.StartNew(() =>
            {
                Universe = new UniverseModel();
                List<SectorModel> list = new List<SectorModel>();
                using (BinaryReader br = new BinaryReader(File.Open("Universe.dat", FileMode.Open)))
                {
                    int count = br.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(SectorModel.Create(br));
                    }
                }
                universe.MakeUniverseFromList(Universe, list);
                Parallel.ForEach(list, (item) =>
                {
                    item.SetLinks(Universe.Sectors);
                    if (item.Position.X > Universe.MaxX)
                        Universe.MaxX = (int)item.Position.X;
                    if (item.Position.Y > Universe.MaxY)
                        Universe.MaxY = (int)item.Position.Y;
                    if (item.Position.X < Universe.MinX)
                        Universe.MinX = (int)item.Position.X;
                    if (item.Position.Y < Universe.MinY)
                        Universe.MinY = (int)item.Position.Y;
                });
            });
            SetSectors();
            IsInProgress = false;
        }
    }
}
