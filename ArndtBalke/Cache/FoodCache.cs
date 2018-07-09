using AntMe.English;

namespace AntMe.Player.ArndtBalke.Cache
{
    /// <summary>
    /// Cache for food items.
    /// </summary>
    /// <typeparam name="T">The specified type of the cache items.</typeparam>
    internal class FoodCache<T> : CacheBase<T>
        where T : Food
    {
        /// <summary>
        /// Gets whether an item is deprecated.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>Returns true if the object is deprecated.</returns>
        protected override bool IsDeprecated(T obj)
        {
            // Check if item has no amount left
            return obj.Amount <= 0;
        }

    }
}
