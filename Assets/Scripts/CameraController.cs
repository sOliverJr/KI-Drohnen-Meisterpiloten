using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;
    public float mouseSensitivity = 2f;
    
    [Range(0.0f, 1.0f)] 
    public float cameraFollowSpeed = 0.5f;

    private float mouseX;
    private float mouseY;

    void Update()
    {
        // Rotation
        mouseX += mouseSensitivity * Input.GetAxis("Mouse X");
        mouseY -= mouseSensitivity * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(mouseY, mouseX, 0);
    }

    private void FixedUpdate()
    {
        // Position
        transform.position = Vector3.Lerp(transform.position, followTarget.position, cameraFollowSpeed);
    }
}
