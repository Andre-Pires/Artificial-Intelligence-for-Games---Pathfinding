using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.NavMesh;
using UnityEngine;

namespace Assets.Scripts
{
    public class Decomposer
    {
        public SteeringPipeline Pipeline { get; set; }
        public NodeArrayAStarPathFinding aStarPathFinding { get; set; }
        public DynamicCharacter Character { get; set; }
        public Vector3 Position { get; private set; }
        public GlobalPath CurrentSolution { get; private set; }

        public Decomposer(SteeringPipeline pipeline, NavMeshPathGraph navMesh, DynamicCharacter character)
        {
            Character = character;
            Pipeline = pipeline;
            aStarPathFinding = new NodeArrayAStarPathFinding(navMesh, new EuclideanDistanceHeuristic());
            aStarPathFinding.NodesPerSearch = 150;
        }

        public bool Initialize(Vector3 position)
        {
            Position = position;
            //initialize the search algorithm
            if (aStarPathFinding.InitializePathfindingSearch(Character.KinematicData.position, position))
            {
                StringPullingPathSmoothing.Initialize();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        ///     Calculates the path if there is a new goal
        /// </summary>
        public void CalculatePath(out GlobalPath currentSolution)
        {
            currentSolution = null;

            //call the pathfinding method if the user specified a new goal
            if (aStarPathFinding.InProgress)
            {
                bool finished = aStarPathFinding.Search(out currentSolution, true);

                if (finished && currentSolution != null)
                {
                    Pipeline.CalculatePath = false;
                }
            }
        }

        /// <summary>
        ///     Smoothes the current path
        /// </summary>
        public GlobalPath SmoothPath(GlobalPath currentSolution)
        {
            GlobalPath currentSmoothedSolution = null;
            currentSmoothedSolution = StringPullingPathSmoothing.SmoothPath(Character.KinematicData,
                currentSolution);
            currentSmoothedSolution.PathPositions.Add(Position);
            currentSmoothedSolution.CalculateLocalPathsFromPathPositions(
                Character.KinematicData.position);
            // currentSmoothedSolution.LastParam = lastParam;

            return currentSmoothedSolution;
        }

        public bool InNewLocalPath(GlobalPath currentSolution)
        {
            return currentSolution.Switched;
        }
    }
}