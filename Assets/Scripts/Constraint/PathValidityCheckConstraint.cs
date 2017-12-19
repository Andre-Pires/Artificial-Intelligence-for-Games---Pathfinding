using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    internal class PathValidityCheckConstraint : IConstraint
    {
        private readonly DynamicCharacter Character;
        public float AvoidMargin = 4f;
        public float MaxLookAhead = 3.0f;

        public PathValidityCheckConstraint(DynamicCharacter character)
        {
            this.Character = character;
        }

        public bool WillViolate(GlobalPath path, out Vector3 suggestedPosition)
        {
            foreach (LocalPath local in path.LocalPaths)
            {
                if (local.StartPosition == local.EndPosition) continue;

                var direction = local.EndPosition - local.StartPosition;

                RaycastHit collision;
                if (!Physics.Raycast(local.StartPosition, direction, out collision, direction.magnitude)) continue; // can't be optimized to sqrMagnitude with no side effects

                suggestedPosition = this.Character.KinematicData.position +
                                    (collision.normal +
                                     (local.EndPosition - this.Character.KinematicData.position)).normalized *
                                    this.AvoidMargin;

                Debug.DrawRay(collision.point, collision.normal, Color.cyan);
                Debug.DrawLine(this.Character.KinematicData.position, suggestedPosition, Color.magenta);

                return true;
            }

            suggestedPosition = Vector3.zero;
            return false;
        }
    }
}