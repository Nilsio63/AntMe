using AntMe.English;
using AntMe.Player.ArndtBalke.Cache;
using AntMe.Player.ArndtBalke.Map;
using AntMe.Player.ArndtBalke.Markers;
using System;
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

        private bool initialized = false;

        #endregion

        #region Properties

        /// <summary>
        /// The caste name for this behavior.
        /// </summary>
        public abstract string Caste { get; }

        protected Anthill Anthill { get; private set; }

        protected byte BugSpotted => 0;
        protected byte AntSpotted => 1;
        protected byte SugarSpotted => 2;
        protected byte FruitNeedsCarriers => 3;
        protected byte FruitNeedsProtection => 4;
        protected byte AttackPoint => 5;

        protected int Range => _ant.Range;

        protected int WalkedRange => _ant.WalkedRange;

        protected int ViewRange => _ant.Viewrange;

        protected int FriendlyAntsFromSameCasteInViewrange => _ant.FriendlyAntsFromSameCasteInViewrange;

        protected Item Destination => _ant.Destination;

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

            _cache = new MemoryCache();
        }

        private void Init()
        {
            _ant.GoToAnthill();

            Anthill = (Anthill)Destination;

            initialized = true;
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
            if (!initialized)
                return;

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
            if (!initialized)
            {
                Init();
            }

            _cache.Cleanup();

            if (Range - WalkedRange - Range * 0.02 < GetDistanceTo(Anthill)
                || _ant.CurrentEnergy < _ant.MaximumEnergy * 0.15)
            {
                GoToAnthill();
                return;
            }

            Target nextTarget = GetNextTarget();

            if (nextTarget != null)
            {
                GoTo(nextTarget);
            }

            Signal nextSignal = GetNextSignal();

            if (nextSignal != null)
            {
                EmitSignal(nextSignal);
            }
        }

        protected virtual Target GetNextTarget()
        {
            if (Range - WalkedRange - Range * 0.02 < GetDistanceTo(Anthill)
                || _ant.CurrentEnergy < _ant.MaximumEnergy * 0.15)
            {
                return new Target(Anthill);
            }

            return null;
        }

        protected virtual Signal GetNextSignal()
        {
            Fruit nearestFruit = _cache.Fruits.Where(InViewRange).OrderBy(GetDistanceTo).FirstOrDefault();

            if (nearestFruit != null)
            {
                if (_ant.ForeignAntsInViewrange > 2)
                    return new Signal(FruitNeedsProtection, GetCoordinate(nearestFruit));
            }

            return null;
        }

        protected void GoForward()
        {
            _ant.GoForward();
        }

        protected void GoToAnthill()
        {
            GoTo(Anthill);
        }

        protected void GoTo(Target target)
        {
            if (target.Item != null)
            {
                if (target.Item is Insect i)
                    Attack(i);
                else
                    GoTo(target.Item);
            }
            else
                GoTo(target.Signal);
        }

        protected void GoTo(Item item)
        {
            _ant.GoToDestination(item);
        }

        protected void GoTo(Signal signal)
        {
            GoTo(signal.Coordinates);
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

            GoTo((int)degree, (int)distance);
        }

        protected void GoTo(int degree, int distance)
        {
            _ant.TurnToDirection(degree);
            _ant.GoForward(distance);
        }

        protected RelativeCoordinate GetCoordinate(Item item)
        {
            return new RelativeCoordinate(Anthill, item);
        }

        protected RelativeCoordinate GetCoordinate()
        {
            return new RelativeCoordinate(Anthill, _ant);
        }

        protected int GetDistanceTo(Target target)
        {
            if (target.Item != null)
                return GetDistanceTo(target.Item);
            else
                return GetDistanceTo(target.Signal);
        }

        protected int GetDistanceTo(Item item)
        {
            return Coordinate.GetDistanceBetween(_ant, item);
        }

        protected int GetDistanceTo(Signal signal)
        {
            return GetDistanceTo(signal.Coordinates);
        }

        protected int GetDistanceTo(RelativeCoordinate coordinate)
        {
            return GetCoordinate().GetDistanceTo(coordinate);
        }

        protected bool InViewRange(Item item)
        {
            return GetDistanceTo(item) <= ViewRange;
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

            if (!initialized)
                return;

            if (NeedsCarrier(fruit))
            {
                EmitSignal(FruitNeedsCarriers, fruit);
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

            if (!initialized)
                return;

            if (sugar.Amount > MaximumLoad * 8)
            {
                EmitSignal(SugarSpotted, sugar);
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
            if (!initialized)
                return;

            Signal signal = new Signal(marker.Information);

            if (signal.HopCount < 1 && !_cache.Signals.Contains(signal))
                EmitSignal(new Signal(signal));

            _cache.Add(signal);
        }

        protected void EmitSignal(byte infoType, Item item)
        {
            EmitSignal(new Signal(infoType, GetCoordinate(item)));
        }

        private void EmitSignal(Signal signal)
        {
            int range;

            if (signal.InfoType == SugarSpotted)
                range = 50;
            else if (signal.InfoType == FruitNeedsCarriers
                || signal.InfoType == FruitNeedsProtection)
                range = 200;
            else if (signal.InfoType == AttackPoint)
                range = 250;
            else
                range = 75;

            _ant.MakeMark(signal.Encode(), range);
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

            if (!initialized)
                return;

            if (ant.CarriedFruit != null || ant.CurrentLoad > 0)
                EmitSignal(AntSpotted, ant);
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

            if (!initialized)
                return;

            EmitSignal(BugSpotted, bug);
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
