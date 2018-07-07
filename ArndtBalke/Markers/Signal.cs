using AntMe.Player.ArndtBalke.Map;
using System;

namespace AntMe.Player.ArndtBalke.Markers
{
    /// <summary>
    /// Class to hold information for markers through bitwise integer operations.
    /// </summary>
    internal class Signal
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
        /// The hop count of the information.
        /// </summary>
        public short HopCount { get; private set; }
        /// <summary>
        /// Gets the age of the information.
        /// </summary>
        public int Age { get; set; }

        public bool IsBugSpotted => InfoType == 0;

        public bool IsAntSpotted => InfoType == 1;

        public bool IsSugarSpotted => InfoType == 2;

        public bool IsFruitNeedsCarriers => InfoType == 3;

        public bool IsFruitNeedsProtection => InfoType == 4;
        
        /// <summary>
        /// Creates a new marker information instance.
        /// </summary>
        /// <param name="infoType">The type of information to be sent.</param>
        /// <param name="coordinates">The coordinates where to find the object of interest.</param>
        public Signal(byte infoType, RelativeCoordinate coordinates)
        {
            InfoType = infoType;
            Coordinates = coordinates;
        }

        public Signal(Signal signal)
        {
            InfoType = signal.InfoType;
            Coordinates = signal.Coordinates;
            HopCount = (short)(signal.HopCount + 1);
        }

        /// <summary>
        /// Recreates a marker out of the encoded information.
        /// </summary>
        /// <param name="encoded">The encoded information integer.</param>
        public Signal(int encoded)
        {
            // Decode info type from lowest 4 Bits
            InfoType = (byte)(encoded & 0xF);

            // Decode coordinates from third highest to fifth lowest bit
            int coordinates = (encoded & 0x3FFFFFF0) >> 4;
            Coordinates = new RelativeCoordinate(GetInt32(coordinates >> 13), GetInt32(coordinates & 0x1FFF));

            // Decode hop count from two highest bits
            HopCount = (short)((encoded >> 30) & 0x3);
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

            coordinates |= GetInt13(Coordinates.X) << 13;
            coordinates |= GetInt13(Coordinates.Y) & 0x1FFF;

            // Add encoded coordinate data to third highest to fifth lowest bit
            encoded |= (coordinates << 4) & 0x7FFFFFF0;

            // Encode hop count as two highest bits
            encoded |= HopCount << 30;

            return encoded;
        }

        private int GetInt13(int int32)
        {
            int res = Math.Abs(int32) & 0xFFF;

            if (int32 < 0)
            {
                res |= 1 << 12;
            }

            return res;
        }

        private int GetInt32(int int13)
        {
            int value = int13 & 0xFFF;
            int sign = int13 >> 12;

            if (sign == 1)
                value = -value;

            return value;
        }

    }
}
