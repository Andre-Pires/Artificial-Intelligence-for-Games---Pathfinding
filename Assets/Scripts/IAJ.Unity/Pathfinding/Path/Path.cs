using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public abstract class Path
    {
        /// <summary>
        /// Gets the param value (a value that represents how far from the beggining are we in the path) in the path closer to the specified position
        /// </summary>
        /// <param name="position">the position which will be used to determined the closest Param</param>
        /// <param name="previousParam">the value of the previous param, usefull to disambiguate if you have more than one closest possible param</param>
        /// <returns></returns>
        public abstract float GetParam(Vector3 position, float previousParam);

        /// <summary>
        /// Gets the position in the path that corresponds to the specified param
        /// </summary>
        /// <param name="param">the param value (that represents how far from the beggining are we in the path)</param>
        /// <returns></returns>
        public abstract Vector3 GetPosition(float param);

        /// <summary>
        /// Returns true if the param received corresponds to (or is very close to) the end of the path
        /// </summary>
        /// <param name="param">the param to see if corresponds to the end of the path</param>
        /// <returns></returns>
        public abstract bool PathEnd(float param);
    }
}