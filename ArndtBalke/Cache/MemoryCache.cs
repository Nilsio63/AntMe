using AntMe.English;
using AntMe.Player.ArndtBalke.MarkerInfo;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class MemoryCache : CacheBase
    {
        public FoodCache<Fruit> Fruits { get; private set; }
        public FoodCache<Sugar> Sugar { get; private set; }

        public OpponentCache<Bug> Bugs { get; private set; }
        public OpponentCache<Ant> Ants { get; private set; }

        public MarkerMemoryCache Markers { get; private set; }

        public MemoryCache()
        {
            Fruits = new FoodCache<Fruit>();
            Sugar = new FoodCache<Sugar>();

            Bugs = new OpponentCache<Bug>();
            Ants = new OpponentCache<Ant>();

            Markers = new MarkerMemoryCache();
        }

        public void Add(Fruit fruit) => Fruits.Add(fruit);
        public void Add(Sugar sugar) => Sugar.Add(sugar);

        public void Add(Bug bug) => Bugs.Add(bug);
        public void Add(Ant ant) => Ants.Add(ant);

        public void Add(MarkerInformation markerInfo)
        {
            Markers.Add(markerInfo);
        }

        public override void Cleanup()
        {
            Fruits.Cleanup();
            Sugar.Cleanup();

            Bugs.Cleanup();
            Ants.Cleanup();

            Markers.Cleanup();
        }

    }
}
