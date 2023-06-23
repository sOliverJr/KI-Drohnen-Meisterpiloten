using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;


public class DroneAgent : Agent
{
    private DroneArea _droneArea;
    
    private Rigidbody _rigidbody;
    private float _thrustRotorFrontLeft, _thrustRotorFrontRight, _thrustRotorRearLeft, _thrustRotorRearRight;
    
    public Transform rotorFrontLeft;
    public Transform rotorFrontRight;
    public Transform rotorRearLeft;
    public Transform rotorRearRight;

    /// <summary>
    /// Inizialisiere den Agenten.
    /// Wird beim Aktivieren des Agenten aufgerufen.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        _droneArea = GetComponentInParent<DroneArea>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public override void OnEpisodeBegin()
    {
        //_droneArea.ResetArea();
    }

    
    /// <summary>
    /// Sammelt Daten über die Umgebung.
    /// </summary>
    /// <param name="sensor">Der Vector Sensor zum Sammeln von Umgebungsdaten</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        var droneTransform = transform;
        sensor.AddObservation(droneTransform.localPosition);
        sensor.AddObservation(droneTransform.rotation.eulerAngles);
        sensor.AddObservation(_rigidbody.velocity);
    }
    
    /// <summary>
    /// Fürt aktionen auf Basis des eingabevektors aus.
    /// </summary>
    /// <param name="actionBuffers">Eingabevektor</param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        _thrustRotorFrontLeft = ScalarNormalizedToRange(actionBuffers.ContinuousActions[0], 1f, -1f, 0f, 1f);
        _thrustRotorFrontRight = ScalarNormalizedToRange(actionBuffers.ContinuousActions[1], 1f, -1f, 0f, 1f);
        _thrustRotorRearLeft = ScalarNormalizedToRange(actionBuffers.ContinuousActions[2], 1f, -1f, 0f, 1f);
        _thrustRotorRearRight = ScalarNormalizedToRange(actionBuffers.ContinuousActions[3], 1f, -1f, 0f, 1f);
    }

    private void FixedUpdate()
    {
        var up = gameObject.transform.up;
        var thrustMultiplier = 10f;
        _rigidbody.AddForceAtPosition(up * (_thrustRotorFrontLeft * thrustMultiplier), rotorFrontLeft.position);
        _rigidbody.AddForceAtPosition(up * (_thrustRotorFrontRight * thrustMultiplier), rotorFrontRight.position);
        _rigidbody.AddForceAtPosition(up * (_thrustRotorRearLeft * thrustMultiplier), rotorRearLeft.position);
        _rigidbody.AddForceAtPosition(up * (_thrustRotorRearRight * thrustMultiplier), rotorRearRight.position);
    }

    private float ScalarNormalizedToRange(float value, float valmax, float valmin, float newmin, float newmax)
    {
        return (newmax-newmin)/(valmax-valmin)*(value-valmax)+newmax;
    }

    private void OnCollisionEnter(Collision other)
    {
        // if (other.gameObject.CompareTag("Wall"))
        // {
        //     Reset();
        // }
        
        // if (other.gameObject.CompareTag("Checkpoint"))
        // {
        //     CheckpointReached(other.gameObject);
        // }
    }

    private void CheckpointReached(GameObject checkpoint)
    {
        AddReward(1f);
        
        _droneArea.CheckpointReached(checkpoint);
        
        EndEpisode();
    }

    /// <summary>
    /// Setzt die Drohne zurück.
    /// </summary>
    public void Reset()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0f, 2f, 0f);
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    /// <summary>
    /// Definiert die manuelle Drohnensteuerung.
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // continuousActions[0] = -1f;
        // continuousActions[1] = -1f;
        // continuousActions[2] = -1f;
        // continuousActions[3] = -1f;
        
        
        continuousActions[0] = 1f;
        continuousActions[1] = 1f;
        continuousActions[2] = 1f;
        continuousActions[3] = 1f;
        
        if (Input.GetKey(KeyCode.W))
        {
            continuousActions[0] = 1f;
            continuousActions[1] = 1f;
            continuousActions[2] = 1f;
            continuousActions[3] = 1f;
        }
    }
}
