using AntMe.English;
using AntMe.Player.ArndtBalke.MarkerInfo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Behavior
{
    /// <summary>
    /// Behavior for gatherer ants.
    /// </summary>
    internal class GathererBehavior : BaseBehavior
    {
        private readonly List<Sugar> _listSugar = new List<Sugar>();

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
            Sugar s = _listSugar.OrderBy(o => Coordinate.GetDistanceBetween(_ant, o)).FirstOrDefault();

            if (s != null)
            {
                GoTo(s);
                return;
            }

            _ant.GoForward();
        }

        /// <summary>
        /// This method is called when an ant has travelled one third of its 
        /// movement range.
        /// Read more: "http://wiki.antme.net/en/API1:GettingTired"
        /// </summary>
        public override void GettingTired()
        {
        }

        /// <summary>
        /// This method is called if an ant dies. It informs you that the ant has 
        /// died. The ant cannot undertake any more actions from that point forward.
        /// Read more: "http://wiki.antme.net/en/API1:HasDied"
        /// </summary>
        /// <param name="kindOfDeath">Kind of Death</param>
        public override void HasDied(KindOfDeath kindOfDeath)
        {
        }

        /// <summary>
        /// This method is called in every simulation round, regardless of additional 
        /// conditions. It is ideal for actions that must be executed but that are not 
        /// addressed by other methods.
        /// Read more: "http://wiki.antme.net/en/API1:Tick"
        /// </summary>
        public override void Tick()
        {
            for (int i = 0; i < _listSugar.Count; i++)
            {
                if (_listSugar[i].Amount <= 0)
                    _listSugar.RemoveAt(i);
            }

            if (_ant.CarryingFruit != null && _ant.NeedsCarrier(_ant.CarryingFruit))
            {
                MarkFruitNeedsCarriers(_ant.CarryingFruit);
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

            if (_ant.NeedsCarrier(fruit))
            {
                MarkFruitNeedsCarriers(fruit);

                if (_ant.Destination == null || _ant.Destination is Marker)
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

            if (!_listSugar.Contains(sugar))
                _listSugar.Add(sugar);

            if (_ant.CarryingFruit == null && _ant.CurrentLoad < _ant.MaximumLoad)
            {
                if (_ant.Range - _ant.WalkedRange > _ant.DistanceToAnthill)
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
            if (_ant.NeedsCarrier(fruit))
            {
                _ant.Take(fruit);
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
            _ant.Take(sugar);
            GoToAnthill();
        }

        #endregion

        #region Communication

        protected override bool IgnoreMarker(Marker marker)
        {
            if (_ant.Destination == null)
            {
                if (_ant.Destination is Anthill)
                    return true;
                else if (_ant.Destination is Sugar)
                    return false;
                else if (_ant.DistanceToDestination < Coordinate.GetDistanceBetween(_ant, marker))
                    return true;
            }

            return false;
        }

        protected override void OnSugarSpotted(MarkerInformation markerInfo)
        {
            if (!(_ant.Destination is Sugar) && _ant.CurrentLoad < _ant.MaximumLoad)
            {
                GoTo(markerInfo);
            }
        }

        protected override void OnFruitNeedsCarriers(MarkerInformation markerInfo)
        {
            if (!(_ant.Destination is Fruit) && _ant.CurrentLoad <= 0 && _ant.CarryingFruit == null)
            {
                GoTo(markerInfo);
            }
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees an ant from the 
        /// same colony.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsFriend(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public override void SpotsFriend(Ant ant)
        {
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant detects an ant from a 
        /// friendly colony (an ant on the same team).
        /// Read more: "http://wiki.antme.net/en/API1:SpotsTeammate(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public override void SpotsTeammate(Ant ant)
        {
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
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if a 
        /// bug attacks; the ant can decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Bug)"
        /// </summary>
        /// <param name="bug">attacking bug</param>
        public override void UnderAttack(Bug bug)
        {
        }

        #endregion

    }
}
