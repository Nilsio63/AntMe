using AntMe.English;
using AntMe.Player.ArndtBalke.Behavior;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class FoodCache<T> : ItemCache<T>
        where T : Food
    {
        public FoodCache(BaseBehavior ant)
            : base(ant)
        { }

        protected override bool IsDeprecated(T obj)
        {
            return obj.Amount <= 0;
        }

    }
}
