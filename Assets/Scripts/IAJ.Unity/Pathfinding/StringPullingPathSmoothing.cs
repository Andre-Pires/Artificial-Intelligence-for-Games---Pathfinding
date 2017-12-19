using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;

using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public static class StringPullingPathSmoothing
    {
        public static int previousIndex = 0;

        public static void Initialize()
        {
            previousIndex = 0;
        }

        /// <summary>
        /// Method used to smooth a received path, using a string pulling technique
        /// it returns a new path, where the path positions are selected in order to provide a smoother path
        /// </summary>
        /// <param name="data"></param>
        /// <param name="globalPath"></param>
        /// <returns></returns>
        public static GlobalPath SmoothPath(KinematicData data, GlobalPath globalPath)
        {
            var smoothedPath = new GlobalPath
            {
                IsPartial = globalPath.IsPartial
            };

            Vector3 initialPosition = data.position;
            int closestIndex = globalPath.PathNodes.Count - 1;

            if (globalPath.PathNodes.Count > 0)
            {
                float minMagnitude = float.MaxValue;
                for (int i = globalPath.PathNodes.Count - 1; i >= previousIndex; i--)
                {
                    if ((globalPath.PathNodes[i].Position - data.position).sqrMagnitude < minMagnitude
                        && i >= previousIndex)
                    {
                        minMagnitude = (globalPath.PathNodes[i].Position - data.position).sqrMagnitude;
                        closestIndex = i;
                    }
                }
                previousIndex = closestIndex;

                NavMeshEdge edge;
                Vector3 previousPoint = globalPath.PathPositions[globalPath.PathPositions.Count - 1];

                for (int i = globalPath.PathNodes.Count - 1; i > previousIndex; i--)
                {
                    edge = globalPath.PathNodes[i] as NavMeshEdge;
                    if (edge == null)
                    {
                        globalPath.PathNodes.RemoveAt(i);
                        globalPath.PathPositions.RemoveAt(i);
                        continue;
                    }

                    Vector3 newPoint = MathHelper.ClosestPointInLineSegment2ToLineSegment1(initialPosition,
                        previousPoint, edge.PointTwo, edge.PointOne, initialPosition);

                    var distance = (newPoint - previousPoint).sqrMagnitude;
                    if (distance < 1.44f) // 1.44 = 1.2^2
                    {
                        globalPath.PathNodes.RemoveAt(i);
                        globalPath.PathPositions.RemoveAt(i);
                        continue;
                    }

                    previousPoint = newPoint;
                    smoothedPath.PathPositions.Add(newPoint);
                }

                smoothedPath.PathPositions.Reverse();
            }
            //previousIndex++;

            return smoothedPath;
        }
    }
}