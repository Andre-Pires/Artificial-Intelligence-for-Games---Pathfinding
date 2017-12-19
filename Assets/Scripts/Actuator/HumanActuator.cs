using System;
using Assets.Scripts.Actuator;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class HumanActuator : BaseActuator, IActuator
    {
        public HumanActuator(DynamicCharacter character)
        {
            SeekMovement = new DynamicSeek
            {
                MaxAcceleration = 60f,
                Character = character.KinematicData,
                StopRadius = 49f // 49 = 7^2
            };

            this.Character = character;
            this.Character.MaxSpeed = 30f;

            FollowPathMovement = new DynamicFollowPath(character.KinematicData)
            {
                MaxAcceleration = 40f,
                MaxSpeed = 30f,
                SlowRadius = 3.5f,
                StopRadius = 3f,
                GoalPosition = GoalPosition
            };
        }

        public void SetPath(GlobalPath path)
        {
            FollowPathMovement.Path = path;
        }

        public void SetGoalPosition(Vector3 goal)
        {
            GoalPosition = goal;
            FollowPathMovement.GoalPosition = goal;
            SeekMovement.GoalPosition = goal;
            FollowPathMovement.CurrentParam = 0;
        }

        public GlobalPath GetPath(GlobalPath smoothedPath)
        {
            GlobalPath realPath = new GlobalPath();

            int i;

            for (i = 0; i < smoothedPath.LocalPaths.Count; ++i)
            {
                if ((smoothedPath.LocalPaths[i].EndPosition - this.Character.KinematicData.position).magnitude >
                    // can't be optimized to sqrMagnitude with no side effects
                    LookAheadDistance)
                {
                    break;
                }
            }

            if (i == 0)
            {
                //Debug.Log("PATH VAZIO");
                realPath.LocalPaths.Add(new LineSegmentPath(this.Character.KinematicData.position,
                    smoothedPath.LocalPaths[0].StartPosition));
                return realPath;
            }

            Vector3 newGoal = smoothedPath.LocalPaths[i - 1].EndPosition;
            newGoal.y = 0;
            Vector3 newDirection = newGoal - this.Character.KinematicData.position;

            // tests if the new position is behind an obstacle
            Debug.DrawLine(this.Character.KinematicData.position, newGoal, Color.magenta);

            if (!Physics.Raycast(this.Character.KinematicData.position, newDirection, newDirection.magnitude))
                // can't be optimized to sqrMagnitude with no side effects
            {
                realPath.LocalPaths.Add(new LineSegmentPath(this.Character.KinematicData.position, newGoal));
            }
            else
            {
                //Debug.Log("Colisao");
                realPath.LocalPaths.Add(new LineSegmentPath(this.Character.KinematicData.position,
                    smoothedPath.LocalPaths[0].EndPosition));

                this.Character.KinematicData.velocity *= 0.995f;
            }

            return realPath;
        }


        public MovementOutput GetOutput(GlobalPath suggestedPath, DynamicCharacter character)
        {
            return base.GetOutput(suggestedPath, character);
        }

        public DynamicMovement GetMovement()
        {
            return FollowPathMovement;
        }

        protected override MovementOutput FilterMovementOutput(MovementOutput output, DynamicCharacter character)
        {
            float charOrientation = character.KinematicData.orientation;
            float charSpeed = character.KinematicData.velocity.sqrMagnitude;
            float velocity = MathHelper.ConvertVectorToOrientation(output.linear);
            float angleDiff = Math.Abs(MathHelper.SmallestDifferenceBetweenTwoAngles(charOrientation, velocity));

            // OPTIMIZACAO: trocar a divisao por multiplicacao
            if (angleDiff > MathConstants.MATH_PI_2/6f && charSpeed >= 729) // 729 = 27^2
            {
                output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_2/6f).normalized;
                character.KinematicData.velocity = character.KinematicData.velocity.normalized*22f;
                return output;
            }
            if (angleDiff > MathConstants.MATH_PI_2/3f && charSpeed >= 484) // 484 = 22^2
            {
                output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_2/3f).normalized;
                character.KinematicData.velocity = character.KinematicData.velocity.normalized*18f;
                return output;
            }
            if (angleDiff > MathConstants.MATH_PI_4 && charSpeed >= 289) // 289 = 17^2
            {
                output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_4).normalized;
                character.KinematicData.velocity = character.KinematicData.velocity.normalized*15f;
                return output;
            }
            if (angleDiff > MathConstants.MATH_PI_2 && charSpeed >= 225) // 225 = 15^2
            {
                output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_2).normalized;
                character.KinematicData.velocity = character.KinematicData.velocity.normalized*13f;
                return output;
            }
            return output;
        }
    }
}