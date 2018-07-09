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
        #region Fields

        /// <summary>
        /// All known memories of the ant's mind.
        /// </summary>
        protected readonly MemoryCache _cache = new MemoryCache();

        /// <summary>
        /// The ant to be controlled.
        /// </summary>
        private readonly ArndtBalkeClass _ant;

        /// <summary>
        /// Helper variable to check if behavior is initialized and can work properly.
        /// </summary>
        private bool initialized = false;

        #endregion

        #region Properties

        /// <summary>
        /// The caste name for this behavior.
        /// </summary>
        public abstract string Caste { get; }

        /// <summary>
        /// The ant's home anthill for coordinate references.
        /// </summary>
        protected Anthill Anthill { get; private set; }

        /// <summary>
        /// Signal's value for 'bug spotted'.
        /// </summary>
        protected byte BugSpotted => 0;
        /// <summary>
        /// Signal's value for 'ant spotted'.
        /// </summary>
        protected byte AntSpotted => 1;
        /// <summary>
        /// Signal's value for 'sugar spotted'.
        /// </summary>
        protected byte SugarSpotted => 2;
        /// <summary>
        /// Signal's value for 'fruit needs carriers'.
        /// </summary>
        protected byte FruitNeedsCarriers => 3;
        /// <summary>
        /// Signal's value for 'fruit needs protection'.
        /// </summary>
        protected byte FruitNeedsProtection => 4;

        /// <summary>
        /// The ant's maximum walking range.
        /// </summary>
        protected int MaximumRange => _ant.Range;
        /// <summary>
        /// The ant's currently walked range.
        /// </summary>
        protected int WalkedRange => _ant.WalkedRange;
        /// <summary>
        /// The ant's view range.
        /// </summary>
        protected int ViewRange => _ant.Viewrange;
        /// <summary>
        /// The ant's current destination.
        /// </summary>
        protected Item Destination => _ant.Destination;
        /// <summary>
        /// The ant's currently carried fruit.
        /// </summary>
        protected Fruit CarryingFruit => _ant.CarryingFruit;
        /// <summary>
        /// The ant's maximum load.
        /// </summary>
        protected int MaximumLoad => _ant.MaximumLoad;
        /// <summary>
        /// The ant's current load.
        /// </summary>
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

        /// <summary>
        /// Initializes the behavior instance.
        /// </summary>
        private void Init()
        {
            // Set destination to anthill
            _ant.GoToAnthill();

            // Read anthill from destination
            Anthill = (Anthill)Destination;

            // Mark as initialized
            initialized = true;
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
            // Abort when not initialized
            if (!initialized)
                return;

            // Walk forward
            _ant.GoForward();
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
            // Initialize if not yet done
            if (!initialized)
                Init();

            // Clean up cache
            _cache.Cleanup();

            // Return to anthill if remaining distance is barely enough to reach anthill or ant low on health
            if (MaximumRange - WalkedRange - MaximumRange * 0.02 < GetDistanceTo(Anthill)
                || _ant.CurrentEnergy < _ant.MaximumEnergy * 0.15)
            {
                GoToAnthill();
                return;
            }

            // Calculate next target and move to it if set
            Target nextTarget = GetNextTarget();

            if (nextTarget != null)
                GoTo(nextTarget);

            // Calculate next signal and emit it if set
            Signal nextSignal = GetNextSignal();

            if (nextSignal != null)
                EmitSignal(nextSignal);
        }

        /// <summary>
        /// Function to calculate the next target on each tick.
        /// </summary>
        /// <returns>Returns the next target or null.</returns>
        protected virtual Target GetNextTarget()
        {
            // Return to anthill if remaining distance is barely enough to reach anthill or ant low on health
            if (MaximumRange - WalkedRange - MaximumRange * 0.02 < GetDistanceTo(Anthill)
                || _ant.CurrentEnergy < _ant.MaximumEnergy * 0.15)
            {
                return new Target(Anthill);
            }

            // Return no target
            return null;
        }

        /// <summary>
        /// Function to calculate the next signal on each tick.
        /// </summary>
        /// <returns>Returns the next signal or null.</returns>
        protected virtual Signal GetNextSignal()
        {
            // Search nearest fruit in view range
            Fruit nearestFruit = _cache.Fruits.Where(InViewRange).OrderBy(GetDistanceTo).FirstOrDefault();
            
            if (nearestFruit != null)
            {
                // Create protection request signal if hostile ants are in view range
                if (_ant.ForeignAntsInViewrange > 2)
                    return new Signal(FruitNeedsProtection, GetCoordinate(nearestFruit));
            }

            // Return no signal
            return null;
        }

        /// <summary>
        /// Let's the ant move to it's anthill.
        /// </summary>
        protected void GoToAnthill()
        {
            // Move to anthill if set
            if (Anthill != null)
                GoTo(Anthill);
        }

        /// <summary>
        /// Let's the ant move to the given target.
        /// </summary>
        /// <param name="target">The ant's next target.</param>
        protected void GoTo(Target target)
        {
            // Check if item is given
            if (target.Item != null)
            {
                // Attack insects and go to all other items
                if (target.Item is Insect i)
                    Attack(i);
                else
                    GoTo(target.Item);
            }
            else
            {
                // Move to signal's coordinates
                GoTo(target.Signal.Coordinates);
            }
        }

        /// <summary>
        /// Let's the ant move to the given item.
        /// </summary>
        /// <param name="item">The ant's next destination.</param>
        protected void GoTo(Item item)
        {
            // Move ant to item
            _ant.GoToDestination(item);
        }

        /// <summary>
        /// Let's the ant move to the given coordinates.
        /// </summary>
        /// <param name="coordinate">The ant's next coordinates.</param>
        protected void GoTo(RelativeCoordinate coordinate)
        {
            // Return if reference anthill not given
            if (Anthill == null)
                return;

            // Get own coordinates
            RelativeCoordinate ownCoordinate = GetCoordinate();

            if (coordinate == null || ownCoordinate == null)
                return;

            // Get Coordinate differences
            double x = coordinate.X - ownCoordinate.X;
            double y = coordinate.Y - ownCoordinate.Y;

            // Calculate distance
            double distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            if (distance == 0)
                return;

            // Calculate degree
            double degree = Math.Atan(y / x) * 180 / Math.PI;
            degree += 360;
            if (x < 0)
                degree += 180;
            degree %= 360;

            // Go to distance on degree
            GoTo((int)degree, (int)distance);
        }

        /// <summary>
        /// Let's the ant walk the given distance in the given degree.
        /// </summary>
        /// <param name="degree">The degree the ant has to turn to.</param>
        /// <param name="distance">The distance to be walked.</param>
        protected void GoTo(int degree, int distance)
        {
            // Turn and walk
            _ant.TurnToDirection(degree);
            _ant.GoForward(distance);
        }

        /// <summary>
        /// Gets the coordinates of the given item.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns>Returns the relative coordinates of the item.</returns>
        protected RelativeCoordinate GetCoordinate(Item item)
        {
            // Create coordinates from anthill to item
            return new RelativeCoordinate(Anthill, item);
        }

        /// <summary>
        /// Gets the ant's own coordinates.
        /// </summary>
        /// <returns>Returns the relative coordinates of the ant.</returns>
        protected RelativeCoordinate GetCoordinate()
        {
            // Create coordinates from anthill to ant
            return new RelativeCoordinate(Anthill, _ant);
        }

        /// <summary>
        /// Gets the distance to the given target.
        /// </summary>
        /// <param name="target">The target to be checked.</param>
        /// <returns>Returns the distance to the given target.</returns>
        protected int GetDistanceTo(Target target)
        {
            // Get distance to given item or signal (depending on which is not null)
            if (target.Item != null)
                return GetDistanceTo(target.Item);
            else
                return GetDistanceTo(target.Signal);
        }

        /// <summary>
        /// Gets the distance to the given item.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns>Returns the distance to the given item.</returns>
        protected int GetDistanceTo(Item item)
        {
            // Calculate distance via antme coordinate class
            return Coordinate.GetDistanceBetween(_ant, item);
        }

        /// <summary>
        /// Gets the distance to the given signal.
        /// </summary>
        /// <param name="signal">The signal to be checked.</param>
        /// <returns>Returns the distance to the given signal.</returns>
        protected int GetDistanceTo(Signal signal)
        {
            // Return distance to signal's coordinates
            return GetDistanceTo(signal.Coordinates);
        }

        /// <summary>
        /// Gets the distance to the given coordinates.
        /// </summary>
        /// <param name="item">The coordinates to be checked.</param>
        /// <returns>Returns the distance to the given coordinates.</returns>
        protected int GetDistanceTo(RelativeCoordinate coordinate)
        {
            // Calculate distance from own coordinates to given coordinates
            return GetCoordinate().GetDistanceTo(coordinate);
        }

        /// <summary>
        /// Checks if the given item is still in view range.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns>Returns true if the item is in view range.</returns>
        protected bool InViewRange(Item item)
        {
            // Check if distance to item is smaller than or equal to view range
            return GetDistanceTo(item) <= ViewRange;
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
            // Abort if not initialized
            if (!initialized)
                return;

            // Decode signal from information
            Signal signal = new Signal(marker.Information);

            // Re-emit signal if not re-emitted too often or already in cache
            if (signal.HopCount < 1 && !_cache.Signals.Contains(signal))
                EmitSignal(new Signal(signal));

            // Remember signal
            _cache.Add(signal);
        }

        /// <summary>
        /// Emits the a signal with the given information type for the given object.
        /// </summary>
        /// <param name="infoType">The info type for the signal to be emitted.</param>
        /// <param name="item">The item of the signal to be sent.</param>
        protected void EmitSignal(byte infoType, Item item)
        {
            // Create new signal from info type and coordinates and emit it
            EmitSignal(new Signal(infoType, GetCoordinate(item)));
        }

        /// <summary>
        /// Emits the given signal in a radius depending on the info type.
        /// </summary>
        /// <param name="signal">The signal to be emitted.</param>
        private void EmitSignal(Signal signal)
        {
            int range;

            // Set range depending on info type
            if (signal.InfoType == SugarSpotted)
                range = 50;
            else if (signal.InfoType == FruitNeedsCarriers
                || signal.InfoType == FruitNeedsProtection)
                range = 200;
            else
                range = 75;

            // Emit a mark with the encoded signal in the given range
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

        #region Food

        /// <summary>
        /// Checks if the currently carried fruit needs more carriers.
        /// </summary>
        /// <returns>Returns true if the currently carried fruit needs more carriers.</returns>
        protected bool NeedsCarrier()
        {
            // Check if currently carried fruit needs more carriers
            return NeedsCarrier(CarryingFruit);
        }

        /// <summary>
        /// Checks if the given fruit needs more carriers.
        /// </summary>
        /// <param name="fruit">The fruit to be checked.</param>
        /// <returns>Returns true if the given fruit needs more carriers.</returns>
        public bool NeedsCarrier(Fruit fruit)
        {
            // Return true if fruit is given and needs carriers
            return fruit != null
                && _ant.NeedsCarrier(fruit);
        }

        /// <summary>
        /// Method to take the given fruit.
        /// </summary>
        /// <param name="food">The fruit to be taken.</param>
        protected void Take(Food food)
        {
            // Let ant take the food
            _ant.Take(food);
        }

        /// <summary>
        /// Method to drop the currently loaded food.
        /// </summary>
        protected void Drop()
        {
            // Let ant drop food
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
            // Remember fruit
            _cache.Add(fruit);

            // Abort if not initiaized
            if (!initialized)
                return;

            // Emit signal for fruit if it needs carriers
            if (NeedsCarrier(fruit))
                EmitSignal(FruitNeedsCarriers, fruit);
        }

        /// <summary>
        /// This method is called as soon as an ant sees a mound of sugar in its 360° 
        /// visual range. The parameter is the mound of sugar that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Sugar)"
        /// </summary>
        /// <param name="sugar">spotted sugar</param>
        public virtual void Spots(Sugar sugar)
        {
            // Remember sugar
            _cache.Add(sugar);

            // Abort if not initiaized
            if (!initialized)
                return;

            // Emit signal if amount of sugar is not too low
            if (sugar.Amount > MaximumLoad * 8)
                EmitSignal(SugarSpotted, sugar);
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

        #region Fight

        /// <summary>
        /// Attacks the given insect.
        /// </summary>
        /// <param name="insect">The insect target.</param>
        protected void Attack(Insect insect)
        {
            // Go to insect if not in view range, else attack it
            if (!InViewRange(insect))
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
            // Remember ant
            _cache.Add(ant);

            // Abort if not initialized
            if (!initialized)
                return;

            // Emit signal if ant is carrying something
            if (ant.CarriedFruit != null && ant.CurrentLoad > 0)
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
            // Remember bug
            _cache.Add(bug);

            // Abort if not initialized
            if (!initialized)
                return;

            // Emit signal for bug
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
