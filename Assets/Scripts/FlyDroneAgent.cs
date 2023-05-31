using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FlyDroneAgent : Agent
{
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material looseMaterial;
    [SerializeField] private MeshRenderer floor;
    
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 15f, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneControllerTest>().ResetVelocity();
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Convert.ToInt32(Input.GetKey(KeyCode.Q));
        discreteActions[1] = Convert.ToInt32(Input.GetKey(KeyCode.W));
        discreteActions[2] = Convert.ToInt32(Input.GetKey(KeyCode.A));
        discreteActions[3] = Convert.ToInt32(Input.GetKey(KeyCode.S));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Position of the drone as input
        // Position <=> Vector3 -> 3 inputs
        sensor.AddObservation(transform.localPosition);
        // Rotation of the drone as input
        // Rotation <=> Vector3 -> 3 inputs
        sensor.AddObservation(transform.rotation.eulerAngles);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int rotorOneActive = actions.DiscreteActions[0];
        int rotorTwoActive = actions.DiscreteActions[1];
        int rotorThreeActive = actions.DiscreteActions[2];
        int rotorFourActive = actions.DiscreteActions[3];
        GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneControllerTest>().FrontLeftRotor(rotorOneActive);
        GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneControllerTest>().FrontRightRotor(rotorTwoActive);
        GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneControllerTest>().RearLeftRotor(rotorThreeActive);
        GameObject.FindGameObjectWithTag("Drone").GetComponent<DroneControllerTest>().RearRightRotor(rotorFourActive);
        SetReward(+1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10) // Goal
        {
            SetReward(+500f);
            floor.material = winMaterial;
            EndEpisode();
        }
        
        if (collision.gameObject.layer == 12) // Walls or floor
        {
            SetReward(-100f);
            floor.material = looseMaterial;
            EndEpisode();
        }
    }
}
