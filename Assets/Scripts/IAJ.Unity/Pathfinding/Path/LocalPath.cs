using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public abstract class LocalPath : Path
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
    }
}
