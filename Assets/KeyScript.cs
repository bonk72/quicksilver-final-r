using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public vulnerable vulnerableObject;
    
    // Tag of the object that can pick up the key (usually the player)
    public string pickupTag = "Player";
    
    // Optional visual effect when picked up


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the pickup tag
        if (collision.CompareTag(pickupTag))
        {
            OnPickup();
        }
    }

    // This method is called when the player picks up the key
    public void OnPickup()
    {
        if (vulnerableObject != null)
        {
            vulnerableObject.EnableVulnerability();
        }
        
        // Spawn pickup effect if assigned

        
        // Destroy the key after pickup
        Destroy(gameObject);
    }
}