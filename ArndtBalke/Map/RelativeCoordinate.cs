namespace AntMe.Player.ArndtBalke.Map
{
    /// <summary>
    /// Class to save relative position of an object.
    /// </summary>
    public class RelativeCoordinate
    {
        /// <summary>
        /// The distance between the object and the relation.
        /// </summary>
        public int Distance { get; private set; }
        /// <summary>
        /// The rotational degree between the object and the relation.
        /// </summary>
        public int Rotation { get; private set; }

        /// <summary>
        /// Creates a new coordinate instance.
        /// </summary>
        /// <param name="distance">The distance to the relation.</param>
        /// <param name="rotation">The rotation to the relation.</param>
        public RelativeCoordinate(int distance, int rotation)
        {
            Distance = distance < 0 ? 0 : distance;
            Rotation = rotation < 0 ? rotation + 360 : rotation;
        }

    }
}
