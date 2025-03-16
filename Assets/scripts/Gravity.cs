using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravityValue1 = 1.0f;  // First gravity scale value (default normal)
    public float gravityValue2 = -1.0f; // Second gravity scale value (default inverted)
    public float switchInterval = 3.0f; // Time in seconds between gravity switches
    
    [Header("Damage Settings")]
    public float timeDamage = 5.0f;     // How much time is deducted from player on collision
    
    private Rigidbody2D rb;
    private float timer = 0f;
    private bool useFirstGravity = true;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found on this GameObject!");
            enabled = false; // Disable this script if no Rigidbody2D is found
            return;
        }
        
        // Apply initial gravity
        ApplyGravity();
    }

    // Update is called once per frame
    void Update()
    {
        // Increment timer
        timer += Time.deltaTime;
        
        // Check if it's time to switch gravity
        if (timer >= switchInterval)
        {
            // Reset timer
            timer = 0f;
            
            // Toggle gravity state
            useFirstGravity = !useFirstGravity;
            
            // Apply the new gravity
            ApplyGravity();
        }
    }
    
    // Apply the current gravity value to the Rigidbody2D
    private void ApplyGravity()
    {
        if (rb != null)
        {
            // Set gravity scale based on current state
            float currentGravity = useFirstGravity ? gravityValue1 : gravityValue2;
            rb.gravityScale = currentGravity;
            
            Debug.Log("Gravity switched to: " + currentGravity);
        }
    }
    
    // Handle collision with player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the PlayerTime component
            PlayerTime playerTime = collision.gameObject.GetComponent<PlayerTime>();
            
            // Apply damage if PlayerTime component exists
            if (playerTime != null)
            {
                playerTime.TakeDamage(timeDamage, true);
                Debug.Log("Applied " + timeDamage + " damage to player");
            }
        }
    }
}
