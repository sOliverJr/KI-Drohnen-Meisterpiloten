using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Rotors")]
    public Transform rotorFrontLeft;
    public Transform rotorFrontRight;
    public Transform rotorRearLeft;
    public Transform rotorRearRight;

    [Header("Thrust")]
    public float thrustInputValue = 9f;

    [Range(0f,1f)]
    public float rotateSpeed = 50f;

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Rotate(float speed)
    {
        transform.Rotate(transform.up, speed * rotateSpeed * Time.deltaTime, Space.Self);
    }

    // Agent Controls
    public void FrontLeftRotor(float thrust)
    {
        _rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorFrontLeft.position);
    }
    public void FrontRightRotor(float thrust)
    {
        _rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorFrontRight.position);
    }
    public void RearLeftRotor(float thrust)
    {
        _rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorRearLeft.position);
    }
    public void RearRightRotor(float thrust)
    {
        _rb.AddForceAtPosition(gameObject.transform.up * thrust * thrustInputValue, rotorRearRight.position);
    }

    public void ResetVelocity()
    {
        _rb.velocity = Vector3.zero;                                                                          
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        StartCoroutine(EnableRigidbodyAfterDelay());
    }

    IEnumerator EnableRigidbodyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); 

        _rb.isKinematic = false;
    }
}