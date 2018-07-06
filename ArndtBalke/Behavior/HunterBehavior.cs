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

        protected override Target GetNextTarget()
        {
            if (Destination is Anthill)
                return null;

            return GetNextOpponentAnt()
                ?? GetNextBugs();
        }

        private Target GetNextOpponentAnt()
        {
            Ant nearestAnt = _cache.Ants.OrderBy(GetDistanceTo).FirstOrDefault();

            Signal nearestAntMarker = _cache.Markers.Ants.OrderBy(GetDistanceTo).FirstOrDefault();

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

            Signal nearestBugMarker = _cache.Markers.Bugs.OrderBy(GetDistanceTo).FirstOrDefault();

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
