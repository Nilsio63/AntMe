using AntMe.English;
using AntMe.Player.ArndtBalke.Behavior;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class OpponentCache<T> : ItemCache<T>
        where T : Insect
    {
        public OpponentCache(BaseBehavior ant)
            : base(ant)
        { }

        protected override bool IsDeprecated(T obj)
        {
            return obj.CurrentEnergy <= 0;
        }

    }
}
