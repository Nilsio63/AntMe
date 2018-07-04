﻿using AntMe.English;
using AntMe.Player.ArndtBalke.Map;
using AntMe.Player.ArndtBalke.MarkerInfo;
using System;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke.Behavior
{
    /// <summary>
    /// Abstract class for ant behavior.
    /// </summary>
    internal abstract class BaseBehavior
    {
        protected readonly List<Sugar> _listSugar = new List<Sugar>();
        protected readonly List<Fruit> _listFruit = new List<Fruit>();
        protected readonly List<Bug> _listBugs = new List<Bug>();
        protected readonly List<Ant> _listEnemyAnts = new List<Ant>();

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
            if (_ant.CurrentEnergy < _ant.MaximumEnergy * 0.15)
            {
                GoToAnthill();
                return;
            }
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

            for (int i = 0; i < _listSugar.Count; i++)
            {
                if (_listSugar[i].Amount <= 0)
                    _listSugar.RemoveAt(i);
            }

            for (int i = 0; i < _listFruit.Count; i++)
            {
                if (_listFruit[i].Amount <= 0)
                    _listFruit.RemoveAt(i);
            }

            for (int i = 0; i < _listBugs.Count; i++)
            {
                if (_listBugs[i].CurrentEnergy <= 0)
                    _listBugs.RemoveAt(0);
            }

            for (int i = 0; i < _listEnemyAnts.Count; i++)
            {
                if (_listEnemyAnts[i].CurrentEnergy <= 0)
                    _listEnemyAnts.RemoveAt(i);
            }

            if (Range - WalkedRange - Range * 0.02 < GetDistanceTo(Anthill))
            {
                GoToAnthill();
            }
        }

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

        public void GoTo(RelativeCoordinate coordinate)
        {
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
            if (Anthill == null)
                return null;

            return new RelativeCoordinate(Anthill, item);
        }

        public RelativeCoordinate GetCoordinate()
        {
            if (Anthill == null)
                return null;

            return new RelativeCoordinate(Anthill, _ant);
        }

        public int GetDistanceTo(RelativeCoordinate coordinate)
        {
            RelativeCoordinate ownCoordinate = GetCoordinate();

            if (coordinate == null || ownCoordinate == null)
                return -1;

            int x = ownCoordinate.X - coordinate.X;
            int y = ownCoordinate.Y - coordinate.Y;

            double distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            return (int)distance;
        }

        protected int GetDistanceTo(Item item)
        {
            return item == null ? -1 : Coordinate.GetDistanceBetween(_ant, item);
        }

        #endregion

        #region Food

        protected bool NeedsCarrier()
        {
            return NeedsCarrier(_ant.CarryingFruit);
        }

        protected bool NeedsCarrier(Fruit fruit)
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
            if (!_listFruit.Contains(fruit))
                _listFruit.Add(fruit);

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
            if (!_listSugar.Contains(sugar))
                _listSugar.Add(sugar);

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
            MarkerInformation markerInfo = new MarkerInformation(marker.Information);

            MakeMark(markerInfo);

            if (IgnoreMarker(markerInfo))
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

        protected virtual bool IgnoreMarker(MarkerInformation markerInfo)
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

        protected void MakeMark(byte infoType, RelativeCoordinate coordinate, int range)
        {
            _ant.MakeMark(new MarkerInformation(infoType, coordinate).Encode(), range);
        }

        private void MakeMark(MarkerInformation markerInfo)
        {
            if (markerInfo.HopCount < 4 && markerInfo.Coordinates != null)
            {
                _ant.MakeMark(new MarkerInformation(markerInfo).Encode(), 75);
            }
        }

        protected virtual void MarkBugSpotted(Bug bug)
        {
            MakeMark(0, GetCoordinate(bug), 75);
        }

        protected virtual void MarkEnemyAntSpotted(Ant ant)
        {
            MakeMark(1, GetCoordinate(ant), 75);
        }

        protected virtual void MarkSugarSpotted(Sugar sugar)
        {
            MakeMark(2, GetCoordinate(sugar), 50);
        }

        protected virtual void MarkFruitNeedsCarriers(Fruit fruit)
        {
            MakeMark(3, GetCoordinate(fruit), 200);
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
            if (!_listEnemyAnts.Contains(ant))
                _listEnemyAnts.Add(ant);

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
            if (!_listBugs.Contains(bug))
                _listBugs.Add(bug);

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
