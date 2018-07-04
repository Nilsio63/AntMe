using AntMe.English;
using AntMe.Player.ArndtBalke.Behavior;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class FruitCache : FoodCache<Fruit>
    {
        public FruitCache(BaseBehavior ant)
            : base(ant)
        { }

        protected override bool IsValid(Fruit obj)
        {
            return _ant.NeedsCarrier(obj);
        }

    }
}
