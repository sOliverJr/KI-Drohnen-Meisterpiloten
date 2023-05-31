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
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
    {
    }
    
    private void Rotate(float speed)
    {
        this.transform.Rotate(this.transform.up, speed * Time.deltaTime, Space.Self);
    }

    // Agent Controls
    public void FrontLeftRotor(int running)
    {
        rb.AddForceAtPosition(this.gameObject.transform.up * running * thrustInputValue, rotorFrontLeft.position);
    }
    public void FrontRightRotor(int running)
    {
        rb.AddForceAtPosition(this.gameObject.transform.up * running * thrustInputValue, rotorFrontRight.position);
    }
    public void RearLeftRotor(int running)
    {
        rb.AddForceAtPosition(this.gameObject.transform.up * running * thrustInputValue, rotorRearLeft.position);
    }
    public void RearRightRotor(int running)
    {
        rb.AddForceAtPosition(this.gameObject.transform.up * running * thrustInputValue, rotorRearRight.position);
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}