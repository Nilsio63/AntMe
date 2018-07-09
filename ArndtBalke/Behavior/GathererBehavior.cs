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

        /// <summary>
        /// Function to calculate the next target on each tick.
        /// </summary>
        /// <returns>Returns the next target or null.</returns>
        protected override Target GetNextTarget()
        {
            // Call base function
            Target nextTarget = base.GetNextTarget();

            if (nextTarget != null)
                return nextTarget;

            // Do nothing if target is anthill or ant is carrying something
            if (Destination is Anthill
                || CarryingFruit != null
                || CurrentLoad > 0)
                return null;

            // Get target for fruit or for sugar if fruit not known
            return GetTargetFruit()
                ?? GetTargetSugar();
        }

        /// <summary>
        /// Function to calculate the next fruit target.
        /// </summary>
        /// <returns>Returns the calculated fruit target.</returns>
        private Target GetTargetFruit()
        {
            // Get nearest fruit that needs carriers
            Fruit nearestFruit = _cache.Fruits.Where(NeedsCarrier).OrderBy(GetDistanceTo).FirstOrDefault();

            // Get nearest signal for 'fruit needs carriers'
            Signal nearestFruitSignal = _cache.Signals.FromType(FruitNeedsCarriers).OrderBy(GetDistanceTo).FirstOrDefault();
            
            if (nearestFruit != null
                && (nearestFruitSignal == null || GetDistanceTo(nearestFruit) < GetDistanceTo(nearestFruitSignal)))
            {
                // Return fruit as target if it's closer than signal
                return new Target(nearestFruit);
            }
            else if (nearestFruitSignal != null && GetDistanceTo(nearestFruitSignal) > ViewRange)
            {
                // Return signal as target if it's not yet in view range
                return new Target(nearestFruitSignal);
            }

            // Return no target
            return null;
        }

        /// <summary>
        /// Function to calculate the next sugar target.
        /// </summary>
        /// <returns>Returns the calculated sugar target.</returns>
        private Target GetTargetSugar()
        {
            // Get nearest sugar
            Sugar nearestSugar = _cache.Sugar.OrderBy(GetDistanceTo).FirstOrDefault();

            // Get nearest signal for 'sugar spotted'
            Signal nearestSugarSignal = _cache.Signals.FromType(SugarSpotted).OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestSugar != null
                && (nearestSugarSignal == null || GetDistanceTo(nearestSugar) < GetDistanceTo(nearestSugarSignal)))
            {
                // Return sugar as target if it's closer than signal
                return new Target(nearestSugar);
            }
            else if (nearestSugarSignal != null && GetDistanceTo(nearestSugarSignal) > ViewRange)
            {
                // Return signal as target if it's not yet in view range
                return new Target(nearestSugarSignal);
            }

            // Return no target
            return null;
        }

        /// <summary>
        /// Function to calculate the next signal on each tick.
        /// </summary>
        /// <returns>Returns the next signal or null.</returns>
        protected override Signal GetNextSignal()
        {
            // Call base function
            Signal nextSignal = base.GetNextSignal();

            if (nextSignal != null)
                return nextSignal;

            // Return signal for fruit if it needs carriers
            if (NeedsCarrier())
                return new Signal(FruitNeedsCarriers, GetCoordinate(CarryingFruit));

            // Return no target
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
            // Take fruit and return to anthill if it needs carriers
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
            // Take sugar and return to anthill
            Take(sugar);
            GoToAnthill();
        }

        #endregion

        #region Fight

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if an 
        /// enemy ant attacks; the ant can then decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Ant)"
        /// </summary>
        /// <param name="ant">attacking ant</param>
        public override void UnderAttack(Ant ant)
        {
            // Escape to anthill
            EscapeToAnthill();
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if a 
        /// bug attacks; the ant can decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Bug)"
        /// </summary>
        /// <param name="bug">attacking bug</param>
        public override void UnderAttack(Bug bug)
        {
            // Escape to anthill
            EscapeToAnthill();
        }

        /// <summary>
        /// Method to let the ant escape to it's anthill.
        /// </summary>
        private void EscapeToAnthill()
        {
            // Drop current load if present
            if (CarryingFruit != null || CurrentLoad > 0)
                Drop();

            // Escape to anthill
            GoToAnthill();
        }

        #endregion

    }
}
