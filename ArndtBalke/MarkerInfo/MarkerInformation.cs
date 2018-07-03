using AntMe.Player.ArndtBalke.Map;
using System;

namespace AntMe.Player.ArndtBalke.MarkerInfo
{
    /// <summary>
    /// Class to hold information for markers through bitwise integer operations.
    /// </summary>
    internal class MarkerInformation
    {
        /// <summary>
        /// The type of information to be sent.
        /// </summary>
        public byte InfoType { get; private set; }
        /// <summary>
        /// The coordinates of the original marker.
        /// </summary>
        public RelativeCoordinate Coordinates { get; private set; }

        /// <summary>
        /// Creates a new marker information instance.
        /// </summary>
        /// <param name="infoType">The type of information to be sent.</param>
        public MarkerInformation(byte infoType)
            : this(infoType, null)
        { }

        /// <summary>
        /// Creates a new marker information instance.
        /// </summary>
        /// <param name="infoType">The type of information to be sent.</param>
        /// <param name="coordinates">The coordinates where to find the object of interest.</param>
        public MarkerInformation(byte infoType, RelativeCoordinate coordinates)
        {
            InfoType = infoType;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Recreates a marker out of the encoded information.
        /// </summary>
        /// <param name="encoded">The encoded information integer.</param>
        public MarkerInformation(int encoded)
        {
            // Decode info type from lowest 4 Bits
            InfoType = (byte)(encoded & 0xF);

            // Decode coordinates from second highest to fifth lowest bit
            int coordinates = (encoded & 0x7FFFFFF0) >> 4;
            Coordinates = new RelativeCoordinate(GetInt32(coordinates >> 13), GetInt32(coordinates & 0x1FFF));
        }

        /// <summary>
        /// Encodes the given marker information of the instance.
        /// </summary>
        /// <returns>Returns the encoded integer.</returns>
        public int Encode()
        {
            int encoded = 0;

            // Encode info type as 4 lowest bits
            encoded |= InfoType & 0xF;

            // Encode coordinate data
            int coordinates = 0;
            if (Coordinates != null)
            {
                coordinates |= GetInt13(Coordinates.X) << 13;
                coordinates |= GetInt13(Coordinates.Y) & 0x1FFF;
            }

            // Add encoded coordinate data to second highest to fifth lowest bit
            encoded |= (coordinates << 4) & 0x7FFFFFF0;

            return encoded;
        }

        private int GetInt13(int int32)
        {
            int res = Math.Abs(int32) & 0xFFF;

            System.Diagnostics.Trace.WriteLine(Convert.ToString(int32, 2));
            System.Diagnostics.Trace.WriteLine(Convert.ToString(res, 2));

            if (int32 < 0)
            {
                res |= 1 << 12;
            }

            System.Diagnostics.Trace.WriteLine(Convert.ToString(res, 2));

            return res;
        }

        private int GetInt32(int int13)
        {
            int value = int13 & 0xFFF;
            int sign = int13 >> 12;

            if (sign == 1)
                value = -value;

            System.Diagnostics.Trace.WriteLine(Convert.ToString(int13, 2));
            System.Diagnostics.Trace.WriteLine(Convert.ToString(value, 2));
            System.Diagnostics.Trace.WriteLine(Convert.ToString(sign, 2));

            return value;
        }

    }
}
