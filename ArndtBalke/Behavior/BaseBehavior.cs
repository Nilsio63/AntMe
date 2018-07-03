using AntMe.English;
using AntMe.Player.ArndtBalke.MarkerInfo;
using System;

namespace AntMe.Player.ArndtBalke.Behavior
{
    /// <summary>
    /// Abstract class for ant behavior.
    /// </summary>
    internal abstract class BaseBehavior
    {
        #region Fields

        /// <summary>
        /// The ant to be controlled.
        /// </summary>
        protected readonly ArndtBalkeClass _ant;

        #endregion

        #region Properties

        /// <summary>
        /// The caste name for this behavior.
        /// </summary>
        public abstract string Caste { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Base constructor for all behaviors.
        /// </summary>
        /// <param name="ant">The ant to be controlled.</param>
        protected BaseBehavior(ArndtBalkeClass ant)
        {
            // Save ant reference
            _ant = ant;
        }

        #endregion

        #region Movement

        /// <summary>
        /// If the ant has no assigned tasks, it waits for new tasks. This method 
        /// is called to inform you that it is waiting.
        /// Read more: "http://wiki.antme.net/en/API1:Waiting"
        /// </summary>
        public virtual void Waiting()
        {
        }

        /// <summary>
        /// This method is called when an ant has travelled one third of its 
        /// movement range.
        /// Read more: "http://wiki.antme.net/en/API1:GettingTired"
        /// </summary>
        public virtual void GettingTired()
        {
        }

        /// <summary>
        /// This method is called if an ant dies. It informs you that the ant has 
        /// died. The ant cannot undertake any more actions from that point forward.
        /// Read more: "http://wiki.antme.net/en/API1:HasDied"
        /// </summary>
        /// <param name="kindOfDeath">Kind of Death</param>
        public virtual void HasDied(KindOfDeath kindOfDeath)
        {
        }

        /// <summary>
        /// This method is called in every simulation round, regardless of additional 
        /// conditions. It is ideal for actions that must be executed but that are not 
        /// addressed by other methods.
        /// Read more: "http://wiki.antme.net/en/API1:Tick"
        /// </summary>
        public virtual void Tick()
        {
        }

        protected void GoToAnthill()
        {
            _ant.GoToAnthill();
        }

        protected void GoTo(Item item)
        {
            _ant.GoToDestination(item);
        }

        protected void GoTo(MarkerInformation markerInfo)
        {
            _ant.GoTo(markerInfo.Coordinates);
        }

        #endregion

        #region Food

        /// <summary>
        /// This method is called as soon as an ant sees an apple within its 360° 
        /// visual range. The parameter is the piece of fruit that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Fruit)"
        /// </summary>
        /// <param name="fruit">spotted fruit</param>
        public virtual void Spots(Fruit fruit)
        {
            if (_ant.NeedsCarrier(fruit))
            {
                MarkFruitNeedsCarriers(fruit);
            }
        }

        /// <summary>
        /// This method is called as soon as an ant sees a mound of sugar in its 360° 
        /// visual range. The parameter is the mound of sugar that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Sugar)"
        /// </summary>
        /// <param name="sugar">spotted sugar</param>
        public virtual void Spots(Sugar sugar)
        {
            if (sugar.Amount > _ant.MaximumLoad * 4)
            {
                MarkSugarSpotted(sugar);
            }
        }

        /// <summary>
        /// If the ant’s destination is a piece of fruit, this method is called as soon 
        /// as the ant reaches its destination. It means that the ant is now near enough 
        /// to its destination/target to interact with it.
        /// Read more: "http://wiki.antme.net/en/API1:DestinationReached(Fruit)"
        /// </summary>
        /// <param name="fruit">reached fruit</param>
        public virtual void DestinationReached(Fruit fruit)
        {
        }

        /// <summary>
        /// If the ant’s destination is a mound of sugar, this method is called as soon 
        /// as the ant has reached its destination. It means that the ant is now near 
        /// enough to its destination/target to interact with it.
        /// Read more: "http://wiki.antme.net/en/API1:DestinationReached(Sugar)"
        /// </summary>
        /// <param name="sugar">reached sugar</param>
        public virtual void DestinationReached(Sugar sugar)
        {
        }

        #endregion

        #region Communication

        /// <summary>
        /// Friendly ants can detect markers left by other ants. This method is called 
        /// when an ant smells a friendly marker for the first time.
        /// Read more: "http://wiki.antme.net/en/API1:DetectedScentFriend(Marker)"
        /// </summary>
        /// <param name="marker">marker</param>
        public virtual void DetectedScentFriend(Marker marker)
        {
            MarkerInformation markerInfo = new MarkerInformation(marker.Information);

            if (IgnoreMarker(marker))
                return;

            switch (markerInfo.InfoType)
            {
                case 0:
                    OnBugSpotted(markerInfo);
                    break;
                case 1:
                    OnEnemyAntSpotted(markerInfo);
                    break;
                case 2:
                    OnSugarSpotted(markerInfo);
                    break;
                case 3:
                    OnFruitNeedsCarriers(markerInfo);
                    break;
            }
        }

        protected virtual bool IgnoreMarker(Marker marker)
        {
            return false;
        }

        protected virtual void OnBugSpotted(MarkerInformation markerInfo)
        {

        }

        protected virtual void OnEnemyAntSpotted(MarkerInformation markerInfo)
        {

        }

        protected virtual void OnSugarSpotted(MarkerInformation markerInfo)
        {

        }

        protected virtual void OnFruitNeedsCarriers(MarkerInformation markerInfo)
        {

        }

        private void MakeMark(byte infoType, int range)
        {
            _ant.MakeMark(new MarkerInformation(infoType).Encode(), range);
        }

        protected virtual void MarkBugSpotted(Bug bug)
        {
            MakeMark(0, 75);
        }

        protected virtual void MarkEnemyAntSpotted(Ant ant)
        {
            MakeMark(1, 75);
        }

        protected virtual void MarkSugarSpotted(Sugar sugar)
        {
            MakeMark(2, 50);
        }

        protected virtual void MarkFruitNeedsCarriers(Fruit fruit)
        {
            MakeMark(3, 200);
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees an ant from the 
        /// same colony.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsFriend(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public virtual void SpotsFriend(Ant ant)
        {
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant detects an ant from a 
        /// friendly colony (an ant on the same team).
        /// Read more: "http://wiki.antme.net/en/API1:SpotsTeammate(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public virtual void SpotsTeammate(Ant ant)
        {
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
        public virtual void SpotsEnemy(Ant ant)
        {
            MarkEnemyAntSpotted(ant);
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees a bug.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Bug)"
        /// </summary>
        /// <param name="bug">spotted bug</param>
        public virtual void SpotsEnemy(Bug bug)
        {
            MarkBugSpotted(bug);
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if an 
        /// enemy ant attacks; the ant can then decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Ant)"
        /// </summary>
        /// <param name="ant">attacking ant</param>
        public virtual void UnderAttack(Ant ant)
        {
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if a 
        /// bug attacks; the ant can decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Bug)"
        /// </summary>
        /// <param name="bug">attacking bug</param>
        public virtual void UnderAttack(Bug bug)
        {
        }

        #endregion

    }
}
