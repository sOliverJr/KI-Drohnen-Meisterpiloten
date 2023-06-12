using System;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("References")]
    public Transform rotorFrontLeft;
    public Transform rotorFrontRight;
    public Transform rotorRearLeft;
    public Transform rotorRearRight;

    public Rigidbody _rb;
    
    [Header("Thrust")]
    public float thrustInputValue = 5f;

    [Range(0f,1f)]
    public float rotateSpeed = 50f;

    [SerializeField]
    private Vector4 currentThrust = new Vector4(0, 0, 0, 0);

    public Vector3 velocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        velocity = _rb.velocity;
    }

    public void FixedUpdate()
    {
        _rb.AddForceAtPosition(gameObject.transform.up * currentThrust.x * thrustInputValue, rotorFrontLeft.position);
        _rb.AddForceAtPosition(gameObject.transform.up * currentThrust.y * thrustInputValue, rotorFrontRight.position);
        _rb.AddForceAtPosition(gameObject.transform.up * currentThrust.z * thrustInputValue, rotorRearLeft.position);
        _rb.AddForceAtPosition(gameObject.transform.up * currentThrust.w * thrustInputValue, rotorRearRight.position);
    }

    public void Rotate(float speed)
    {
        transform.Rotate(transform.up, speed * rotateSpeed * Time.deltaTime, Space.Self);
    }

    public void setThrust(float fl, float fr, float rl, float rr)
    {
        float frontLeft = Mathf.Clamp(fl, 0, 1);
        float frontRight = Mathf.Clamp(fr, 0, 1);
        float rearLeft = Mathf.Clamp(rl, 0, 1);
        float rearRight = Mathf.Clamp(rr, 0, 1);

        currentThrust = new Vector4(frontLeft, frontRight, rearRight, rearLeft);
    }

    public Vector3 getVelocity()
    {
        return _rb.velocity;
    }

    public void ResetVelocity()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
}