using RAIN.Navigation.Graph;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistance : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            return Mathf.Sqrt(Mathf.Pow(node.Position.x - goalNode.Position.x, 2) + Mathf.Pow(node.Position.y - goalNode.Position.y, 2) + Mathf.Pow(node.Position.z - goalNode.Position.z, 2));
        }
    }
}