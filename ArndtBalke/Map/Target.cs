using AntMe.English;
using AntMe.Player.ArndtBalke.Markers;

namespace AntMe.Player.ArndtBalke.Map
{
    internal class Target
    {
        public Item Item { get; private set; }
        public RelativeCoordinate Coordinates { get; private set; }

        public Target(Item item)
        {
            Item = item;
        }

        public Target(Signal signal)
            : this(signal.Coordinates)
        { }

        public Target(RelativeCoordinate coordinate)
        {
            Coordinates = coordinate;
        }

    }
}
