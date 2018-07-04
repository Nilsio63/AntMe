using AntMe.English;
using AntMe.Player.ArndtBalke.Behavior;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal abstract class ItemCache<T> : CacheBase<T>
        where T : Item
    {
        protected ItemCache(BaseBehavior ant)
            : base(ant)
        { }

        protected override int GetDistanceToAnt(T obj)
        {
            return _ant.GetDistanceTo(obj);
        }

    }
}
