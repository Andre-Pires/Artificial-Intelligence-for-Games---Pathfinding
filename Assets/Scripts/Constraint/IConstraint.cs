using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IConstraint
    {
        bool WillViolate(GlobalPath path, out Vector3 suggestedPosition);
    }
}