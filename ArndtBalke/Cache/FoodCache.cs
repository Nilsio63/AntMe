using AntMe.English;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class FoodCache<T> : ItemCache<T>
        where T : Food
    {
        protected override bool IsDeprecated(T obj)
        {
            return obj.Amount <= 0;
        }

    }
}
