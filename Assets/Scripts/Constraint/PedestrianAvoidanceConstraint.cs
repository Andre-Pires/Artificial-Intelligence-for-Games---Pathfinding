using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Assets.Scripts
{
    internal class PedestrianAvoidanceConstraint : IConstraint
    {
        private readonly DynamicCharacter Character;
        private readonly List<Pedestrian> Pedestrians;

        private const float AvoidMargin = 15.0f;
        private const float CollisionRadius = 2.0f;
        private const float MaxTimeLookAhead = 1.0f;

        public PedestrianAvoidanceConstraint(SteeringPipeline pipeline, DynamicCharacter character, List<Pedestrian> pedestrians)
        {
            this.Pedestrians = pedestrians;
            this.Character = character;
        }

        public bool WillViolate(GlobalPath path, out Vector3 suggestedPosition)
        {
            var targetsToAvoid = false;
            var shortestTime = float.MaxValue;
            var closestMinSeparation = 0f;
            var closestDistance = 0f;
            var closestDeltaPos = Vector3.zero;
            var closestDeltaVel = Vector3.zero;
            DynamicCharacter closestTarget = null;

            foreach (var target in this.Pedestrians)
            {
                var deltaPos = target.KinematicData.position - this.Character.KinematicData.position;
                var deltaVel = target.KinematicData.velocity - this.Character.KinematicData.velocity;
                var deltaSpeed = deltaVel.magnitude; // can't be optimized to sqrMagnitude with no side effects

                if (deltaSpeed < 0.005f)
                    continue;

                var timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / (deltaSpeed * deltaSpeed);

                if (timeToClosest > MaxTimeLookAhead)
                    continue;

                var distance = deltaPos.magnitude; // can't be optimized to sqrMagnitude with no side effects
                var minSeparation = distance - deltaSpeed * timeToClosest;

                if (minSeparation > 2 * CollisionRadius)
                    continue;

                if (timeToClosest > 0 && timeToClosest < shortestTime)
                {
                    targetsToAvoid = true;
                    shortestTime = timeToClosest;
                    closestTarget = target;
                    closestMinSeparation = minSeparation;
                    closestDistance = distance;
                    closestDeltaPos = deltaPos;
                    closestDeltaVel = deltaVel;
                }
            }

            if (!targetsToAvoid)
            {
                suggestedPosition = Vector3.zero;
                return false;
            }

            Vector3 avoidanceDirection;
            if (closestMinSeparation <= 0 || closestDistance < 2 * CollisionRadius)
                avoidanceDirection = this.Character.KinematicData.position - closestTarget.KinematicData.position;
            else
                avoidanceDirection = (closestDeltaPos + closestDeltaVel * shortestTime) * -1;

            suggestedPosition = this.Character.KinematicData.position + avoidanceDirection.normalized * AvoidMargin;
            return true;
        }
    }
}