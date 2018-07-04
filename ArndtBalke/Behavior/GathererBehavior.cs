using AntMe.English;
using AntMe.Player.ArndtBalke.MarkerInfo;
using System.Collections.Generic;
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
        /// If the ant has no assigned tasks, it waits for new tasks. This method 
        /// is called to inform you that it is waiting.
        /// Read more: "http://wiki.antme.net/en/API1:Waiting"
        /// </summary>
        public override void Waiting()
        {
            Fruit f = _listFruit.OrderBy(o => GetDistanceTo(o)).FirstOrDefault();

            if (f != null)
            {
                GoTo(f);
                return;
            }

            Sugar s = _listSugar.OrderBy(o => GetDistanceTo(o)).FirstOrDefault();

            if (s != null)
            {
                GoTo(s);
                return;
            }

            GoForward();
        }

        /// <summary>
        /// This method is called in every simulation round, regardless of additional 
        /// conditions. It is ideal for actions that must be executed but that are not 
        /// addressed by other methods.
        /// Read more: "http://wiki.antme.net/en/API1:Tick"
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (NeedsCarrier())
            {
                MarkFruitNeedsCarriers(CarryingFruit);
            }
        }

        #endregion

        #region Food

        /// <summary>
        /// This method is called as soon as an ant sees an apple within its 360° 
        /// visual range. The parameter is the piece of fruit that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Fruit)"
        /// </summary>
        /// <param name="fruit">spotted fruit</param>
        public override void Spots(Fruit fruit)
        {
            base.Spots(fruit);

            if (NeedsCarrier(fruit))
            {
                MarkFruitNeedsCarriers(fruit);

                if (Destination == null || Destination is Sugar || Destination is Marker)
                {
                    GoTo(fruit);
                }
            }
        }

        /// <summary>
        /// This method is called as soon as an ant sees a mound of sugar in its 360° 
        /// visual range. The parameter is the mound of sugar that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Sugar)"
        /// </summary>
        /// <param name="sugar">spotted sugar</param>
        public override void Spots(Sugar sugar)
        {
            base.Spots(sugar);

            if (CarryingFruit == null && CurrentLoad < MaximumLoad && Destination == null)
            {
                if (Range - WalkedRange > GetDistanceTo(Anthill))
                {
                    GoTo(sugar);
                }
            }
        }

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

        protected override bool IgnoreMarker(MarkerInformation markerInfo)
        {
            if (Destination == null)
            {
                if (Destination is Anthill)
                    return true;
                else if (Destination is Sugar)
                    return false;
                else if (GetDistanceTo(Destination) < GetDistanceTo(markerInfo.Coordinates))
                    return true;
            }

            return false;
        }

        protected override void OnSugarSpotted(MarkerInformation markerInfo)
        {
            if (!(Destination is Sugar) && CurrentLoad < MaximumLoad)
            {
                GoTo(markerInfo);
            }
        }

        protected override void OnFruitNeedsCarriers(MarkerInformation markerInfo)
        {
            if (!(Destination is Fruit) && CurrentLoad <= 0 && CarryingFruit == null)
            {
                GoTo(markerInfo);
            }
        }

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
