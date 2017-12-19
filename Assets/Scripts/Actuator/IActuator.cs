using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IActuator
    {
        GlobalPath GetPath(GlobalPath smoothedPath);

        void SetPath(GlobalPath path);

        MovementOutput GetOutput(GlobalPath suggestedPath, DynamicCharacter character);

        void SetGoalPosition(Vector3 goal);

        //Has to be passed for debug purposes due to DrawGizmos, use for debug only!
        DynamicMovement GetMovement();
    }
}