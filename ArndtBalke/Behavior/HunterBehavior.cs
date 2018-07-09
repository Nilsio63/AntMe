using AntMe.English;
using AntMe.Player.ArndtBalke.Map;
using AntMe.Player.ArndtBalke.Markers;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Behavior
{
    /// <summary>
    /// Behavior for hunter ants.
    /// </summary>
    internal class HunterBehavior : BaseBehavior
    {
        #region Properties

        /// <summary>
        /// The caste name for this behavior.
        /// </summary>
        public override string Caste => "Hunter";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of hunter behavior.
        /// </summary>
        /// <param name="ant">The ant to be controlled.</param>
        public HunterBehavior(ArndtBalkeClass ant)
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
            // Do nothing if target is anthill
            if (Destination is Anthill)
                return null;

            // Get target for fruit, ant or bug
            return GetNextFruitProtection()
                ?? GetNextOpponentAnt()
                ?? GetNextBug();
        }

        /// <summary>
        /// Function to calculate the next target for fruit protection.
        /// </summary>
        /// <returns>Returns the calculated fruit protection target.</returns>
        private Target GetNextFruitProtection()
        {
            // Get nearest signal for 'fruit needs protection'
            Signal nearestFruitSignal = _cache.Signals.FromType(FruitNeedsProtection).OrderBy(GetDistanceTo).FirstOrDefault();

            // Return target for signal if signal not already in view range
            if (nearestFruitSignal != null && GetDistanceTo(nearestFruitSignal) > ViewRange)
                return new Target(nearestFruitSignal);

            // Return no target
            return null;
        }

        /// <summary>
        /// Function to calculate the next ant target.
        /// </summary>
        /// <returns>Returns the calculated ant target.</returns>
        private Target GetNextOpponentAnt()
        {
            // Get nearest ant
            Ant nearestAnt = _cache.Ants.OrderBy(GetDistanceTo).FirstOrDefault();

            // Get nearest ant signal
            Signal nearestAntSignal = _cache.Signals.FromType(AntSpotted).OrderBy(GetDistanceTo).FirstOrDefault();
            
            if (nearestAnt != null
                && (nearestAntSignal == null || GetDistanceTo(nearestAnt) < GetDistanceTo(nearestAntSignal) * 2.5))
            {
                // Return ant as target if it's closer than distance to signal times 2.5
                return new Target(nearestAnt);
            }
            else if (nearestAntSignal != null && GetDistanceTo(nearestAntSignal) > ViewRange)
            {
                // Return signal as target if it's not yet in view range
                return new Target(nearestAntSignal);
            }

            // Return no target
            return null;
        }

        /// <summary>
        /// Function to calculate the next bug target.
        /// </summary>
        /// <returns>Returns the calculated bug target.</returns>
        private Target GetNextBug()
        {
            // Get nearest bug
            Bug nearestBug = _cache.Bugs.OrderBy(GetDistanceTo).FirstOrDefault();

            // Get nearest bug signal
            Signal nearestBugSignal = _cache.Signals.FromType(BugSpotted).OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestBug != null
                && (nearestBugSignal == null || GetDistanceTo(nearestBug) < GetDistanceTo(nearestBugSignal) * 2.5))
            {
                // Return bug as target if it's closer than distance to signal times 2.5
                return new Target(nearestBug);
            }
            else if (nearestBugSignal != null && GetDistanceTo(nearestBugSignal) > ViewRange)
            {
                // Return signal as target if it's not yet in view range
                return new Target(nearestBugSignal);
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

            // Create signal for current destination
            if (Destination is Ant a)
                return new Signal(AntSpotted, GetCoordinate(a));
            else if (Destination is Bug b)
                return new Signal(BugSpotted, GetCoordinate(b));

            // Return no signal
            return null;
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
            // Attack ant
            Attack(ant);
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if a 
        /// bug attacks; the ant can decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Bug)"
        /// </summary>
        /// <param name="bug">attacking bug</param>
        public override void UnderAttack(Bug bug)
        {
            // Attack bug
            Attack(bug);
        }

        #endregion

    }
}
