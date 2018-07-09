using AntMe.English;
using AntMe.Player.ArndtBalke.Markers;

namespace AntMe.Player.ArndtBalke.Map
{
    /// <summary>
    /// Class to save ant targets from either an antme item or a signal.
    /// </summary>
    internal class Target
    {
        /// <summary>
        /// The targeted item.
        /// </summary>
        public Item Item { get; private set; }
        /// <summary>
        /// The targeted signal.
        /// </summary>
        public Signal Signal { get; private set; }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="item">The targeted item.</param>
        public Target(Item item)
        {
            // Save item
            Item = item;
        }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="signal">The targeted signal.</param>
        public Target(Signal signal)
        {
            // Save signal
            Signal = signal;
        }

    }
}
