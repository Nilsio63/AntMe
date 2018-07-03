namespace AntMe.Player.ArndtBalke.Map
{
    /// <summary>
    /// Class to save relative position of an object.
    /// </summary>
    public class RelativeCoordinate
    {
        /// <summary>
        /// The X coordinate relative to the anthill.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// The Y coordinate relative to the anthill.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Creates a new coordinate instance.
        /// </summary>
        /// <param name="x">The X coordinate relative to the anthill.</param>
        /// <param name="y">The Y coordinate relative to the anthill.</param>
        public RelativeCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}
