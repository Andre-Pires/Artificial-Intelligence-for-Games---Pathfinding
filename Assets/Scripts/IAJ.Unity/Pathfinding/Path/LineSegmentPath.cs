using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class LineSegmentPath : LocalPath
    {
        protected Vector3 LineVector;

        public LineSegmentPath(Vector3 start, Vector3 end)
        {
            this.StartPosition = start;
            this.EndPosition = end;
            this.LineVector = end - start;
        }

        public override Vector3 GetPosition(float param)
        {
            return this.StartPosition + this.LineVector*param;
        }

        public override bool PathEnd(float param)
        {
            throw new NotImplementedException();
        }

        public override float GetParam(Vector3 position, float lastParam)
        {
            return MathHelper.closestParamInLineSegmentToPoint(StartPosition, EndPosition, position);
        }
    }
}