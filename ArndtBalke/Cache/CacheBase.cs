using System.Collections;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke.Cache
{
    /// <summary>
    /// Base class for all caches.
    /// </summary>
    internal abstract class CacheBase
    {
        /// <summary>
        /// Cleans the cache from old data.
        /// </summary>
        public abstract void Cleanup();

    }

    /// <summary>
    /// Base class for caches of a certain type.
    /// </summary>
    /// <typeparam name="T">The specified type of the cache items.</typeparam>
    internal abstract class CacheBase<T> : CacheBase, IEnumerable<T>
        where T : class
    {
        /// <summary>
        /// List to store all cache data.
        /// </summary>
        protected readonly List<T> _data = new List<T>();

        /// <summary>
        /// Adds the given object to the collection.
        /// </summary>
        /// <param name="obj">The object to be added to the cache.</param>
        public virtual void Add(T obj)
        {
            // Add object when not already contained
            if (!Contains(obj))
                _data.Add(obj);
        }

        /// <summary>
        /// Checks if the item is contained in the cache.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>Returns true if the item is already contained.</returns>
        public virtual bool Contains(T obj)
        {
            // Check if item is contained
            return _data.Contains(obj);
        }

        /// <summary>
        /// Cleans the cache from old data.
        /// </summary>
        public override void Cleanup()
        {
            // Iterate through data
            for (int i = 0; i < _data.Count; i++)
            {
                // Remove data if deprecated
                if (IsDeprecated(_data[i]))
                    _data.RemoveAt(i--);
            }
        }

        /// <summary>
        /// Gets whether an item is deprecated.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>Returns true if the object is deprecated.</returns>
        protected abstract bool IsDeprecated(T obj);

        /// <summary>
        /// Gets an enumerator to iterate through the list.
        /// </summary>
        /// <returns>Returns the list's enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            // Return enumerator for data
            return _data.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator to iterate through the list.
        /// </summary>
        /// <returns>Returns the list's enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Return enumerator for data
            return _data.GetEnumerator();
        }

    }
}
