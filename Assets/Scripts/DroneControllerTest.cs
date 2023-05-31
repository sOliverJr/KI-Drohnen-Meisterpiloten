using System;
using Unity.Barracuda;
using UnityEngine;

public class DroneControllerTest : MonoBehaviour
{
    [Header("Rotors")]
    public Transform rotorFrontLeft;
    public Transform rotorFrontRight;
    public Transform rotorRearLeft;
    public Transform rotorRearRight;

    [Header("Thrust")]
    public float thrustInputValue = 5f;

    [Range(0f,1f)]
    public float rotateSpeed = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Rotate(float speed)
    {
        transform.Rotate(transform.up, speed * rotateSpeed * Time.deltaTime, Space.Self);
    }

    // Agent Controls
    public void FrontLeftRotor(float thrust)
    {
        rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorFrontLeft.position);
    }
    public void FrontRightRotor(float thrust)
    {
        rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorFrontRight.position);
    }
    public void RearLeftRotor(float thrust)
    {
        rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorRearLeft.position);
    }
    public void RearRightRotor(float thrust)
    {
        rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorRearRight.position);
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}