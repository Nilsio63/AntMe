using AntMe.Player.ArndtBalke.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntMe.Player.ArndtBalke.MarkerInfo
{
    internal class MarkerInformation
    {
        public InfoType InfoType { get; private set; }
        public RotationCoordinates Coordinates { get; private set; }

        public MarkerInformation(InfoType infoType, RotationCoordinates coordinates)
        {
            InfoType = infoType;
            Coordinates = coordinates;
        }

        public MarkerInformation(int encoded)
        {
            InfoType = (InfoType)(encoded & 0xF);

            int coordinates = (encoded & 0x7FFFFFF0) >> 4;
            Coordinates = new RotationCoordinates(coordinates >> 9, coordinates & 0x1FF);
        }

        public int Encode()
        {
            int encoded = 0;

            encoded |= (int)InfoType & 0xF;

            int coordinates = 0;

            coordinates |= Coordinates.Distance << 9;
            coordinates |= Coordinates.Rotation & 0x1FF;

            encoded |= coordinates << 4;

            return encoded;
        }

    }
}
