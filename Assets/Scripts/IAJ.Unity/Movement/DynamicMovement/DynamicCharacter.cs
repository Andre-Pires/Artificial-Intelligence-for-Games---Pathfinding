using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicCharacter
    {
        public GameObject GameObject { get; protected set; }
        public KinematicData KinematicData { get; protected set; }
        public MovementOutput MovementOutput { get; set; }
        public float Drag { get; set; }
        public float MaxSpeed { get; set; }
        public bool BackingUp { get; set; }

        public DynamicCharacter(GameObject gameObject)
        {
            this.KinematicData = new KinematicData(new StaticData(gameObject.transform.position, gameObject.transform.eulerAngles.y));
            this.GameObject = gameObject;
            this.Drag = 1;
            this.MaxSpeed = 30.0f;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (this.MovementOutput.linear != Vector3.zero)
            {
                this.KinematicData.position.y = 0f;

                this.KinematicData.Integrate(MovementOutput, this.Drag, Time.deltaTime);

                if (!BackingUp)
                    this.KinematicData.SetOrientationFromVelocity();

                this.KinematicData.TrimMaxSpeed(this.MaxSpeed);

                this.GameObject.transform.position = this.KinematicData.position;
                this.GameObject.transform.rotation = Quaternion.AngleAxis(this.KinematicData.orientation * MathConstants.MATH_180_PI, Vector3.up);
            }
        }
    }
}