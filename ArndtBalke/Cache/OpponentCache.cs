using AntMe.English;

namespace AntMe.Player.ArndtBalke.Cache
{
    /// <summary>
    /// Cache for insect items.
    /// </summary>
    /// <typeparam name="T">The specified type of the cache items.</typeparam>
    internal class OpponentCache<T> : CacheBase<T>
        where T : Insect
    {
        /// <summary>
        /// Gets whether an item is deprecated.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>Returns true if the object is deprecated.</returns>
        protected override bool IsDeprecated(T obj)
        {
            // Check if insect has no health left
            return obj.CurrentEnergy <= 0;
        }

    }
}
