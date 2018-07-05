using AntMe.Player.ArndtBalke.Markers;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal class SignalCache : CacheBase<Signal>
    {
        public override void Add(Signal obj)
        {
            _list.Add(obj);
        }

        public override bool Contains(Signal obj)
        {
            if (base.Contains(obj))
                return true;

            foreach (Signal signal in _list)
            {
                if (signal.InfoType == obj.InfoType
                    && signal.Coordinates.GetDistanceTo(obj.Coordinates) < 50)
                    return true;
            }

            return false;
        }

        public override void Cleanup()
        {
            foreach (Signal signal in _list)
            {
                signal.Age++;
            }

            base.Cleanup();
        }

        protected override bool IsDeprecated(Signal obj)
        {
            foreach (Signal signal in _list)
            {
                if (signal != obj
                    && signal.Age <= obj.Age
                    && signal.InfoType == obj.InfoType
                    && signal.Coordinates.GetDistanceTo(obj.Coordinates) < 70)
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
