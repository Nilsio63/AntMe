using AntMe.English;
using AntMe.Player.ArndtBalke.MarkerInfo;
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

        protected override void DoNextMove()
        {
            if (Destination is Anthill)
                return;

            if (Destination is Ant a)
                MarkEnemyAntSpotted(a);
            else if (Destination is Bug b)
                MarkBugSpotted(b);

            if (DoMoveOpponentAnt())
                return;

            if (DoMoveBugs())
                return;
        }

        private bool DoMoveOpponentAnt()
        {
            Ant nearestAnt = _cacheEnemyAnts.OrderBy(o => GetDistanceTo(o)).FirstOrDefault();

            MarkerInformation nearestAntMarker = _cacheMarker.Where(o => o.IsAntSpotted).OrderBy(o => GetDistanceTo(o)).FirstOrDefault();

            if (nearestAnt != null
                && (nearestAntMarker == null || GetDistanceTo(nearestAnt) < GetDistanceTo(nearestAntMarker)))
            {
                Attack(nearestAnt);
                return true;
            }
            else if (nearestAntMarker != null && GetDistanceTo(nearestAntMarker) > ViewRange)
            {
                GoTo(nearestAntMarker);
                return true;
            }

            return false;
        }

        private bool DoMoveBugs()
        {
            Bug nearestBug = _cacheBugs.OrderBy(o => GetDistanceTo(o)).FirstOrDefault();

            MarkerInformation nearestBugMarker = _cacheMarker.Where(o => o.IsBugSpotted).OrderBy(o => GetDistanceTo(o)).FirstOrDefault();

            if (nearestBug != null
                && (nearestBugMarker == null || GetDistanceTo(nearestBug) < GetDistanceTo(nearestBugMarker)))
            {
                Attack(nearestBug);
                return true;
            }
            else if (nearestBugMarker != null && GetDistanceTo(nearestBugMarker) > ViewRange)
            {
                GoTo(nearestBugMarker);
                return true;
            }

            return false;
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
