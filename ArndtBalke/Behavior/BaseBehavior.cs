using AntMe.English;
using AntMe.Player.ArndtBalke.Cache;
using AntMe.Player.ArndtBalke.Map;
using AntMe.Player.ArndtBalke.MarkerInfo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Behavior
{
    /// <summary>
    /// Abstract class for ant behavior.
    /// </summary>
    internal abstract class BaseBehavior
    {
        protected readonly MemoryCache _cache;

        #region Fields

        /// <summary>
        /// The ant to be controlled.
        /// </summary>
        private readonly ArndtBalkeClass _ant;

        #endregion

        #region Properties

        /// <summary>
        /// The caste name for this behavior.
        /// </summary>
        public abstract string Caste { get; }

        protected Anthill Anthill { get; private set; }

        protected int Range => _ant.Range;

        protected int WalkedRange => _ant.WalkedRange;

        protected int ViewRange => _ant.Viewrange;

        protected int FriendlyAntsFromSameCasteInViewrange => _ant.FriendlyAntsFromSameCasteInViewrange;

        protected Item Destination =>  _ant.Destination;

        protected Fruit CarryingFruit => _ant.CarryingFruit;

        protected int MaximumLoad => _ant.MaximumLoad;

        protected int CurrentLoad => _ant.CurrentLoad;

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

            _cache = new MemoryCache(this);
        }

        #endregion

        #region Movement

        protected void Think(string message)
        {
            _ant.Think(message);
        }

        /// <summary>
        /// If the ant has no assigned tasks, it waits for new tasks. This method 
        /// is called to inform you that it is waiting.
        /// Read more: "http://wiki.antme.net/en/API1:Waiting"
        /// </summary>
        public virtual void Waiting()
        {
            GoForward();
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
            if (Anthill == null)
            {
                GoToAnthill();

                Anthill = Destination as Anthill;
            }

            _cache.Cleanup();

            if (Range - WalkedRange - Range * 0.02 < GetDistanceTo(Anthill)
                || _ant.CurrentEnergy < _ant.MaximumEnergy * 0.15)
            {
                GoToAnthill();
                return;
            }

            DoNextMove();
        }

        protected abstract void DoNextMove();

        protected void GoForward()
        {
            _ant.GoForward();
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
            GoTo(markerInfo.Coordinates);
        }

        protected void GoTo(RelativeCoordinate coordinate)
        {
            if (Anthill == null)
                return;

            RelativeCoordinate ownCoordinate = GetCoordinate();

            if (coordinate == null || ownCoordinate == null)
                return;

            double x = coordinate.X - ownCoordinate.X;
            double y = coordinate.Y - ownCoordinate.Y;

            double distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            if (distance == 0)
                return;

            double degree = Math.Atan(y / x) * 180 / Math.PI;
            degree += 360;
            if (x < 0)
                degree += 180;
            degree %= 360;

            _ant.TurnToDirection((int)degree);
            _ant.GoForward((int)distance);
        }

        public RelativeCoordinate GetCoordinate(Item item)
        {
            return new RelativeCoordinate(Anthill, item);
        }

        public RelativeCoordinate GetCoordinate()
        {
            return new RelativeCoordinate(Anthill, _ant);
        }

        public int GetDistanceTo(RelativeCoordinate coordinate)
        {
            RelativeCoordinate ownCoordinate = GetCoordinate();

            return ownCoordinate != null ? ownCoordinate.GetDistanceTo(coordinate) : -1;
        }

        public int GetDistanceTo(MarkerInformation markerInfo)
        {
            return GetDistanceTo(markerInfo.Coordinates);
        }

        public int GetDistanceTo(Item item)
        {
            return item == null ? -1 : Coordinate.GetDistanceBetween(_ant, item);
        }

        #endregion

        #region Food

        protected bool NeedsCarrier()
        {
            return NeedsCarrier(_ant.CarryingFruit);
        }

        public bool NeedsCarrier(Fruit fruit)
        {
            return fruit != null
                && _ant.NeedsCarrier(fruit);
        }

        protected void Take(Food food)
        {
            _ant.Take(food);
        }

        protected void Drop()
        {
            _ant.Drop();
        }

        /// <summary>
        /// This method is called as soon as an ant sees an apple within its 360° 
        /// visual range. The parameter is the piece of fruit that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Fruit)"
        /// </summary>
        /// <param name="fruit">spotted fruit</param>
        public virtual void Spots(Fruit fruit)
        {
            _cache.Add(fruit);

            if (NeedsCarrier(fruit))
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
            _cache.Add(sugar);

            if (sugar.Amount > MaximumLoad * 4)
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
        public void DetectedScentFriend(Marker marker)
        {
            if (Anthill == null)
                return;

            MarkerInformation markerInfo = new MarkerInformation(marker.Information);

            if (markerInfo.HopCount < 2 && !_cache.Markers.Contains(markerInfo))
                MakeMark(new MarkerInformation(markerInfo), 90);

            _cache.Add(markerInfo);
        }

        protected void MakeMark(byte infoType, Item item, int range)
        {
            MakeMark(new MarkerInformation(infoType, GetCoordinate(item)), range);
        }

        private void MakeMark(MarkerInformation markerInfo, int range)
        {
            _ant.MakeMark(markerInfo.Encode(), range);
        }

        protected virtual void MarkBugSpotted(Bug bug)
        {
            if (Anthill == null)
                return;

            MakeMark(0, bug, 75);
        }

        protected virtual void MarkEnemyAntSpotted(Ant ant)
        {
            if (Anthill == null)
                return;

            MakeMark(1, ant, 75);
        }

        protected virtual void MarkSugarSpotted(Sugar sugar)
        {
            if (Anthill == null)
                return;

            MakeMark(2, sugar, 50);
        }

        protected virtual void MarkFruitNeedsCarriers(Fruit fruit)
        {
            if (Anthill == null)
                return;

            MakeMark(3, fruit, 200);
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

        protected void Attack(Insect insect)
        {
            if (GetDistanceTo(insect) > ViewRange)
                GoTo(insect);
            else
                _ant.Attack(insect);
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant detects an ant from an 
        /// enemy colony.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public virtual void SpotsEnemy(Ant ant)
        {
            _cache.Add(ant);

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
            _cache.Add(bug);

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
