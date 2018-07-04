using AntMe.English;
using AntMe.Player.ArndtBalke.Behavior;
using AntMe.Player.ArndtBalke.Map;
using System;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke
{
    [Player(
        ColonyName = "ArndtBalke",
        FirstName = "",
        LastName = ""
    )]
    [Caste(
        Name = "Gatherer",
        AttackModifier = -1,
        EnergyModifier = -1,
        LoadModifier = 1,
        RangeModifier = 0,
        RotationSpeedModifier = 0,
        SpeedModifier = 0,
        ViewRangeModifier = 1
    )]
    [Caste(
        Name = "Hunter",
        AttackModifier = 1,
        EnergyModifier = 1,
        LoadModifier = -1,
        RangeModifier = -1,
        RotationSpeedModifier = 0,
        SpeedModifier = 0,
        ViewRangeModifier = 0
    )]
    public class ArndtBalkeClass : BaseAnt
    {
        #region Fields

        /// <summary>
        /// The behavior of the ant.
        /// </summary>
        private BaseBehavior behavior;

        #endregion

        #region Caste

        /// <summary>
        /// Every time that a new ant is born, its job group must be set. You can 
        /// do so with the help of the value returned by this method.
        /// Read more: "http://wiki.antme.net/en/API1:ChooseCaste"
        /// </summary>
        /// <param name="typeCount">Number of ants for every caste</param>
        /// <returns>Caste-Name for the next ant</returns>
        public override string ChooseCaste(Dictionary<string, int> typeCount)
        {
            //// Check caste counts
            //if (typeCount["Hunter"] < typeCount["Gatherer"])
            //{
            //    // Create hunter behavior
            //    behavior = new HunterBehavior(this);
            //}
            //else
            //{
            //    // Create gatherer behavior
            //    behavior = new GathererBehavior(this);
            //}

            // Create gatherer behavior
            behavior = new GathererBehavior(this);

            // Return Caste of behavior
            return behavior.Caste;
        }

        #endregion

        #region Movement

        /// <summary>
        /// If the ant has no assigned tasks, it waits for new tasks. This method 
        /// is called to inform you that it is waiting.
        /// Read more: "http://wiki.antme.net/en/API1:Waiting"
        /// </summary>
        public override void Waiting()
        {
            // Call behavior
            behavior.Waiting();
        }

        /// <summary>
        /// This method is called when an ant has travelled one third of its 
        /// movement range.
        /// Read more: "http://wiki.antme.net/en/API1:GettingTired"
        /// </summary>
        public override void GettingTired()
        {
            // Call behavior
            behavior.GettingTired();
        }

        /// <summary>
        /// This method is called if an ant dies. It informs you that the ant has 
        /// died. The ant cannot undertake any more actions from that point forward.
        /// Read more: "http://wiki.antme.net/en/API1:HasDied"
        /// </summary>
        /// <param name="kindOfDeath">Kind of Death</param>
        public override void HasDied(KindOfDeath kindOfDeath)
        {
            // Call behavior
            behavior.HasDied(kindOfDeath);
        }

        /// <summary>
        /// This method is called in every simulation round, regardless of additional 
        /// conditions. It is ideal for actions that must be executed but that are not 
        /// addressed by other methods.
        /// Read more: "http://wiki.antme.net/en/API1:Tick"
        /// </summary>
        public override void Tick()
        {
            // Call behavior
            behavior.Tick();
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
            // Call behavior
            behavior.Spots(fruit);
        }

        /// <summary>
        /// This method is called as soon as an ant sees a mound of sugar in its 360° 
        /// visual range. The parameter is the mound of sugar that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Sugar)"
        /// </summary>
        /// <param name="sugar">spotted sugar</param>
        public override void Spots(Sugar sugar)
        {
            // Call behavior
            behavior.Spots(sugar);
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
            // Call behavior
            behavior.DestinationReached(fruit);
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
            // Call behavior
            behavior.DestinationReached(sugar);
        }

        #endregion

        #region Communication

        /// <summary>
        /// Friendly ants can detect markers left by other ants. This method is called 
        /// when an ant smells a friendly marker for the first time.
        /// Read more: "http://wiki.antme.net/en/API1:DetectedScentFriend(Marker)"
        /// </summary>
        /// <param name="marker">marker</param>
        public override void DetectedScentFriend(Marker marker)
        {
            // Call behavior
            behavior.DetectedScentFriend(marker);
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
            // Call behavior
            behavior.SpotsFriend(ant);
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
            // Call behavior
            behavior.SpotsTeammate(ant);
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
            // Call behavior
            behavior.SpotsEnemy(ant);
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees a bug.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Bug)"
        /// </summary>
        /// <param name="bug">spotted bug</param>
        public override void SpotsEnemy(Bug bug)
        {
            // Call behavior
            behavior.SpotsEnemy(bug);
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if an 
        /// enemy ant attacks; the ant can then decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Ant)"
        /// </summary>
        /// <param name="ant">attacking ant</param>
        public override void UnderAttack(Ant ant)
        {
            // Call behavior
            behavior.UnderAttack(ant);
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if a 
        /// bug attacks; the ant can decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Bug)"
        /// </summary>
        /// <param name="bug">attacking bug</param>
        public override void UnderAttack(Bug bug)
        {
            // Call behavior
            behavior.UnderAttack(bug);
        }

        #endregion

    }
}