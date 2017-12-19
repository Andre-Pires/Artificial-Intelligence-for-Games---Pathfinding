using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Utils
{
    public static class MathHelper
    {
        public static float SmallestDifferenceBetweenTwoAngles(float sourceAngle, float targetAngle)
        {
            var delta = targetAngle - sourceAngle;
            if (delta > MathConstants.MATH_PI) delta -= MathConstants.MATH_2PI;
            else if (delta < -MathConstants.MATH_PI) delta += MathConstants.MATH_2PI;

            return delta;
        }

        public static Vector3 ConvertOrientationToVector(float orientation)
        {
            return new Vector3((float)Math.Sin(orientation), 0, (float)Math.Cos(orientation));
        }

        public static float ConvertVectorToOrientation(Vector3 vector)
        {
            return Mathf.Atan2(vector.x, vector.z);
        }

        public static Vector3 Rotate2D(Vector3 vector, float angle)
        {
            var sin = (float)Math.Sin(angle);
            var cos = (float)Math.Cos(angle);

            var x = vector.x * cos - vector.z * sin;
            var z = vector.x * sin + vector.z * cos;
            return new Vector3(x, vector.y, z);
        }

        /// <summary>
        /// Returns the closest param (a value between 0 and 1) in the line segment to a given point.
        /// algorithm based on the algorithm to get the minimum distance between a point and a line segment
        /// http://geomalgorithms.com/a02-_lines.html
        /// </summary>
        /// <param name="line1P0">Start point of Line Segment</param>
        /// <param name="line1P1">End point of Line segment</param>
        /// <param name="targetPoint">The point to which we want to find the closest param</param>
        /// <returns></returns>
        public static float closestParamInLineSegmentToPoint(Vector3 line1P0, Vector3 line1P1, Vector3 targetPoint)
        {
            Vector3 v = line1P1 - line1P0;
            Vector3 w = targetPoint - line1P0;

            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
                return 0;

            float c2 = v.sqrMagnitude;
            if (c2 <= c1)
                return 1;

            return c1 / c2;
        }

        /// <summary>
        /// Returns the point in Line segment2 that is closest to Line Segment 1
        /// algorithm based on the algorithm to get the minimum distance between 2 line segments
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="line1P0">Start point of Line Segment 1</param>
        /// <param name="line1P1">End point of Line segment 1</param>
        /// <param name="line2P0">Start point of Line Segment 2</param>
        /// <param name="line2P1">End point of Line Segment 2</param>
        /// <param name="parallelTieBreaker">this point is used to select the closest point when the two line segments are pararell. In this situation, the method will return the closest line2P0/line2P1 to the tiebreaker</param>
        /// <returns></returns>
        public static Vector3 ClosestPointInLineSegment2ToLineSegment1(Vector3 line1P0, Vector3 line1P1, Vector3 line2P0, Vector3 line2P1, Vector3 parallelTieBreaker)
        {
            var u = line1P1 - line1P0;
            var v = line2P1 - line2P0;
            var w = line1P0 - line2P0;

            var a = u.sqrMagnitude;
            var b = Vector3.Dot(u, v);
            var c = v.sqrMagnitude;
            var d = Vector3.Dot(u, w);
            var e = Vector3.Dot(v, w);

            var D = a * c - b * b;
            float sN;
            float sD = D;
            float tN;
            float tD = D;

            var cosTeta = b / (u.magnitude * v.magnitude);

            if (cosTeta > (1 - 0.05f)) //the lines are almost parallel
            {
                //paralel line segments
                //we use a distinct method for parallel line segments
                //We will basically select from P0 or P1, the closest to the tiebreaker;
                if ((parallelTieBreaker - line2P0).magnitude < (parallelTieBreaker - line2P1).magnitude)
                {
                    return line2P0;
                }
                else
                {
                    return line2P1;
                }
            }
            else
            {
                sN = b * e - c * d;
                tN = a * e - b * d;
                if (sN < 0.0f)
                {
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0f)
            {
                tN = 0.0f;
            }
            else if (tN > tD)
            {
                tN = tD;
            }

            float tC = tN / tD;

            return line2P0 + tC * v;
        }
    }
}