using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DroneAgent : Agent
{
    Rigidbody rBody;

    public Transform rotorFrontLeft;
    public Transform rotorFrontRight;
    public Transform rotorRearLeft;
    public Transform rotorRearRight;
    
    public float maxThrottle = 10f;


    private float _thrustFrontLeft, _thrustFrontRight, _thrustRearLeft, _thrustRearRight;
    void Start () {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform checkpoint;
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (transform.localPosition.y < 0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3( 0, 0.5f, 0);
        }

        // Move the target to a new spot
        checkpoint.localPosition = new Vector3(
            Random.value * 8 - 4,
            Random.value * 4 + 4,
            Random.value * 8 - 4);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        var droneTransform = transform;
        var dronePosition = droneTransform.localPosition;
        var targetVector = Vector3.Normalize(checkpoint.localPosition - dronePosition);
        
        sensor.AddObservation(dronePosition);
        sensor.AddObservation(droneTransform.rotation.eulerAngles);
        sensor.AddObservation(rBody.velocity);
        
        sensor.AddObservation(targetVector);
    }
        
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log("I was here");
        
        _thrustFrontLeft = NormalizeThrust(actionBuffers.ContinuousActions[0]);
        _thrustFrontRight = NormalizeThrust(actionBuffers.ContinuousActions[1]);
        _thrustRearLeft = NormalizeThrust(actionBuffers.ContinuousActions[2]);
        _thrustRearRight = NormalizeThrust(actionBuffers.ContinuousActions[3]);

        Debug.Log("Thrust FL: " + _thrustFrontLeft);
        Debug.Log("Thrust FR: " + _thrustFrontRight);
        Debug.Log("Thrust RL: " + _thrustRearLeft);
        Debug.Log("Thrust RR: " + _thrustRearRight);

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, checkpoint.localPosition);
        if (distanceToTarget < 1.25f)
        {
            AddReward(1);
            EndEpisode();
        }
    }

    private void FixedUpdate()
    {
        Vector3 up = rBody.transform.up;
        rBody.AddForceAtPosition(up * _thrustFrontLeft, rotorFrontLeft.position);
        rBody.AddForceAtPosition(up * _thrustFrontRight, rotorFrontRight.position);
        rBody.AddForceAtPosition(up * _thrustRearLeft, rotorRearLeft.position);
        rBody.AddForceAtPosition(up * _thrustRearRight, rotorRearRight.position);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = -1f;
        continuousActions[1] = -1f;
        continuousActions[2] = -1f;
        continuousActions[3] = -1f;
        
        
        // continuousActions[0] = 1f;
        // continuousActions[1] = 1f;
        // continuousActions[2] = 1f;
        // continuousActions[3] = 1f;
        
        if (Input.GetKey(KeyCode.W))
        {
            continuousActions[0] = 1f;
            continuousActions[1] = 1f;
            continuousActions[2] = 1f;
            continuousActions[3] = 1f;
        }
    }

    private float NormalizeThrust(float thrust)
    {
        float maxInput = 1f;
        float minInput = -1f;
        float minThrottle = 0f;

        return ScalarNormalizedToRange(thrust,maxInput,minInput,minThrottle,maxThrottle);
    }

    private float ScalarNormalizedToRange(float value, float valmax, float valmin, float newmin, float newmax)
    {
        return (newmax-newmin)/(valmax-valmin)*(value-valmax)+newmax;
    }

}
