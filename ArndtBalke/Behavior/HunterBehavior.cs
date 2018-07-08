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
        #region Fields

        private AttackPoint currentAttackPoint = null;

        #endregion

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

        protected override Target GetNextTarget()
        {
            Target nextTarget = base.GetNextTarget();

            if (nextTarget != null)
                return nextTarget;

            if (currentAttackPoint != null)
            {
                currentAttackPoint.Age++;

                if (currentAttackPoint.Age > 55)
                    currentAttackPoint = null;
            }

            if (Destination is Anthill)
                return null;

            return GetNextAttackPoint()
                ?? GetNextFruitProtection()
                ?? GetNextOpponentAnt()
                ?? GetNextBugs();
        }

        private Target GetNextAttackPoint()
        {
            if (currentAttackPoint != null)
            {
                if (GetDistanceTo(currentAttackPoint) > 100)
                {
                    return new Target(currentAttackPoint);
                }
            }
            else
            {
                Signal nearestAttackPoint = _cache.Signals.FromType(AttackPoint).OrderBy(GetDistanceTo).FirstOrDefault();

                if (nearestAttackPoint != null)
                    return new Target(nearestAttackPoint);
            }

            return null;
        }

        private Target GetNextFruitProtection()
        {
            Signal nearestFruitMarker = _cache.Signals.FromType(FruitNeedsProtection).OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestFruitMarker != null && GetDistanceTo(nearestFruitMarker) > ViewRange)
                return new Target(nearestFruitMarker);

            return null;
        }

        private Target GetNextOpponentAnt()
        {
            Ant nearestAnt = _cache.Ants.OrderBy(GetDistanceTo).FirstOrDefault();

            Signal nearestAntMarker = _cache.Signals.FromType(AntSpotted).OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestAnt != null
                && (nearestAntMarker == null || GetDistanceTo(nearestAnt) < GetDistanceTo(nearestAntMarker) * 1.75))
            {
                return new Target(nearestAnt);
            }
            else if (nearestAntMarker != null && GetDistanceTo(nearestAntMarker) > ViewRange)
            {
                return new Target(nearestAntMarker);
            }

            return null;
        }

        private Target GetNextBugs()
        {
            Bug nearestBug = _cache.Bugs.OrderBy(GetDistanceTo).FirstOrDefault();

            Signal nearestBugMarker = _cache.Signals.FromType(BugSpotted).OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestBug != null
                && (nearestBugMarker == null || GetDistanceTo(nearestBug) < GetDistanceTo(nearestBugMarker) * 1.75))
            {
                return new Target(nearestBug);
            }
            else if (nearestBugMarker != null && GetDistanceTo(nearestBugMarker) > ViewRange)
            {
                return new Target(nearestBugMarker);
            }

            return null;
        }

        protected override Signal GetNextSignal()
        {
            Signal nextSignal = base.GetNextSignal();

            if (nextSignal != null)
                return nextSignal;

            if (Destination is Ant a)
                return new Signal(AntSpotted, GetCoordinate(a));
            else if (Destination is Bug b)
                return new Signal(BugSpotted, GetCoordinate(b));

            return null;
        }

        #endregion

        #region Communication

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
            base.UnderAttack(ant);

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
            base.UnderAttack(bug);

            Attack(bug);
        }

        #endregion

    }
}
