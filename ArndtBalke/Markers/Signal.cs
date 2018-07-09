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

        /// <summary>
        /// Gets where the signal's info type is 'bug spotted'.
        /// </summary>
        public bool IsBugSpotted => InfoType == 0;
        /// <summary>
        /// Gets where the signal's info type is 'ant spotted'.
        /// </summary>
        public bool IsAntSpotted => InfoType == 1;
        /// <summary>
        /// Gets where the signal's info type is 'sugar spotted'.
        /// </summary>
        public bool IsSugarSpotted => InfoType == 2;
        /// <summary>
        /// Gets where the signal's info type is 'fruit needs carriers'.
        /// </summary>
        public bool IsFruitNeedsCarriers => InfoType == 3;
        /// <summary>
        /// Gets where the signal's info type is 'fruit needs protection'.
        /// </summary>
        public bool IsFruitNeedsProtection => InfoType == 4;

        /// <summary>
        /// Creates a new marker information instance.
        /// </summary>
        /// <param name="infoType">The type of information to be sent.</param>
        /// <param name="coordinates">The coordinates where to find the object of interest.</param>
        public Signal(byte infoType, RelativeCoordinate coordinates)
        {
            // Save type and coordinates
            InfoType = infoType;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Creates a new marker information instance.
        /// </summary>
        /// <param name="signal">The signal to re-emit.</param>
        public Signal(Signal signal)
        {
            // Save type, coordinates and hop count + 1
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

        /// <summary>
        /// Converts an int32 to a custom int13.
        /// </summary>
        /// <param name="int32">The int32 to be encoded.</param>
        /// <returns>Returns the int13 object.</returns>
        private int GetInt13(int int32)
        {
            // Mask absolute value to get lowest 12 bits
            int res = Math.Abs(int32) & 0xFFF;

            // Add a 1 on 13th bit when negative
            if (int32 < 0)
                res |= 1 << 12;

            return res;
        }

        /// <summary>
        /// Converts a custom int13 back to an int32.
        /// </summary>
        /// <param name="int13">The int13 to be decoded.</param>
        /// <returns>Returns the int32 object.</returns>
        private int GetInt32(int int13)
        {
            // Mask lowest 12 bit
            int value = int13 & 0xFFF;
            // Get 13th bit as sign
            int sign = int13 >> 12;

            // Turn value when sign is set
            if (sign == 1)
                value = -value;

            return value;
        }

    }
}
