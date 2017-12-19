using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Constraint
{
    internal class StaticObstacleConstraint : IConstraint
    {
        public DynamicCharacter Character { get; private set; }
        public float AvoidMargin { get; set; }

        public StaticObstacleConstraint(DynamicCharacter character)
        {
            Character = character;
        }

        public bool WillViolate(GlobalPath path, out Vector3 suggestedPosition)
        {
            suggestedPosition = Vector3.zero;

            if (Character.KinematicData.velocity.sqrMagnitude < 0.01f)
                return false;

            var centralOrientation = Character.KinematicData.velocity;
            var leftOrientation = MathHelper.ConvertOrientationToVector(MathHelper.ConvertVectorToOrientation(centralOrientation) - MathConstants.MATH_PI_2 / 4.0F);
            var rightOrientation = MathHelper.ConvertOrientationToVector(MathHelper.ConvertVectorToOrientation(centralOrientation) + MathConstants.MATH_PI_2 / 4.0F);

            Vector3 localGoal = path.LocalPaths[0].EndPosition;

            if (checkCollision(localGoal, centralOrientation, 12f, out suggestedPosition)
                    || checkCollision(localGoal, leftOrientation, 10f, out suggestedPosition)
                    || checkCollision(localGoal, rightOrientation, 10f, out suggestedPosition))
            {
                //Debug.Log("there was a static object collision");
                return true;
            }

            return false;
        }

        private bool checkCollision(Vector3 localGoal, Vector3 orientation, float maxLookAhead, out Vector3 suggestedPosition)
        {
            RaycastHit collision;
            Debug.DrawLine(Character.KinematicData.position, (Character.KinematicData.position + orientation.normalized * maxLookAhead), Color.magenta);
            if (Physics.Raycast(Character.KinematicData.position, orientation, out collision, maxLookAhead))
            {
                Debug.DrawLine(collision.point, collision.point + (localGoal - collision.point).normalized * AvoidMargin, Color.cyan);
                suggestedPosition = collision.point + (localGoal - collision.point).normalized * AvoidMargin;
                return true;
            }
            else
            {
                suggestedPosition = Vector3.zero;
                return false;
            }
        }
    }
}