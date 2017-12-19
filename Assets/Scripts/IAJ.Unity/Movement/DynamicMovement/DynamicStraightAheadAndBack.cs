using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicStraightAheadAndBack : DynamicSeek
    {
        public override MovementOutput GetMovement()
        {
            var orientationVec = MathHelper.ConvertOrientationToVector(this.Character.orientation);

            if (Physics.Raycast(this.Character.position, orientationVec, 10f))
                orientationVec *= -1;

            this.Target.position = this.Character.position + orientationVec;

            return base.GetMovement();
        }
    }
}
