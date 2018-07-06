using AntMe.English;
using AntMe.Player.ArndtBalke.Markers;

namespace AntMe.Player.ArndtBalke.Map
{
    internal class Target
    {
        public Item Item { get; private set; }
        public Signal Signal { get; private set; }

        public Target(Item item)
        {
            Item = item;
        }

        public Target(Signal signal)
        {
            Signal = signal;
        }

    }
}
