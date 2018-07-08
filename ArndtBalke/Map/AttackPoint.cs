namespace AntMe.Player.ArndtBalke.Map
{
    internal class AttackPoint : RelativeCoordinate
    {
        public int Age { get; set; }
        public int Priority { get; set; }

        public bool IsDeprecated => Age > 60;

        public AttackPoint(RelativeCoordinate coordinate, int priority)
            : this(coordinate.X, coordinate.Y)
        {
            Priority = priority;
        }

        public AttackPoint(int x, int y)
            : base(x, y)
        { }

    }
}
