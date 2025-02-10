using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAroundAxis : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Default: Y-axis
    [SerializeField] private float rotationSpeed = 30f; // Degrees per second
    [SerializeField] private bool rotateContinuously = true; // If false, rotate only when triggered

    private bool isRotating = false;

    void Update()
    {
        if (rotateContinuously)
        {
            Rotate();
        }
        else if (isRotating)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    // Call this function to start/stop rotation manually
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    // Call this function to rotate once by a specific angle
    public void RotateOnce(float angle)
    {
        transform.Rotate(rotationAxis * angle);
    }
}

