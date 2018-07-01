namespace AntMe.Player.ArndtBalke.Map
{
    public class RotationCoordinates
    {
        public int Distance { get; private set; }
        public int Rotation { get; private set; }

        public RotationCoordinates(int distance, int rotation)
        {
            Distance = distance;
            Rotation = rotation;
        }

    }
}
