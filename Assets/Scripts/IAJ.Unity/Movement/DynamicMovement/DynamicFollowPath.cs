using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public Path Path { get; set; }
        public float PathOffset { get; set; }
        public float CurrentParam { get; set; }
        private MovementOutput EmptyMovementOutput { get; set; }

        public DynamicFollowPath(KinematicData character)
        {
            PathOffset = 0.4f;
            Target = new KinematicData();
            Character = character;
            EmptyMovementOutput = new MovementOutput
            {
                linear = Vector3.zero
            };
        }

        public override MovementOutput GetMovement()
        {
            CurrentParam = Path.GetParam(Character.position, CurrentParam);
            Target.position = Path.GetPosition(CurrentParam + PathOffset);
            Target.position.y = Character.position.y;

            return base.GetMovement();
        }
    }
}