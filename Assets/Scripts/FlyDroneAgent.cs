using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FlyDroneAgent : Agent
{
    public DynGoalController goalController;
    
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material looseMaterial;
    [SerializeField] private MeshRenderer floor;
    
    [SerializeField] public DroneControllerTest drone;

    [SerializeField] public Vector3 startPosition;

    private float lastDistance;


    public override void OnEpisodeBegin()
    {
        transform.localPosition = startPosition;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        drone.ResetVelocity();

        lastDistance = Vector3.Distance(startPosition, goalController.currentGoal.transform.position);
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
        Transform goal = goalController.currentGoal.transform;
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
            EndEpisode();
        } 
        /*
        else if (level < 0.5)
        {
            AddReward(1f);
        }
        */

        // check if is facing target
        float dot = Vector3.Dot(transform.forward,
            (goalController.currentGoal.transform.position - transform.position).normalized);

        AddReward(dot * 3);

        AddReward(1f);
        
        if (lastDistance -
            Vector3.Distance(transform.position, goalController.currentGoal.transform.position) > 0)
            AddReward(1);
        else 
            AddReward(-1);
        
        lastDistance = Vector3.Distance(transform.position, goalController.currentGoal.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == goalController.currentGoal) // Goal
        {
            SetReward(+500);
            floor.material = winMaterial;

            if (!goalController.useStaticGoal)
            {
                // DynGoalController.Instance.DeleteGoal();
                goalController.RandomiseGoalPosition();
            }
        } 
        else
        {
            // check distance
            AddReward(-500 -Vector3.Distance(transform.position, goalController.currentGoal.transform.position));
            // SetReward(-100f);
            floor.material = looseMaterial;
        }
        
        EndEpisode();
    }
}
