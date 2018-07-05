using AntMe.Player.ArndtBalke.Behavior;
using AntMe.Player.ArndtBalke.MarkerInfo;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class MarkerCache : CacheBase<MarkerInformation>
    {
        public override void Add(MarkerInformation obj)
        {
            _list.Add(obj);
        }

        public override bool Contains(MarkerInformation obj)
        {
            if (base.Contains(obj))
                return true;

            foreach (MarkerInformation markerInfo in _list)
            {
                if (markerInfo.InfoType == obj.InfoType
                    && markerInfo.Coordinates.GetDistanceTo(obj.Coordinates) < 50)
                    return true;
            }

            return false;
        }

        public override void Cleanup()
        {
            foreach (MarkerInformation markerInfo in _list)
            {
                markerInfo.Age++;
            }

            base.Cleanup();
        }

        protected override bool IsDeprecated(MarkerInformation obj)
        {
            foreach (MarkerInformation markerInfo in _list)
            {
                if (markerInfo != obj
                    && markerInfo.Age <= obj.Age
                    && markerInfo.InfoType == obj.InfoType
                    && markerInfo.Coordinates.GetDistanceTo(obj.Coordinates) < 70)
                    return true;
            }

            if (obj.IsBugSpotted || obj.IsAntSpotted)
                return obj.Age > 1;
            else if (obj.IsSugarSpotted)
                return obj.Age > 15;
            else
                return obj.Age > 6;
        }

    }
}
