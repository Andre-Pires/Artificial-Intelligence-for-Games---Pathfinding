using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicSeek : DynamicMovement
    {
        public override string Name
        {
            get { return "Seek"; }
        }

        public override KinematicData Target { get; set; }

        public float StopRadius { get; set; } // must be squared
        public Vector3 GoalPosition { get; set; }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();

            float goalDistance = (this.GoalPosition - this.Character.position).sqrMagnitude;

            if (goalDistance < StopRadius)
            {
                //Debug.Log("ARRIVED");
                Character.Arrived = true;
                return output;
            }

            output.linear = this.Target.position - this.Character.position;

            if (output.linear.sqrMagnitude > 0)
            {
                output.linear.Normalize();
                output.linear *= this.MaxAcceleration;
            }

            return output;
        }
    }
}