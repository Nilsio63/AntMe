using AntMe.English;
using System;

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

        public RelativeCoordinate(Anthill anthill, Item item)
        {
            double distance = Coordinate.GetDistanceBetween(anthill, item);
            double degrees = Coordinate.GetDegreesBetween(anthill, item);

            SetCoordinatesBeDegrees(distance, degrees);
        }

        public RelativeCoordinate(Anthill anthill, BaseAnt ant)
        {
            double distance = Coordinate.GetDistanceBetween(anthill, ant);
            double degrees = Coordinate.GetDegreesBetween(anthill, ant);

            SetCoordinatesBeDegrees(distance, degrees);
        }

        private void SetCoordinatesBeDegrees(double distance, double degrees)
        {
            double x = Math.Cos(degrees * Math.PI / 180) * distance;
            double y = Math.Sin(degrees * Math.PI / 180) * distance;

            X = (int)x;
            Y = (int)y;
        }

        public int GetDistanceTo(RelativeCoordinate coordinate)
        {
            int x = X - coordinate.X;
            int y = Y - coordinate.Y;

            double distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            return (int)distance;
        }

    }
}
