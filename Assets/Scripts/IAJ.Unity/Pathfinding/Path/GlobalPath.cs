using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; }
        public bool IsPartial { get; set; }
        public List<LocalPath> LocalPaths { get; protected set; }
        public float LastParam { get; set; }

        public bool Switched { get; private set; }

        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LocalPath>();
            Switched = true;
            LastParam = 0f;
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            Vector3 previousPosition = initialPosition;
            for (int i = 0; i < this.PathPositions.Count; i++)
            {
                if (!previousPosition.Equals(this.PathPositions[i]))
                {
                    this.LocalPaths.Add(new LineSegmentPath(previousPosition, this.PathPositions[i]));
                    previousPosition = this.PathPositions[i];
                }
            }
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
            float localCurrentParam;
            int integerPart = (int)previousParam;
            float localParam = previousParam - integerPart;

            if (integerPart >= LocalPaths.Count)
                localCurrentParam = LocalPaths[LocalPaths.Count - 1].GetParam(position, localParam);
            else
                localCurrentParam = LocalPaths[integerPart].GetParam(position, localParam);

            if (localCurrentParam > 0.9)
            {
                integerPart++;
                localParam = 0;
                if (integerPart >= LocalPaths.Count)
                    localCurrentParam = LocalPaths[LocalPaths.Count - 1].GetParam(position, localParam);
                else
                    localCurrentParam = LocalPaths[integerPart].GetParam(position, localParam);
            }

            Switched = integerPart > (int)this.LastParam;
            this.LastParam = integerPart + localCurrentParam;

            return integerPart + localCurrentParam;
        }

        public override Vector3 GetPosition(float param)
        {
            int integerPart = (int)param;
            float localParam = param - integerPart;

            if (param >= LocalPaths.Count)
                return LocalPaths[LocalPaths.Count - 1].GetPosition(0.99f);
            else
                return LocalPaths[integerPart].GetPosition(localParam);
        }

        public override bool PathEnd(float param)
        {
            return Math.Floor(param) > (LocalPaths.Count - 1);
        }
    }
}