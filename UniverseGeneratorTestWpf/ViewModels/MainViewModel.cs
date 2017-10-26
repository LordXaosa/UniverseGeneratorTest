using Common;
using Common.Entities;
using Common.Models;
using Common.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniverseGeneratorTestWpf.Helpers;
using UniverseGeneratorTestWpf.Helpers.Network;
using WpfCommon.Helpers;
using WpfCommon.ViewModels;

namespace UniverseGeneratorTestWpf.ViewModels
{
    public class MainViewModel : ActiveViewModel
    {
        UniverseLogic universe;
        DateTime lastPing = DateTime.Now;
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

        int _radius;
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                RaisePropertyChanged();
            }
        }

        ConcurrentQueue<Action> NetworkQueue = new ConcurrentQueue<Action>();

        public ICommand FindWay { get; set; }
        public ICommand FindRadius { get; set; }
        public ICommand GenerateMap { get; set; }
        public ICommand SerializeUniverse { get; set; }
        public ICommand DeserializeUniverse { get; set; }
        TcpClientHelper client;

        public MainViewModel()
        {
            SelectedSectors = new List<SectorModel>();
            universe = new UniverseLogic();
            FindWay = new Command(FindWayCmd, () => SelectedSectors?.Count == 2);
            GenerateMap = new Command(GenerateUniverse);
            SerializeUniverse = new Command(Serialize);
            DeserializeUniverse = new Command(Deserialize);
            FindRadius = new Command(FindRadiusCmd, () => SelectedSector != null);
            StartClient();
        }

        async void StartClient()
        {
            await Task.Factory.StartNew(() =>
            {
                client = new TcpClientHelper("localhost", 5123);
                client.PacketRecieved += Client_PacketRecieved;
                NetworkQueue.Enqueue(Login);
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        NetworkQueue.Enqueue(SendPing);
                        await Task.Delay(5000);
                    }
                });
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        NetworkQueue.TryDequeue(out Action act);
                        act?.Invoke();
                    }
                });
            });
        }

        void SendPing()
        {
            if (lastPing.AddSeconds(5) > DateTime.Now)
                return;
            lastPing = DateTime.Now;
            PingPacket p = new PingPacket();
            p.WritePacket(client.GetWriter());
        }
        void Login()
        {
            LoginPacket login = new LoginPacket("admin", "pass");
            login.WritePacket(client.GetWriter());
        }

        private void Client_PacketRecieved(object sender, PacketEventArgs args)
        {
            TcpClientHelper c = (TcpClientHelper)sender;
            switch (args.Packet)
            {
                case PingPacket p:
                    break;
                case LoginPacket lp:
                    client.IsAuth = lp.IsAuth;
                    break;
                case GetUniversePacket gup:
                    Universe = gup.Universe;
                    SetSectors();
                    OnProgress(false);
                    break;
                default:
                    MessageBox.Show("Unknown packet type recieved.");
                    break;
            }
        }

        async void GenerateUniverse()
        {
            Universe = new UniverseModel();
            OnProgress(true);
            await universe.GenerateUniverse(Universe, Cycles, true, 0.00d);
            SetSectors();
            OnProgress(false);
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
            await OnProgress(Task.Factory.StartNew(() =>
                {
                    Universe.Sectors.AsParallel().ForAll(p => p.Value.IsRoute = false);
                    List<SectorModel> sectorsToHighlight = universe.FindPath(Universe, SelectedSectors[0], SelectedSectors[1], SearchFastest);
                    sectorsToHighlight.AsParallel().ForAll(p => p.IsRoute = true);
                    PathSectorsCount = sectorsToHighlight.Count;
                })
            );
        }
        async void FindRadiusCmd()
        {
            await OnProgress(Task.Factory.StartNew(() =>
                {
                    Universe.Sectors.AsParallel().ForAll(p => p.Value.IsRoute = false);
                    List<SectorModel> sectorsToHighlight = universe.FindRadius(SelectedSector, Radius);
                    sectorsToHighlight.AsParallel().ForAll(p => p.IsRoute = true);
                    PathSectorsCount = sectorsToHighlight.Count;
                })
            );
        }

        async void Serialize()
        {
            await OnProgress(Task.Factory.StartNew(() =>
                {
                    using (BinaryWriter bw = new BinaryWriter(File.Open("Universe.dat", FileMode.Create)))
                    {
                        Universe.WriteBinary(bw);
                    }
                })
            );
        }
        void Deserialize()
        {
            NetworkQueue.Enqueue(() =>
            {
                if (client.IsAuth)
                {
                    OnProgress(true);
                    GetUniversePacket packet = new GetUniversePacket();
                    packet.WritePacket(client.GetWriter());
                }
            });
        }
    }
}
