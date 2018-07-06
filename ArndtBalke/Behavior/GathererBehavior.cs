using AntMe.English;
using AntMe.Player.ArndtBalke.Map;
using AntMe.Player.ArndtBalke.Markers;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Behavior
{
    /// <summary>
    /// Behavior for gatherer ants.
    /// </summary>
    internal class GathererBehavior : BaseBehavior
    {
        #region Properties

        /// <summary>
        /// The caste name for this behavior.
        /// </summary>
        public override string Caste => "Gatherer";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of gatherer behavior.
        /// </summary>
        /// <param name="ant">The ant to be controlled.</param>
        public GathererBehavior(ArndtBalkeClass ant)
            : base(ant)
        { }

        #endregion

        #region Movement

        protected override Target GetNextTarget()
        {
            Target nextTarget = base.GetNextTarget();

            if (nextTarget != null)
                return nextTarget;

            if (Destination is Anthill
                || CarryingFruit != null
                || CurrentLoad > 0)
                return null;

            return GetTargetFruit()
                ?? GetTargetSugar();
        }

        private Target GetTargetFruit()
        {
            Fruit nearestFruit = _cache.Fruits.Where(NeedsCarrier).OrderBy(GetDistanceTo).FirstOrDefault();

            Signal nearestFruitMarker = _cache.Markers.Fruits.OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestFruit != null
                && (nearestFruitMarker == null || GetDistanceTo(nearestFruit) < GetDistanceTo(nearestFruitMarker)))
            {
                return new Target(nearestFruit);
            }
            else if (nearestFruitMarker != null && GetDistanceTo(nearestFruitMarker) > ViewRange)
            {
                return new Target(nearestFruitMarker);
            }

            return null;
        }

        private Target GetTargetSugar()
        {
            Sugar nearestSugar = _cache.Sugar.OrderBy(GetDistanceTo).FirstOrDefault();

            Signal nearestSugarMarker = _cache.Markers.Sugar.OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestSugar != null
                && (nearestSugarMarker == null || GetDistanceTo(nearestSugar) < GetDistanceTo(nearestSugarMarker)))
            {
                return new Target(nearestSugar);
            }
            else if (nearestSugarMarker != null && GetDistanceTo(nearestSugarMarker) > ViewRange)
            {
                return new Target(nearestSugarMarker);
            }

            return null;
        }

        protected override Signal GetNextSignal()
        {
            Signal nextSignal = base.GetNextSignal();

            if (nextSignal != null)
                return nextSignal;

            if (NeedsCarrier())
                return new Signal(FruitNeedsCarriers, GetCoordinate(CarryingFruit));

            return null;
        }

        #endregion

        #region Food

        /// <summary>
        /// If the ant’s destination is a piece of fruit, this method is called as soon 
        /// as the ant reaches its destination. It means that the ant is now near enough 
        /// to its destination/target to interact with it.
        /// Read more: "http://wiki.antme.net/en/API1:DestinationReached(Fruit)"
        /// </summary>
        /// <param name="fruit">reached fruit</param>
        public override void DestinationReached(Fruit fruit)
        {
            base.DestinationReached(fruit);

            if (NeedsCarrier(fruit))
            {
                Take(fruit);
                GoToAnthill();
            }
        }

        /// <summary>
        /// If the ant’s destination is a mound of sugar, this method is called as soon 
        /// as the ant has reached its destination. It means that the ant is now near 
        /// enough to its destination/target to interact with it.
        /// Read more: "http://wiki.antme.net/en/API1:DestinationReached(Sugar)"
        /// </summary>
        /// <param name="sugar">reached sugar</param>
        public override void DestinationReached(Sugar sugar)
        {
            base.DestinationReached(sugar);

            Take(sugar);
            GoToAnthill();
        }

        #endregion

        #region Communication

        #endregion

        #region Fight

        public override void UnderAttack(Ant ant)
        {
            if (CarryingFruit != null || CurrentLoad > 0)
                Drop();

            GoToAnthill();
        }

        public override void UnderAttack(Bug bug)
        {
            if (CarryingFruit != null || CurrentLoad > 0)
                Drop();

            GoToAnthill();
        }

        #endregion

    }
}
