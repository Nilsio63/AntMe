using AntMe.English;
using AntMe.Player.ArndtBalke.Markers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Behavior
{
    internal class ScoutBehavior : BaseBehavior
    {
        public override string Caste => "Scout";

        public ScoutBehavior(ArndtBalkeClass ant) : base(ant)
        { }

        protected override Signal GetNextSignal()
        {
            return base.GetNextSignal()
                ?? GetRandomInfoSignal();
        }

        private Signal GetRandomInfoSignal()
        {
            List<Func<Signal>> signalFunctions = new List<Func<Signal>>()
            {
                GetRandomAntSignal,
                GetRandomBugSignal,
                GetRandomSugarSignal,
                GetRandomFruitSignal
            };

            while (signalFunctions.Any())
            {
                int randomIndex = RandomNumber.Number(signalFunctions.Count);
                Signal signal = signalFunctions[randomIndex].Invoke();

                if (signal != null)
                {
                    Think($"Signal: Type: {signal.InfoType}; X: {signal.Coordinates.X}; Y: {signal.Coordinates.Y}");
                    return signal;
                }

                signalFunctions.RemoveAt(randomIndex);
            }

            return null;
        }

        private Signal GetRandomAntSignal()
        {
            List<Ant> listAnts = new List<Ant>();

            foreach (Ant ant in _cache.Ants)
            {
                if (ant.CarriedFruit != null
                    || ant.CurrentLoad > 0)
                    listAnts.Add(ant);
            }

            if (listAnts.Any())
            {
                return new Signal(AntSpotted, GetCoordinate(listAnts[RandomNumber.Number(listAnts.Count)]));
            }

            return null;
        }

        private Signal GetRandomBugSignal()
        {
            List<Bug> listBugs = new List<Bug>(_cache.Bugs);

            if (listBugs.Any())
            {
                return new Signal(BugSpotted, GetCoordinate(listBugs[RandomNumber.Number(listBugs.Count)]));
            }

            return null;
        }

        private Signal GetRandomSugarSignal()
        {
            List<Sugar> listSugar = new List<Sugar>();

            foreach (Sugar sugar in _cache.Sugar)
            {
                if (sugar.Amount > 250)
                    listSugar.Add(sugar);
            }

            if (listSugar.Any())
            {
                return new Signal(SugarSpotted, GetCoordinate(listSugar[RandomNumber.Number(listSugar.Count)]));
            }

            return null;
        }

        private Signal GetRandomFruitSignal()
        {
            List<Fruit> listFruits = new List<Fruit>();

            foreach (Fruit fruit in _cache.Fruits)
            {
                foreach (Ant ant in _cache.Ants)
                {
                    if (Coordinate.GetDistanceBetween(ant, fruit) < 75)
                        return new Signal(FruitNeedsProtection, GetCoordinate(ant));
                }

                if (NeedsCarrier(fruit))
                    listFruits.Add(fruit);
            }

            if (listFruits.Any())
            {
                return new Signal(FruitNeedsCarriers, GetCoordinate(listFruits[RandomNumber.Number(listFruits.Count)]));
            }

            return null;
        }

        #region Fight

        public override void UnderAttack(Ant ant)
        {
            GoToAnthill();
        }

        public override void UnderAttack(Bug bug)
        {
            GoToAnthill();
        }

        #endregion

    }
}
