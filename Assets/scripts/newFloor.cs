using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newFloor : MonoBehaviour
{
    // Reference to the collider component
    private Collider2D floorCollider;
    
    // Delay before enabling the collider
    public float colliderEnableDelay = 1.0f;
    
    // Tag of the object that can trigger the new floor generation (usually "Player")
    
    // Flag to prevent multiple triggers
    private bool hasTriggered = false;
    
    void Start()
    {
        // Get the collider component
        floorCollider = GetComponent<Collider2D>();
        
        // Disable the collider initially
        if (floorCollider != null)
        {
            floorCollider.enabled = false;
            
            // Enable the collider after a delay
            StartCoroutine(EnableColliderAfterDelay());
        }
        else
        {
            Debug.LogError("No Collider2D found on newFloor object!");
        }
    }
    
    // Coroutine to enable the collider after a delay
    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderEnableDelay);
        floorCollider.enabled = true;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the correct tag and we haven't triggered yet
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            // Find the GameManager and generate a new dungeon
            NotifyNewFloorGeneration();
        }
    }
    
    // Method to notify the scene that a new floor should be generated
    private void NotifyNewFloorGeneration()
    {
        // Find the GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager != null)
        {
            // Call the specific method for new floor generation
            gameManager.generatenew = true;
            Debug.Log("New floor generation requested!");
        }
        else
        {
            Debug.LogWarning("GameManager not found! Cannot generate new floor.");
        }
    }
}
