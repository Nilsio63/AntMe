namespace AntMe.Player.ArndtBalke.Map
{
    /// <summary>
    /// Class to save position of an object relative to the anthill.
    /// </summary>
    public class RotationCoordinates
    {
        /// <summary>
        /// The distance between the object and the anthill.
        /// </summary>
        public int Distance { get; private set; }
        /// <summary>
        /// The rotational degree between the object and the anthill.
        /// </summary>
        public int Rotation { get; private set; }

        /// <summary>
        /// Creates a new coordinate instance.
        /// </summary>
        /// <param name="distance">The distance to the anthill.</param>
        /// <param name="rotation">The rotation to the anthill.</param>
        public RotationCoordinates(int distance, int rotation)
        {
            Distance = distance < 0 ? 0 : distance;
            Rotation = rotation < 0 ? rotation + 360 : rotation;
        }

    }
}
