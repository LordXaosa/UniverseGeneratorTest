
namespace Common.Models
{
    public class SectorModel : NotifyPropertyChanged
    {
        public SectorModel NorthGate { get; set; }
        public SectorModel SouthGate { get; set; }
        public SectorModel EastGate { get; set; }
        public SectorModel WestGate { get; set; }
        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }
        public Point3D Position { get; set; }
        public int DangerLevel { get; set; }
        public string Name { get; set; }
        public RaceEnum Race { get; set; }
        private bool _isRoute;
        public bool IsRoute
        {
            get { return _isRoute; }
            set
            {
                _isRoute = value;
                RaisePropertyChanged();
            }
        }

        private bool _isRevealed;
        public bool IsRevealed
        {
            get { return _isRevealed; }
            set
            {
                _isRevealed = value;
                RaisePropertyChanged();
            }
        }

        public SectorModel()
        { }
        public SectorModel(Point3D position, string name, SectorModel north = null, SectorModel south = null, SectorModel west = null, SectorModel east = null)
        {
            Position = position;
            Name = name;
            NorthGate = north;
            SouthGate = south;
            WestGate = west;
            EastGate = east;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            SectorModel s = obj as SectorModel;
            if (s == null) return false;
            return Position.Equals(s.Position);
        }
    }

}
