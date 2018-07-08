using AntMe.English;
using AntMe.Player.ArndtBalke.Map;
using AntMe.Player.ArndtBalke.Markers;
using System.Collections.Generic;
using System.Linq;

namespace AntMe.Player.ArndtBalke.Behavior
{
    internal class CommanderBehavior : BaseBehavior
    {
        private AttackPoint currentAttackPoint = null;

        public override string Caste => "Commander";

        public CommanderBehavior(ArndtBalkeClass ant)
            : base(ant)
        { }

        protected override Target GetNextTarget()
        {
            Target nextTarget = base.GetNextTarget();

            if (nextTarget != null)
                return nextTarget;

            if (Destination is Anthill)
                return null;

            if (currentAttackPoint != null)
                currentAttackPoint.Age++;

            if (currentAttackPoint == null
                || currentAttackPoint.IsDeprecated)
            {
                currentAttackPoint = GetNextAttackPoint();

                if (currentAttackPoint == null)
                    return null;

                Think($"New attack point: X: {currentAttackPoint.X}; Y: {currentAttackPoint.Y}");
                return new Target(new Signal(AttackPoint, currentAttackPoint));
            }

            return null;
        }

        private AttackPoint GetNextAttackPoint()
        {
            List<AttackPoint> listAttackPoints = GetAttackPoints();

            if (!listAttackPoints.Any())
                return null;

            Dictionary<AttackPoint, int> distances = new Dictionary<AttackPoint, int>();
            int? minDistance = null;

            foreach (AttackPoint attackPoint in listAttackPoints)
            {
                int distance = GetDistanceTo(attackPoint);

                if (!minDistance.HasValue || minDistance.Value > distance)
                    minDistance = distance;

                distances[attackPoint] = distance;
            }

            for (int i = 0; i < listAttackPoints.Count; i++)
            {
                if (distances[listAttackPoints[i]] > (minDistance.Value + 100))
                    listAttackPoints.RemoveAt(i--);
            }

            if (currentAttackPoint != null)
            {
                int maxDistance = listAttackPoints.Max(currentAttackPoint.GetDistanceTo);

                foreach (AttackPoint attackPoint in listAttackPoints)
                {
                    attackPoint.Priority = (int)(attackPoint.Priority * (maxDistance - currentAttackPoint.GetDistanceTo(attackPoint) * 0.5) / maxDistance);
                }
            }

            AttackPoint bestAttackPoint = null;

            foreach (AttackPoint attackPoint in listAttackPoints)
            {
                if (bestAttackPoint == null || bestAttackPoint.Priority < attackPoint.Priority)
                    bestAttackPoint = attackPoint;
            }

            return bestAttackPoint;
        }

        private List<AttackPoint> GetAttackPoints()
        {
            List<AttackPoint> listAttackPoints = new List<AttackPoint>();

            foreach (Ant ant in _cache.Ants)
            {
                if (ant.CarriedFruit != null
                    || ant.CurrentLoad > 0)
                {
                    listAttackPoints.Add(new AttackPoint(GetCoordinate(ant), 5));
                }
            }

            foreach (Bug bug in _cache.Bugs)
            {
                listAttackPoints.Add(new AttackPoint(GetCoordinate(bug), 7));
            }

            foreach (Signal signal in _cache.Signals)
            {
                if (signal.InfoType == AntSpotted)
                    listAttackPoints.Add(new AttackPoint(signal.Coordinates, 2));
                else if (signal.InfoType == BugSpotted)
                    listAttackPoints.Add(new AttackPoint(signal.Coordinates, 4));
                else if (signal.InfoType == FruitNeedsProtection)
                    listAttackPoints.Add(new AttackPoint(signal.Coordinates, 10));
            }

            foreach (AttackPoint attackPoint in listAttackPoints)
            {
                foreach (AttackPoint otherPoint in listAttackPoints)
                {
                    if (attackPoint != otherPoint && attackPoint.GetDistanceTo(otherPoint) < 150)
                        attackPoint.Priority += otherPoint.Priority;
                }
            }

            return listAttackPoints;
        }

        protected override Signal GetNextSignal()
        {
            if (currentAttackPoint != null && currentAttackPoint.Age == 0)
                return new Signal(AttackPoint, currentAttackPoint);

            return base.GetNextSignal();
        }

    }
}
