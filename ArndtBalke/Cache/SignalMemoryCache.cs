using AntMe.Player.ArndtBalke.Markers;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class SignalMemoryCache : CacheBase
    {
        public SignalCache Fruits { get; private set; }
        public SignalCache Sugar { get; private set; }

        public SignalCache Bugs { get; private set; }
        public SignalCache Ants { get; private set; }

        public SignalMemoryCache()
        {
            Fruits = new SignalCache();
            Sugar = new SignalCache();

            Bugs = new SignalCache();
            Ants = new SignalCache();
        }

        public void Add(Signal signal)
        {
            if (signal.IsBugSpotted)
                Bugs.Add(signal);
            else if (signal.IsAntSpotted)
                Ants.Add(signal);
            else if (signal.IsSugarSpotted)
                Sugar.Add(signal);
            else if (signal.IsFruitNeedsCarriers)
                Fruits.Add(signal);
        }

        public bool Contains(Signal signal)
        {
            return Fruits.Contains(signal)
                || Sugar.Contains(signal)
                || Bugs.Contains(signal)
                || Ants.Contains(signal);
        }

        public override void Cleanup()
        {
            Fruits.Cleanup();
            Sugar.Cleanup();
            Bugs.Cleanup();
            Ants.Cleanup();
        }

    }
}
