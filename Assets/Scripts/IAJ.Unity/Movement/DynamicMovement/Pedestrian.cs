using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class Pedestrian : DynamicCharacter
    {
        public DynamicMovement Movement { get; set; }

        public Pedestrian(GameObject gameObject) : base(gameObject)
        {
            this.Movement = new DynamicStraightAheadAndBack()
            {
                Character = this.KinematicData,
                MaxAcceleration = 60.0f,
                Target = new KinematicData()
            };
            this.Drag = 0.3f;
        }

        public override void Update()
        {
            this.MovementOutput = Movement.GetMovement();

            this.KinematicData.Integrate(this.MovementOutput, this.Drag, Time.deltaTime);
            this.KinematicData.SetOrientationFromVelocity();
            this.KinematicData.TrimMaxSpeed(this.MaxSpeed);

            this.GameObject.transform.position = this.KinematicData.position;
            this.GameObject.transform.rotation = Quaternion.AngleAxis(this.KinematicData.orientation * MathConstants.MATH_180_PI, Vector3.up);
        }
    }
}
