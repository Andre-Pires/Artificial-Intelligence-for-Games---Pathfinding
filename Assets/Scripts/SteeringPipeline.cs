using Assets.Scripts;
using Assets.Scripts.Constraint;
using Assets.Scripts.IAJ.Unity.Movement;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;
using UnityEngine;

public class SteeringPipeline : MonoBehaviour
{
    private const int ITERATIONS_LIMIT = 10;

    // Steering pipeline components
    protected Targeter TargeterComponent { get; private set; }

    protected Decomposer DecomposerComponent { get; private set; }
    protected List<IConstraint> PathConstraints { get; private set; }
    protected List<IConstraint> MovementConstraints { get; private set; }
    protected IActuator Actuator { get; private set; }
    public CharacterType CharacterInUse = CharacterType.Human;

    public enum CharacterType
    {
        Human,
        Car
    };

    public Camera camera;

    public DynamicCharacter character;
    public GameObject characterAvatar;

    public GlobalPath currentSmoothedSolution;

    // used to draw the path
    public GlobalPath currentSolution;

    [HideInInspector]
    public bool CalculatePath;

    [HideInInspector]
    public bool draw = true;

    //public fields to be set in Unity Editor
    public GameObject endDebugSphere;

    public List<Pedestrian> Pedestrians;

    //private fields for internal use only
    private Vector3 startPosition;

    private bool violation;
    private Vector3 mouseClickPosition;

    // Use this for initialization
    private void Awake()
    {
        this.draw = true;
        NavMeshPathGraph navMesh = NavigationManager.Instance.NavMeshGraphs[0];

        this.character = new DynamicCharacter(this.characterAvatar);
        this.Pedestrians = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pedestrian")).ConvertAll(p => new Pedestrian(p));

        // Targeter
        TargeterComponent = new Targeter(this);

        // Decomposer
        DecomposerComponent = new Decomposer(this, navMesh, this.character);

        // TargetCollisionConstraint
        PathConstraints = new List<IConstraint>();
        PathConstraints.Add(new PedestrianAvoidanceConstraint(this, character, this.Pedestrians));
        PathConstraints.Add(new AvoidObstacleConstraint(this.character));

        // Movement Constraints
        MovementConstraints = new List<IConstraint>();
        StaticObstacleConstraint obsConstraint = new StaticObstacleConstraint(character)
        {
            AvoidMargin = 10f
        };

        MovementConstraints.Add(obsConstraint);

        if (CharacterInUse.Equals(CharacterType.Car))
        {
            Actuator = new CarActuator(this.character);
        }
        else if (CharacterInUse.Equals(CharacterType.Human))
        {
            Actuator = new HumanActuator(this.character);
        }
    }

    // Update is called once per frame

    public void OnGUI()
    {
        if (this.currentSolution != null)
        {
            float time = DecomposerComponent.aStarPathFinding.TotalProcessingTime * 1000;
            float timePerNode;
            if (DecomposerComponent.aStarPathFinding.TotalProcessedNodes > 0)
            {
                timePerNode = time / DecomposerComponent.aStarPathFinding.TotalProcessedNodes;
            }
            else
            {
                timePerNode = 0;
            }
            string text = "Nodes Visited: " + DecomposerComponent.aStarPathFinding.TotalProcessedNodes
                          + "\nMaximum Open Size: " + DecomposerComponent.aStarPathFinding.MaxOpenNodes
                          + "\nProcessing time (ms): " + time
                          + "\nTime per Node (ms):" + timePerNode;
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(10, 10, 200, 100), text);
        }
    }

    public void OnDrawGizmos()
    {
        if (this.draw)
        {
            //draw the current Solution Path if any (for debug purposes)
            if (this.currentSolution != null)
            {
                Vector3 previousPosition = this.startPosition;
                foreach (Vector3 pathPosition in this.currentSolution.PathPositions)
                {
                    Debug.DrawLine(previousPosition, pathPosition, Color.red);
                    previousPosition = pathPosition;
                }

                previousPosition = this.character.KinematicData.position;

                if (this.currentSmoothedSolution != null)
                {
                    Gizmos.color = Color.black;
                    foreach (Vector3 pathPosition in this.currentSmoothedSolution.PathPositions)
                    {
                        Debug.DrawLine(previousPosition, pathPosition, Color.green);
                        Gizmos.DrawCube(pathPosition, new Vector3(1f, 1f, 1f));
                        previousPosition = pathPosition;
                    }
                }
            }

            //  draw the nodes in Open and Closed Sets
            if (DecomposerComponent.aStarPathFinding != null)
            {
                Gizmos.color = Color.cyan;

                if (DecomposerComponent.aStarPathFinding.Open != null)
                {
                    foreach (NodeRecord nodeRecord in DecomposerComponent.aStarPathFinding.Open.All())
                    {
                        //Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }

                Gizmos.color = Color.blue;

                if (DecomposerComponent.aStarPathFinding.Closed != null)
                {
                    foreach (NodeRecord nodeRecord in DecomposerComponent.aStarPathFinding.Closed.All())
                    {
                        //Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }
            }

            Gizmos.color = Color.yellow;
            //draw the target for the follow path movement
            //GetMovement should only be used for debug purposes
            if (Actuator.GetMovement() != null)
            {
                Gizmos.DrawSphere(Actuator.GetMovement().Target.position, 1.0f);
            }

            if (this.currentSmoothedSolution != null && CharacterInUse == CharacterType.Car)
            {
                CarActuator carActuator = (CarActuator)Actuator;

                Gizmos.color = Color.green;
                if (carActuator.FollowPathMovement.Target != null)
                    Gizmos.DrawCube(carActuator.FollowPathMovement.Target.position, new Vector3(2f, 2f, 2f));

                Gizmos.color = Color.red;
                if (carActuator.SeekMovement.Target != null)
                    Gizmos.DrawCube(carActuator.SeekMovement.Target.position, new Vector3(2f, 2f, 2f));
            }
        }
    }

    private void Update()
    {
        foreach (var pedestrian in this.Pedestrians)
        {
            pedestrian.Update();
        }

        Vector3 testMouseClickPosition;
        if (TargeterComponent.GetGoalPosition(out testMouseClickPosition))
        {
            this.mouseClickPosition = testMouseClickPosition;
            Actuator.SetGoalPosition(this.mouseClickPosition);
            DecomposerComponent.Initialize(this.mouseClickPosition);
            this.startPosition = this.character.KinematicData.position;
            currentSmoothedSolution = null;
        }

        if (this.character.KinematicData.Arrived)
        {
            //NOTE: stops the char from going forward before it flips back
            this.character.KinematicData.velocity = Vector3.zero;
            return;
        }

        // Path recalculation for the car actuator
        if (currentSmoothedSolution != null)
        {
            float distanceToPath = (character.KinematicData.position - currentSmoothedSolution.GetPosition((Actuator.GetMovement() as DynamicFollowPath).CurrentParam)).sqrMagnitude;

            if (distanceToPath > 400 && DecomposerComponent.Initialize(this.mouseClickPosition)) // 400 = 20^2
            {
                Debug.Log("searching");
                this.startPosition = this.character.KinematicData.position;
                Actuator.SetGoalPosition(this.mouseClickPosition);
                this.CalculatePath = true;
                currentSmoothedSolution = null;
                currentSolution = null;
            }
        }

        if (this.CalculatePath)
        {
            //Debug.Log("CALCULATE PATH");
            DecomposerComponent.CalculatePath(out this.currentSolution);
        }

        if (this.currentSolution != null &&
            (this.currentSmoothedSolution == null || DecomposerComponent.InNewLocalPath(this.currentSmoothedSolution) || this.violation))
        {
            //Debug.Log("NEW LINE");
            this.currentSmoothedSolution = DecomposerComponent.SmoothPath(this.currentSolution);
            Actuator.SetPath(currentSmoothedSolution);
        }

        if (this.currentSmoothedSolution == null)
            return;

        GlobalPath actuatorPath = null;
        GlobalPath pathToFollow = currentSmoothedSolution;
        violation = false;

        for (int i = 0; i < ITERATIONS_LIMIT; i++)
        {
            actuatorPath = Actuator.GetPath(pathToFollow);

            foreach (IConstraint constraint in PathConstraints)
            {
                Vector3 suggestedPosition;
                violation = constraint.WillViolate(actuatorPath, out suggestedPosition);
                if (violation)
                {
                    pathToFollow = new GlobalPath();
                    pathToFollow.LocalPaths.Add(new LineSegmentPath(character.KinematicData.position, suggestedPosition));
                    //Debug.Log("Path collision violation");
                    break;
                }
            }

            if (violation)
                continue;

            // restrições de output
            foreach (IConstraint constraint in MovementConstraints)
            {
                Vector3 suggestedPosition;
                violation = constraint.WillViolate(actuatorPath, out suggestedPosition);
                if (violation)
                {
                    pathToFollow = new GlobalPath();
                    pathToFollow.LocalPaths.Add(new LineSegmentPath(character.KinematicData.position, suggestedPosition));
                    //Debug.Log("Movement collision violation");
                    break;
                }
            }

            if (!violation)
                break;
        }

        Debug.DrawLine(this.character.KinematicData.position, actuatorPath.LocalPaths[0].EndPosition, Color.black);

        //NOTE: setting new legal path for the actuator and clearing auxiliary variable
        if (!violation)
        {
            Actuator.SetPath(actuatorPath);
            actuatorPath = null;
        }

        character.MovementOutput = Actuator.GetOutput(actuatorPath, character);

        this.character.Update();
    }
}