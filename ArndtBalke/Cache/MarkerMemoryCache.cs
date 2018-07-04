using AntMe.Player.ArndtBalke.Behavior;
using AntMe.Player.ArndtBalke.MarkerInfo;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class MarkerMemoryCache : CacheBase
    {
        public MarkerCache Fruits { get; private set; }
        public MarkerCache Sugar { get; private set; }

        public MarkerCache Bugs { get; private set; }
        public MarkerCache Ants { get; private set; }

        public MarkerMemoryCache(BaseBehavior ant)
            : base(ant)
        {
            Fruits = new MarkerCache(ant);
            Sugar = new MarkerCache(ant);

            Bugs = new MarkerCache(ant);
            Ants = new MarkerCache(ant);
        }

        public void Add(MarkerInformation markerInfo)
        {
            if (markerInfo.IsBugSpotted)
                Bugs.Add(markerInfo);
            else if (markerInfo.IsAntSpotted)
                Ants.Add(markerInfo);
            else if (markerInfo.IsSugarSpotted)
                Sugar.Add(markerInfo);
            else if (markerInfo.IsFruitNeedsCarriers)
                Fruits.Add(markerInfo);
        }

        public bool Contains(MarkerInformation markerInfo)
        {
            return Fruits.Contains(markerInfo)
                || Sugar.Contains(markerInfo)
                || Bugs.Contains(markerInfo)
                || Ants.Contains(markerInfo);
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
