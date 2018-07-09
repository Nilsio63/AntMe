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

        /// <summary>
        /// Creates a new coordinate instance.
        /// </summary>
        /// <param name="anthill">The anthill as reference object.</param>
        /// <param name="item">The item for the coordinates.</param>
        public RelativeCoordinate(Anthill anthill, Item item)
        {
            // Calculate distance and degree
            double distance = Coordinate.GetDistanceBetween(anthill, item);
            double degrees = Coordinate.GetDegreesBetween(anthill, item);

            // Calculate coordinates with distance and degree
            SetCoordinatesBeDegrees(distance, degrees);
        }

        /// <summary>
        /// Creates a new coordinate instance.
        /// </summary>
        /// <param name="anthill">The anthill as reference object.</param>
        /// <param name="ant">The ant for the coordinate.</param>
        public RelativeCoordinate(Anthill anthill, BaseAnt ant)
        {
            // Calculate distance and degree
            double distance = Coordinate.GetDistanceBetween(anthill, ant);
            double degrees = Coordinate.GetDegreesBetween(anthill, ant);

            // Calculate coordinates with distance and degree
            SetCoordinatesBeDegrees(distance, degrees);
        }

        /// <summary>
        /// Calculates the X and Y coordinates depending on the distance and degree.
        /// </summary>
        /// <param name="distance">The distance between the anthill and object.</param>
        /// <param name="degrees">The degree between the anthill and object.</param>
        private void SetCoordinatesBeDegrees(double distance, double degrees)
        {
            // Calculate X and Y coordinates
            X = (int)(Math.Cos(degrees * Math.PI / 180) * distance);
            Y = (int)(Math.Sin(degrees * Math.PI / 180) * distance);
        }

        /// <summary>
        /// Get the distance to the given coordinates.
        /// </summary>
        /// <param name="coordinate">The coordinates to be checked.</param>
        /// <returns>Returns the distance between the two objects.</returns>
        public int GetDistanceTo(RelativeCoordinate coordinate)
        {
            // Return -1 on no coordinate given
            if (coordinate == null)
                return -1;

            // Calculate x and y deltas
            int x = X - coordinate.X;
            int y = Y - coordinate.Y;

            // Calculate and return distances with pythagoras
            return (int)(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
        }

    }
}
