using AntMe.English;
using AntMe.Player.ArndtBalke.MarkerInfo;

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
        /// If the ant has no assigned tasks, it waits for new tasks. This method 
        /// is called to inform you that it is waiting.
        /// Read more: "http://wiki.antme.net/en/API1:Waiting"
        /// </summary>
        public override void Waiting()
        {
            GoForward();
        }

        #endregion

        #region Communication

        protected override void OnEnemyAntSpotted(MarkerInformation markerInfo)
        {
            if (Destination == null)
            {
                GoTo(markerInfo);
            }
        }

        #endregion

        #region Fight

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant detects an ant from an 
        /// enemy colony.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public override void SpotsEnemy(Ant ant)
        {
            base.SpotsEnemy(ant);

            if (Destination == null)
            {
                Attack(ant);
            }
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees a bug.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Bug)"
        /// </summary>
        /// <param name="bug">spotted bug</param>
        public override void SpotsEnemy(Bug bug)
        {
            base.SpotsEnemy(bug);

            if (Destination == null /*&& FriendlyAntsFromSameCasteInViewrange > 4*/)
            {
                Attack(bug);
            }
        }

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
