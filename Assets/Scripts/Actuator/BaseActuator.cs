using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using UnityEngine;

namespace Assets.Scripts.Actuator
{
    public abstract class BaseActuator
    {
        protected const float LookAheadDistance = 40f;
        protected DynamicCharacter Character;
        public Vector3 GoalPosition { get; set; }
        public DynamicFollowPath FollowPathMovement { get; protected set; }
        public DynamicSeek SeekMovement { get; protected set; }

        protected MovementOutput GetOutput(GlobalPath suggestedPath, DynamicCharacter character)
        {
            DynamicMovement movement;

            if (suggestedPath != null)
            {
                SeekMovement.Target = new KinematicData()
                {
                    position = suggestedPath.LocalPaths[0].EndPosition
                };
                //Debug.DrawLine(character.KinematicData.position, SeekMovement.Target.position, Color.black);
                movement = SeekMovement;

                return movement.GetMovement();
            }
            else
            {
                movement = FollowPathMovement;
            }

            return FilterMovementOutput(movement.GetMovement(), Character);
        }

        protected abstract MovementOutput FilterMovementOutput(MovementOutput output, DynamicCharacter character);

    }
}
