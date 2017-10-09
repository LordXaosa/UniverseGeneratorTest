namespace Common
{
    public class Sector : NotifyPropertyChanged
    {
        public Sector NorthGate { get; set; }
        public Sector SouthGate { get; set; }
        public Sector EastGate { get; set; }
        public Sector WestGate { get; set; }
        //public int X, Y;
        public int X { get { return (int)Position.X; } }
        public int Y { get { return (int)Position.Y; } }
        public Point3D Position { get; set; }
        public int DangerLevel { get; set; }
        public string Name { get; set; }
        public Race Race { get; set; }

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

        public Sector(Point3D position, string name, Sector north = null, Sector south = null, Sector west = null, Sector east = null)
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
            Sector s = obj as Sector;
            if (s == null) return false;
            return Position.Equals(s.Position);
        }
    }
    public enum Race
    {
        Argon = 0,
        Paranid = 1,
        Teladi = 2,
        Split = 3,
        Boron = 4,
        None = 5,
        Xenon = 6,
        Pirate = 7
    }
}
