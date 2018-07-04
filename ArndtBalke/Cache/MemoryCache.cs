using AntMe.English;
using AntMe.Player.ArndtBalke.Behavior;
using AntMe.Player.ArndtBalke.MarkerInfo;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class MemoryCache : CacheBase
    {
        public FruitCache Fruits { get; private set; }
        public FoodCache<Sugar> Sugar { get; private set; }

        public OpponentCache<Bug> Bugs { get; private set; }
        public OpponentCache<Ant> Ants { get; private set; }

        public MarkerMemoryCache Markers { get; private set; }

        public MemoryCache(BaseBehavior ant)
            : base(ant)
        {
            Fruits = new FruitCache(ant);
            Sugar = new FoodCache<Sugar>(ant);

            Bugs = new OpponentCache<Bug>(ant);
            Ants = new OpponentCache<Ant>(ant);

            Markers = new MarkerMemoryCache(ant);
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
