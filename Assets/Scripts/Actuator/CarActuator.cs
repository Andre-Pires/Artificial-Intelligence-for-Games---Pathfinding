﻿using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using Assets.Scripts.Actuator;
using UnityEngine;

namespace Assets.Scripts
{
    public class CarActuator : BaseActuator, IActuator
    {
        public CarActuator(DynamicCharacter character)
        {
            SeekMovement = new DynamicSeek()
            {
                MaxAcceleration = 80f,
                Character = character.KinematicData,
                StopRadius = 49f // 49 = 7^2
            };

            this.Character = character;
            this.Character.BackingUp = false;
            //setting character MaxSpeed according to actuator
            this.Character.MaxSpeed = 40f;
            this.FollowPathMovement = new DynamicFollowPath(character.KinematicData)
            {
                MaxAcceleration = 60f,
                MaxSpeed = 50f,
                SlowRadius = 15f,
                StopRadius = 7f,
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
            Character.BackingUp = false;
        }

        public DynamicMovement GetMovement()
        {
            return this.FollowPathMovement;
        }

        public GlobalPath GetPath(GlobalPath smoothedPath)
        {
            GlobalPath realPath = new GlobalPath();

            float distance = 0f;
            int longIndex = 0;
            int shortIndex = 0;

            for (; longIndex < smoothedPath.LocalPaths.Count; ++longIndex)
            {
                distance += (smoothedPath.LocalPaths[longIndex].EndPosition - smoothedPath.LocalPaths[longIndex].StartPosition).magnitude;  // can't be optimized to sqrMagnitude with no side effects

                if (distance < LookAheadDistance / 2)
                {
                    shortIndex++;
                }

                if (distance > LookAheadDistance)
                {
                    break;
                }
            }

            if (longIndex == 0 || shortIndex == 0)
            {
                realPath.LocalPaths.Add(new LineSegmentPath(Character.KinematicData.position, smoothedPath.LocalPaths[0].EndPosition));
                return realPath;
            }

            // long lookahead goal
            Vector3 longGoal = smoothedPath.LocalPaths[longIndex - 1].EndPosition;
            longGoal.y = 0;
            Vector3 longNewDirection = longGoal - Character.KinematicData.position;
            // long lookahead goal

            // short lookahead goal
            Vector3 shortGoal = smoothedPath.LocalPaths[shortIndex - 1].EndPosition;
            shortGoal.y = 0;
            Vector3 shortNewDirection = shortGoal - Character.KinematicData.position;
            // short lookahead goal

            // tests if the new position is behind an obstacle
            Debug.DrawLine(Character.KinematicData.position, (Character.KinematicData.position + longNewDirection.normalized * (longNewDirection.magnitude + 15f)), Color.magenta);
            Debug.DrawLine(Character.KinematicData.position, (Character.KinematicData.position + shortNewDirection.normalized * (shortNewDirection.magnitude + 15f)), Color.grey);

            if (Physics.Raycast(Character.KinematicData.position, longNewDirection, longNewDirection.magnitude + 15.0f)) // can't be optimized to sqrMagnitude with no side effects
            {
                CapCharacterVelocity(this.Character);

                if (Physics.Raycast(Character.KinematicData.position, shortNewDirection,
                    shortNewDirection.magnitude + 15.0f)) // can't be optimized to sqrMagnitude with no side effects
                {
                    //Debug.Log("Colisao long & short" );
                    realPath.LocalPaths.Add(new LineSegmentPath(Character.KinematicData.position,
                        smoothedPath.LocalPaths[0].EndPosition));
                }
                else
                {
                    // Debug.Log("Colisao long");
                    realPath.LocalPaths.Add(new LineSegmentPath(Character.KinematicData.position, shortGoal));
                }
            }
            else
            {
                if (Physics.Raycast(Character.KinematicData.position, shortNewDirection,
                    shortNewDirection.magnitude + 15.0f)) // can't be optimized to sqrMagnitude with no side effects
                {
                    //Debug.Log("Colisao short");
                    CapCharacterVelocity(this.Character);
                }
                //Debug.Log("Long index " + longIndex);
                realPath.LocalPaths.Add(new LineSegmentPath(Character.KinematicData.position, longGoal));
            }

            return realPath;
        }

        public MovementOutput GetOutput(GlobalPath suggestedPath, DynamicCharacter character)
        {
            return base.GetOutput(suggestedPath, character);
        }

        protected override MovementOutput FilterMovementOutput(MovementOutput output, DynamicCharacter character)
        {
            float charOrientation = character.KinematicData.orientation;
            float charSpeed = character.KinematicData.velocity.sqrMagnitude;
            float velocity = MathHelper.ConvertVectorToOrientation(output.linear);
            float angleDiff = MathHelper.SmallestDifferenceBetweenTwoAngles(charOrientation, velocity);
            float angleDiffAbs = Math.Abs(angleDiff);

            float distance = (GoalPosition - character.KinematicData.position).sqrMagnitude;

            if (character.BackingUp || angleDiffAbs >= MathConstants.MATH_PI - MathConstants.MATH_PI / 3f && charSpeed <= 1 && distance < 900f) // 900 = 30^2
            {
                character.BackingUp = true;

                Vector3 contraryVector =
                    MathHelper.ConvertOrientationToVector(MathHelper.ConvertVectorToOrientation((GoalPosition - character.KinematicData.position)) + MathConstants.MATH_PI);

                Vector3 charVector = MathHelper.ConvertOrientationToVector(charOrientation);
                character.KinematicData.orientation = MathHelper.ConvertVectorToOrientation(Vector3.Lerp(charVector, contraryVector, 0.1f));
                return output;
            }

            if (!character.BackingUp)
            {
                if (Physics.Raycast(character.KinematicData.position, character.KinematicData.velocity, 30f))
                {
                    //Debug.DrawLine(character.KinematicData.position, character.KinematicData.velocity.normalized*30f+character.KinematicData.position, Color.black);

                    CapCharacterVelocity(character);
                }

                if (angleDiff > MathConstants.MATH_PI_2 / 6f && charSpeed >= 4900) // 4900 = 70^2
                {
                    output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_2 / 6f).normalized;
                    character.KinematicData.velocity = character.KinematicData.velocity.normalized * 40f;
                    return output;
                }
                if (angleDiff > MathConstants.MATH_PI_2 / 3f && charSpeed >= 3600) // 3600 = 60^2
                {
                    output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_2 / 3f).normalized;
                    character.KinematicData.velocity = character.KinematicData.velocity.normalized * 30f;
                    return output;
                }
                if (angleDiff > MathConstants.MATH_PI_4 && charSpeed >= 2500) // 2500 = 50^2
                {
                    output.linear = MathHelper.ConvertOrientationToVector(MathConstants.MATH_PI_4).normalized;
                    character.KinematicData.velocity = character.KinematicData.velocity.normalized * 25f;
                    return output;
                }

                if (angleDiffAbs >= MathConstants.MATH_PI_2 / 3f && charSpeed <= 100) // 100 = 10^2
                {
                    // aqui altera apenas a direcção em que em que o carro vira
                    if (angleDiff < 0)
                    {
                        output.linear = MathHelper.ConvertOrientationToVector(character.KinematicData.orientation - MathConstants.MATH_PI_2 / 48f);
                        character.KinematicData.velocity *= 2f;
                    }
                    else if (angleDiff >= 0)
                    {
                        output.linear = MathHelper.ConvertOrientationToVector(character.KinematicData.orientation + MathConstants.MATH_PI_2 / 48f);
                        character.KinematicData.velocity *= 2f;
                    }
                    return output;
                }
            }

            return output;
        }

        private static void CapCharacterVelocity(DynamicCharacter character)
        {
            var charSpeed = character.KinematicData.velocity.sqrMagnitude;

            if (charSpeed >= 900f) // 900 = 30^2
                character.KinematicData.velocity *= 0.95f;
            else if (charSpeed >= 400f) // 400 = 20^2
                character.KinematicData.velocity *= 0.97f;
            else if (charSpeed >= 100f) // 100 = 10^2
                character.KinematicData.velocity *= 0.99f;
        }
    }
}