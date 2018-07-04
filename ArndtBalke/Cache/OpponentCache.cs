using AntMe.English;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class OpponentCache<T> : CacheBase<T>
        where T : Insect
    {
        protected override bool IsDeprecated(T obj)
        {
            return obj.CurrentEnergy <= 0;
        }

    }
}
