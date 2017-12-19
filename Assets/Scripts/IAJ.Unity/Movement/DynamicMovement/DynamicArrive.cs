using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public Vector3 GoalPosition;

        public override string Name
        {
            get { return "DynamicArrive"; }
        }

        public float MaxSpeed { get; set; }
        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }

        // Update is called once per frame
        public override MovementOutput GetMovement()
        {
            Vector3 direction = this.Target.position - this.Character.position;
            float goalDistance = (this.GoalPosition - this.Character.position).magnitude; // can't be optimized to sqrMagnitude with no side effects

            float targetSpeed;

            if (goalDistance < StopRadius)
            {
                //Debug.Log("ARRIVED");
                Character.Arrived = true;
                return new MovementOutput();
            }

            if (goalDistance > SlowRadius)
                targetSpeed = MaxSpeed;
            else
                targetSpeed = MaxSpeed * (goalDistance / SlowRadius);

            this.MovingTarget = new KinematicData();
            this.MovingTarget.velocity = direction.normalized * targetSpeed;

            return base.GetMovement();
        }
    }
}