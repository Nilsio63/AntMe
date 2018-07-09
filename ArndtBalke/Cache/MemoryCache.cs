using AntMe.English;
using AntMe.Player.ArndtBalke.Markers;

namespace AntMe.Player.ArndtBalke.Cache
{
    /// <summary>
    /// Cache for all other caches.
    /// </summary>
    internal class MemoryCache : CacheBase
    {
        /// <summary>
        /// All remembered fruits.
        /// </summary>
        public FoodCache<Fruit> Fruits { get; private set; }
        /// <summary>
        /// All remembered sugar piles.
        /// </summary>
        public FoodCache<Sugar> Sugar { get; private set; }

        /// <summary>
        /// All remembered bugs.
        /// </summary>
        public OpponentCache<Bug> Bugs { get; private set; }
        /// <summary>
        /// All remembered ants.
        /// </summary>
        public OpponentCache<Ant> Ants { get; private set; }

        /// <summary>
        /// All remembered signals.
        /// </summary>
        public SignalCache Signals { get; private set; }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        public MemoryCache()
        {
            // Instanciate all other caches
            Fruits = new FoodCache<Fruit>();
            Sugar = new FoodCache<Sugar>();

            Bugs = new OpponentCache<Bug>();
            Ants = new OpponentCache<Ant>();

            Signals = new SignalCache();
        }

        /// <summary>
        /// Adds a fruit to the fruit cache.
        /// </summary>
        /// <param name="fruit">The given fruit.</param>
        public void Add(Fruit fruit) => Fruits.Add(fruit);
        /// <summary>
        /// Adds a sugar to the sugar cache.
        /// </summary>
        /// <param name="sugar">The given sugar.</param>
        public void Add(Sugar sugar) => Sugar.Add(sugar);

        /// <summary>
        /// Adds a bug to the bug cache.
        /// </summary>
        /// <param name="bug">The given bug.</param>
        public void Add(Bug bug) => Bugs.Add(bug);
        /// <summary>
        /// Adds a ant to the ant cache.
        /// </summary>
        /// <param name="ant">The given ant.</param>
        public void Add(Ant ant) => Ants.Add(ant);

        /// <summary>
        /// Adds a signal to the signal cache.
        /// </summary>
        /// <param name="signal">The given signal.</param>
        public void Add(Signal signal) => Signals.Add(signal);

        /// <summary>
        /// Cleans the cache from old data.
        /// </summary>
        public override void Cleanup()
        {
            // Clean other caches
            Fruits.Cleanup();
            Sugar.Cleanup();

            Bugs.Cleanup();
            Ants.Cleanup();

            Signals.Cleanup();
        }

    }
}
