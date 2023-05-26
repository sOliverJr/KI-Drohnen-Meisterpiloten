using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Rotors")]
    public Transform rotorFrontLeft;
    public Transform rotorFrontRight;
    public Transform rotorRearLeft;
    public Transform rotorRearRight;

    [Header("Thrust")]
    public float defaultThrustRotor = 2;
    public float currentThrustRotorFrontLeft = 2;
    public float currentThrustRotorFrontRight = 2;
    public float currentThrustRotorRearLeft = 2;
    public float currentThrustRotorRearRight = 2;

    public float thrustInputValueUp = 10;
    public float thrustInputValueDirection = 10;

    [Range(0f,1f)]
    public float thrustMultiplicator = 0.9f;

    public float rotateSpeed = 50f;

    public bool stabilise = false;

    private Rigidbody rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
    {
      rb.AddForceAtPosition(this.gameObject.transform.up * currentThrustRotorFrontLeft, rotorFrontLeft.position);
      rb.AddForceAtPosition(this.gameObject.transform.up * currentThrustRotorFrontRight, rotorFrontRight.position);
      rb.AddForceAtPosition(this.gameObject.transform.up * currentThrustRotorRearLeft, rotorRearLeft.position);
      rb.AddForceAtPosition(this.gameObject.transform.up * currentThrustRotorRearRight, rotorRearRight.position);
    }

    void Update()
    {
        // Simplified controls
        if (Input.GetKey(KeyCode.Space)) 
        {
            SetRotorsThrustTo(thrustInputValueUp);
        } 
        else if (Input.GetKey(KeyCode.UpArrow)) 
        {
            Forward();
        } 
        else if (Input.GetKey(KeyCode.DownArrow)) 
        {
            Backwards();
        } 
        else if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            Left();
        } 
        else if (Input.GetKey(KeyCode.RightArrow)) 
        {
            Right();
        } 
        else if (stabilise)
        {
            
            StabiliseDrone();
        }
        else {
            ResetThrust();
        }


        // Turning
        if (Input.GetKey(KeyCode.Y))
        {
            Rotate(rotateSpeed);
        }
        else if (Input.GetKey(KeyCode.X)) 
        {
            Rotate(-rotateSpeed);
        }


        /* 
         // Manual controls
        if(Input.GetKey(KeyCode.Q)) {
            currentThrustRotorFrontLeft = thrustInputValue;
        } else {
            currentThrustRotorFrontLeft = defaultThrustRotor;
        }

        if(Input.GetKey(KeyCode.W)) {
            Forward();
            // currentThrustRotorFrontRight = thrustInputValue;
        } else {
            currentThrustRotorFrontRight = defaultThrustRotor;
        }

        if(Input.GetKey(KeyCode.A)) {
            currentThrustRotorRearLeft = thrustInputValue;
        } else {
            currentThrustRotorRearLeft = defaultThrustRotor;
        }

        if(Input.GetKey(KeyCode.S)) {
            currentThrustRotorRearRight = thrustInputValue;
        } else {
            currentThrustRotorRearRight = defaultThrustRotor;
        } 
        */
    }

    public void SetRotorsThrustTo(float thrust)
    {
        currentThrustRotorFrontLeft = currentThrustRotorFrontRight = currentThrustRotorRearLeft = currentThrustRotorRearRight = thrust;
    }

    private void ResetThrust()
    {
        currentThrustRotorFrontLeft = currentThrustRotorFrontRight = currentThrustRotorRearLeft = currentThrustRotorRearRight = defaultThrustRotor;
    }

    private void Forward()
    {
        SetRotorThrust("rearRight");
        SetRotorThrust("rearLeft");
    }

    private void Backwards()
    {
        SetRotorThrust("frontLeft");
        SetRotorThrust("frontRight");
    }

    private void Left()
    {
        SetRotorThrust("frontRight");
        SetRotorThrust("rearRight");
    }

    private void Right()
    {
        SetRotorThrust("frontLeft");
        SetRotorThrust("rearLeft");
    }

    private void Rotate(float speed)
    {
        this.transform.Rotate(this.transform.up, speed * Time.deltaTime, Space.Self);
    }

    private void StabiliseDrone()
    {
        Vector3 rotation = this.gameObject.transform.rotation.eulerAngles;
        // x-axis
        if (rotation[0] > 0){
            Debug.Log("Correcting to the Front");
            Forward();
        }else if (rotation[0] < 0){
            Debug.Log("Correcting Backwards");
            Backwards();
        }else {
            currentThrustRotorFrontLeft
        }

        // z-axis
        if (rotation[2] > 0){
            Debug.Log("Correcting to the Right");
            Right();
        }else if (rotation[2] < 0){
            Debug.Log("Correcting to the Left");
            Left();
        }
    }

    private void SetRotorThrust (string rotor)
    {
        switch (rotor) {
            case "frontLeft":
                currentThrustRotorFrontLeft = thrustInputValueDirection;
                currentThrustRotorRearRight = thrustInputValueDirection * thrustMultiplicator;
            break;
            case "frontRight":
                currentThrustRotorFrontRight = thrustInputValueDirection;
                currentThrustRotorRearLeft = thrustInputValueDirection * thrustMultiplicator;
            break;
            case "rearLeft":
                currentThrustRotorRearLeft = thrustInputValueDirection;
                currentThrustRotorFrontRight = thrustInputValueDirection * thrustMultiplicator;
            break;
            case "rearRight":
                currentThrustRotorRearRight = thrustInputValueDirection;
                currentThrustRotorFrontLeft = thrustInputValueDirection * thrustMultiplicator;
            break;
        }

      
    }
}
