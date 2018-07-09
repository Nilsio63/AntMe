using AntMe.Player.ArndtBalke.Markers;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke.Cache
{
    /// <summary>
    /// Cache for signal items.
    /// </summary>
    internal class SignalCache : CacheBase<Signal>
    {
        /// <summary>
        /// Gets all signals of a certain type.
        /// </summary>
        /// <param name="infoType">The requested signal info type.</param>
        /// <returns>Gets all signals of the given type.</returns>
        public IEnumerable<Signal> FromType(short infoType)
        {
            // Iterate through all data items
            foreach (Signal signal in _data)
            {
                // Return item if info type equals the given type
                if (signal.InfoType == infoType)
                    yield return signal;
            }
        }

        /// <summary>
        /// Adds the given object to the collection.
        /// </summary>
        /// <param name="obj">The object to be added to the cache.</param>
        public override void Add(Signal obj)
        {
            // Add data to list
            _data.Add(obj);
        }

        /// <summary>
        /// Checks if the item is contained in the cache.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>Returns true if the item is already contained.</returns>
        public override bool Contains(Signal obj)
        {
            // Call base function
            if (base.Contains(obj))
                return true;

            // Return true if there is any signal with the same info type near to the given signal
            foreach (Signal signal in _data)
            {
                if (signal.InfoType == obj.InfoType
                    && signal.Coordinates.GetDistanceTo(obj.Coordinates) < 50)
                    return true;
            }

            // Return false
            return false;
        }

        /// <summary>
        /// Cleans the cache from old data.
        /// </summary>
        public override void Cleanup()
        {
            // Increase age of all signals
            foreach (Signal signal in _data)
                signal.Age++;

            // Call base method
            base.Cleanup();
        }

        /// <summary>
        /// Gets whether an item is deprecated.
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>Returns true if the object is deprecated.</returns>
        protected override bool IsDeprecated(Signal obj)
        {
            // Return true if there is any signal nearby with the same info type and is not older
            foreach (Signal signal in _data)
            {
                if (signal != obj
                    && signal.Age <= obj.Age
                    && signal.InfoType == obj.InfoType
                    && signal.Coordinates.GetDistanceTo(obj.Coordinates) < 70)
                    return true;
            }

            // Return true according to the info type and age of the signal
            if (obj.IsBugSpotted || obj.IsAntSpotted)
                return obj.Age > 1;
            else if (obj.IsSugarSpotted)
                return obj.Age > 15;
            else
                return obj.Age > 6;
        }

    }
}
