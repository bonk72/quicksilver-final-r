using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveOnSpaawn : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 startPosition = new Vector3(100, 0, 0); // Starting position on canvas
    public float moveSpeed = 5f; // Speed at which the object moves to center
    
    [Header("Shake Settings")]
    public float shakeRange = 5f; // Range of the shake effect
    public float shakeSpeed = 10f; // Speed of the shake effect
    
    private enum State { Moving, Shaking }
    private State currentState;
    private Vector3 targetPosition = Vector3.zero; // Center of the canvas (0,0,0)
    private Vector3 shakeOffset;
    private float shakeTime;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the initial position
        transform.localPosition = startPosition;
        currentState = State.Moving;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                MoveToCenter();
                break;
            case State.Shaking:
                ShakeObject();
                break;
        }
    }
    
    // Move the object towards the center (0,0,0)
    private void MoveToCenter()
    {
        // Calculate distance to target
        float distance = Vector3.Distance(transform.localPosition, targetPosition);
        
        // Move towards the target
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
        
        // Check if we've reached the target (with a small threshold)
        if (distance < 0.1f)
        {
            // Switch to shaking state
            currentState = State.Shaking;
            // Ensure we're exactly at the target position before starting to shake
            transform.localPosition = targetPosition;
        }
    }
    
    // Make the object shake in all directions
    private void ShakeObject()
    {
        // Update shake time
        shakeTime += Time.deltaTime * shakeSpeed;
        
        // Generate random shake offset
        shakeOffset = new Vector3(
            Mathf.PerlinNoise(shakeTime, 0) * 2 - 1,
            Mathf.PerlinNoise(0, shakeTime) * 2 - 1,
            0
        ) * shakeRange;
        
        // Apply shake offset to the object's position
        transform.localPosition = targetPosition + shakeOffset;
    }
}
