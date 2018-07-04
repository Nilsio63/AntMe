using AntMe.Player.ArndtBalke.MarkerInfo;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class MarkerCache : CacheBase<MarkerInformation>
    {
        protected override bool Contains(MarkerInformation obj)
        {
            return base.Contains(obj)
                || _list.Any(o => o.InfoType == obj.InfoType && o.Coordinates.GetDistanceTo(obj.Coordinates) < 50);
        }

        protected override bool IsDeprecated(MarkerInformation obj)
        {
            obj.Age++;

            if (obj.IsBugSpotted || obj.IsAntSpotted)
                return obj.Age > 1;
            else
                return obj.Age > 6;
        }

    }
}
