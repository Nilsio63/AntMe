using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntMe.Player.ArndtBalke.MarkerInfo
{
    internal class MarkerInformation
    {
        public InfoType InfoType { get; private set; }

        public MarkerInformation(InfoType infoType/* TODO: Coordinates */)
        {
            InfoType = infoType;
        }

        public MarkerInformation(int encoded)
        {
            InfoType = (InfoType)(encoded & 0x04);

            // TODO: Coordinates
            throw new NotImplementedException();
        }

        public int Encode()
        {
            // TODO
            throw new NotImplementedException();
        }

    }
}
