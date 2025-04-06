using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vulnerable : MonoBehaviour
{
    [Header("Key Settings")]
    public GameObject key;
    public float keySpawnRadius = 7.0f;

    [Header("Vulnerability Timer")]
    public float initialVulnerabilityTimer = 30f; // Initial time before becoming vulnerable
    public float currentVulnerabilityTimer; // Current time remaining
    public float maxTimerReductionPerSecond = 5f; // Maximum time that can be reduced per second
    private float timeSinceLastReduction = 0f; // Time tracking for reduction cap
    private float totalReductionThisSecond = 0f; // Total reduction in the current second

    [Header("Invulnerability Cycle")]
    public bool cycleInvulnerability = true; // Toggle for cycling invulnerability
    public float invulnerabilityCooldown = 15f; // Time before becoming invulnerable again
    private float currentInvulnerabilityCooldown; // Current cooldown time remaining

    [Header("Sprite Colors")]
    public Color invulnerableColor = new Color(0.71f, 0.71f, 0.71f); // Grey B5B5B5
    public Color vulnerableColor = new Color(1f, 0f, 0f); // Red FF0000

    [Header("State")]
    public bool keyDropped = false; // Whether the key has been dropped
    public bool waitingForKeyPickup = false; // Whether waiting for key pickup

    private EnemyHealth health;
    private bool coroutineRunning = false;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<EnemyHealth>();
        health.invulnerable = true;
        currentVulnerabilityTimer = initialVulnerabilityTimer;
        
        // Get the sprite renderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // Try to find it in children if not on this GameObject
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        // Set initial color to invulnerable (grey)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = invulnerableColor;
        }
        
        // Wait a frame to ensure all components are initialized
        StartCoroutine(InitializeHealthbarColor());
        
        // Subscribe to damage event
        health.OnDamageTaken += ReduceVulnerabilityTimer;
    }
    
    // Coroutine to initialize healthbar color after a frame
    private IEnumerator InitializeHealthbarColor()
    {
        // Wait for the end of the frame to ensure all components are initialized
        yield return new WaitForEndOfFrame();
        
        // Explicitly set the healthbar color to grey for invulnerable state
        if (health != null && health.healthbar != null)
        {
            float threshold = GetComponentInChildren<execute>().executeThreshold;
            health.healthbar.UpdateHealthBar(health.currentHealth, health.maxHealth, threshold, true);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from damage event to prevent memory leaks
        if (health != null)
        {
            health.OnDamageTaken -= ReduceVulnerabilityTimer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Reset timer reduction tracking each second
        timeSinceLastReduction += Time.deltaTime;
        if (timeSinceLastReduction >= 1f)
        {
            timeSinceLastReduction = 0f;
            totalReductionThisSecond = 0f;
        }

        // Handle invulnerable state
        if (health.invulnerable && !waitingForKeyPickup && !keyDropped && !coroutineRunning)
        {
            // Count down vulnerability timer
            currentVulnerabilityTimer -= Time.deltaTime;
            
            // When timer expires, drop key
            if (currentVulnerabilityTimer <= 0)
            {
                DropKey();
            }
        }
        // Handle vulnerable state cooldown
        else if (!health.invulnerable && cycleInvulnerability)
        {
            currentInvulnerabilityCooldown -= Time.deltaTime;
            
            // When cooldown expires, become invulnerable again
            if (currentInvulnerabilityCooldown <= 0)
            {
                ResetInvulnerability();
            }
        }
    }

    // Method to reduce vulnerability timer when hit
    private void ReduceVulnerabilityTimer(float damage)
    {
        // Only reduce timer if still invulnerable and timer is running
        if (health.invulnerable && !waitingForKeyPickup && !keyDropped)
        {
            // Calculate reduction amount (could be based on damage)
            float reductionAmount = damage * 0.5f; // Adjust multiplier as needed
            
            // Check if we've hit the cap for this second
            if (totalReductionThisSecond < maxTimerReductionPerSecond)
            {
                // Calculate how much we can still reduce this second
                float allowedReduction = Mathf.Min(reductionAmount, maxTimerReductionPerSecond - totalReductionThisSecond);
                
                // Apply the reduction
                currentVulnerabilityTimer -= allowedReduction;
                totalReductionThisSecond += allowedReduction;
                
                // Ensure timer doesn't go below zero
                currentVulnerabilityTimer = Mathf.Max(0, currentVulnerabilityTimer);
                
                // If timer reaches zero, drop the key
                if (currentVulnerabilityTimer <= 0 && !keyDropped)
                {
                    DropKey();
                }
            }
        }
    }

    // Method to drop the key
    private void DropKey()
    {
        if (!keyDropped)
        {
            keyDropped = true;
            waitingForKeyPickup = true;
            
            // Generate a random position around the GameObject
            Vector2 randomOffset = Random.insideUnitCircle * keySpawnRadius;
            Vector3 randomPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // Instantiate the key at the random position
            GameObject keyInstance = Instantiate(key, randomPosition, Quaternion.identity);
            
            // If the key has a script to reference back to this vulnerable object, set it up here
            // Using string reference to avoid compilation issues
            MonoBehaviour keyScript = keyInstance.GetComponent("KeyScript") as MonoBehaviour;
            if (keyScript != null)
            {
                // Use reflection to set the vulnerableObject field
                System.Type type = keyScript.GetType();
                System.Reflection.FieldInfo field = type.GetField("vulnerableObject");
                if (field != null)
                {
                    field.SetValue(keyScript, this);
                }
            }
        }
    }

    // Method to be called by the key when picked up
    public void EnableVulnerability()
    {
        waitingForKeyPickup = false;
        health.invulnerable = false;
        
        // Change sprite color to vulnerable (red)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = vulnerableColor;
        }
        
        // Explicitly update the healthbar to show the vulnerable state (green)
        if (health.healthbar != null)
        {
            // Use the threshold from the EnemyHealth component
            float threshold = GetComponentInChildren<execute>().executeThreshold;
            health.healthbar.UpdateHealthBar(health.currentHealth, health.maxHealth, threshold, false);
        }
        
        // Start cooldown for cycling back to invulnerable if enabled
        if (cycleInvulnerability)
        {
            currentInvulnerabilityCooldown = invulnerabilityCooldown;
        }
    }

    // Method to reset to invulnerable state
    private void ResetInvulnerability()
    {
        health.invulnerable = true;
        keyDropped = false;
        waitingForKeyPickup = false;
        currentVulnerabilityTimer = initialVulnerabilityTimer;
        
        // Change sprite color back to invulnerable (grey)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = invulnerableColor;
        }
        
        // Explicitly update the healthbar to show the invulnerable state (grey)
        if (health.healthbar != null)
        {
            // Use the threshold from the EnemyHealth component
            float threshold = GetComponentInChildren<execute>().executeThreshold;
            health.healthbar.UpdateHealthBar(health.currentHealth, health.maxHealth, threshold, true);
        }
    }

    // Public method that can be called to force reset invulnerability
    public void ForceResetInvulnerability()
    {
        StopAllCoroutines();
        coroutineRunning = false;
        ResetInvulnerability();
    }
}
