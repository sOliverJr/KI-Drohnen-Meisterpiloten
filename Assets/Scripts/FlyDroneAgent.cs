using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FlyDroneAgent : Agent
{
    public GoalController goalController;

    [SerializeField] private MeshRenderer floor;
    [SerializeField] public DroneController drone;
    [SerializeField] public Vector3 startPosition;

    private float _lastDistance;
    
    private Material _winMaterial;
    private Material _crashLooseMaterial;
    private Material _flipLooseMaterial;

    public void Start()
    {
        _winMaterial = (Material)Resources.Load("Materials/winMaterial", typeof(Material));
        _crashLooseMaterial = (Material)Resources.Load("Materials/crashLooseMaterial", typeof(Material));
        _flipLooseMaterial = (Material)Resources.Load("Materials/flipLooseMaterial", typeof(Material));
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPosition;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        drone.ResetVelocity();

        _lastDistance = Vector3.Distance(startPosition, goalController.goal.transform.position);
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        
        continuousActions[0] = Convert.ToInt32(Input.GetKey(KeyCode.Q));
        continuousActions[1] = Convert.ToInt32(Input.GetKey(KeyCode.W));
        continuousActions[2] = Convert.ToInt32(Input.GetKey(KeyCode.A));
        continuousActions[3] = Convert.ToInt32(Input.GetKey(KeyCode.S));
        continuousActions[4] = Convert.ToInt32(Input.GetKey(KeyCode.Y));
        continuousActions[5] = Convert.ToInt32(Input.GetKey(KeyCode.X));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Position of the drone as input
        // Position <=> Vector3 -> 3 inputs
        sensor.AddObservation(transform.position);
        
        // Rotation of the drone as input
        // Rotation <=> Vector3 -> 3 inputs
        sensor.AddObservation(transform.rotation.eulerAngles);
        
        // Position of the goal as input
        // Position <=> Vector3 -> 3 inputs
        Transform goal = goalController.goal.transform;
        sensor.AddObservation(goal.position);
        
        // Distance
        sensor.AddObservation(Vector3.Distance(transform.position, goal.position));
        
        // Level
        sensor.AddObservation(Vector3.Dot(transform.up, Vector3.down));
        
        // Direction
        sensor.AddObservation(Vector3.Dot(transform.forward,
            (goal.position - transform.position).normalized));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotorOneActive = actions.ContinuousActions[0];
        float rotorTwoActive = actions.ContinuousActions[1];
        float rotorThreeActive = actions.ContinuousActions[2];
        float rotorFourActive = actions.ContinuousActions[3];
        float turnLeftAction = actions.ContinuousActions[4];
        float turnRightAction = actions.ContinuousActions[5];

        drone.FrontLeftRotor(rotorOneActive);
        drone.FrontRightRotor(rotorTwoActive);
        drone.RearLeftRotor(rotorThreeActive);
        drone.RearRightRotor(rotorFourActive);
        drone.Rotate(-turnLeftAction);
        drone.Rotate(turnRightAction);
        
        // check if flipped
        // -1 = upright
        // 1 = flipped 180°
        float level = Vector3.Dot(transform.up, Vector3.down);
        if (level > 0)
        {
            SetReward(-500f);
            floor.material = _flipLooseMaterial;
            EndEpisode();
        }
        else if (level < -0.8)
        {
            AddReward(2f);
        }
        else
        {
            AddReward(-4f);
        }

        // check if is facing target
        float dot = Vector3.Dot(transform.forward,
            (goalController.goal.transform.position - transform.position).normalized);

        AddReward(dot * 3);

        AddReward(7f);
        
        if (_lastDistance -
            Vector3.Distance(transform.position, goalController.goal.transform.position) > 0)
            AddReward(1);
        else 
            AddReward(-1);
        
        _lastDistance = Vector3.Distance(transform.position, goalController.goal.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == goalController.goal) // Goal
        {
            SetReward(+1000);
            floor.material = _winMaterial;
            
            // reset goal?
            goalController.SetGoal();
        } else
        {
            // check distance
            AddReward(-500 -Vector3.Distance(transform.position, goalController.goal.transform.position));
            // SetReward(-100f);
            floor.material = _crashLooseMaterial;
        }
        
        EndEpisode();
    }
}