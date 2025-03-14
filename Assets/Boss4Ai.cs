using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4Ai : MonoBehaviour
{
    [Header("Turret Spawning")]
    public SpawnTurrets turretSpawner;
    public int numberOfTurretsToSpawn = 5;
    public float healthThreshold = 30f;
    
    [Header("References")]
    private Enemy enemyComponent;
    private EnemyHealth healthComponent;
    private Rigidbody2D rb;
    
    private bool hasTriggeredDefenseMode = false;
    private int turretsSpawned = 0;
    private RigidbodyConstraints2D originalConstraints;
    private bool isSpawningActive = false;
    private bool wasDetected = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get components from the same GameObject
        enemyComponent = GetComponent<Enemy>();
        healthComponent = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();

        
        if (rb != null)
        {
            originalConstraints = rb.constraints;
        }
        
        // Make sure the turret spawner is initially disabled
        if (turretSpawner != null)
        {
            turretSpawner.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyComponent == null || turretSpawner == null) return;
        
        // Check if the enemy has detected the player using the detectedStatus method
        bool isDetected = enemyComponent.detectedStatus();
        
        // If detection status changed to detected and spawning not already active
        if (isDetected && !wasDetected && !isSpawningActive)
        {
            ActivateTurretSpawning();
        }
        // If detection status changed to not detected and spawning is active
        else if (!isDetected && wasDetected && isSpawningActive && !hasTriggeredDefenseMode)
        {
            DeactivateTurretSpawning();
        }
        
        // Update previous detection state for next frame
        wasDetected = isDetected;
        
        // Check if health is below the threshold and we haven't triggered defense mode yet
        if (healthComponent.currentHealth <= healthThreshold && !hasTriggeredDefenseMode)
        {
            ActivateDefenseMode();
        }
    }
    
    void ActivateTurretSpawning()
    {
        if (turretSpawner != null)
        {
            turretSpawner.enabled = true;
            isSpawningActive = true;
            Debug.Log("Player detected - Turret spawning activated");
        }
    }
    
    void DeactivateTurretSpawning()
    {
        if (turretSpawner != null)
        {
            turretSpawner.enabled = false;
            isSpawningActive = false;
            Debug.Log("Player lost - Turret spawning deactivated");
        }
    }
    
    void ActivateDefenseMode()
    {
        Debug.Log("Boss health low - Activating defense mode");
        hasTriggeredDefenseMode = true;
        
        // Stop the GameObject's movement
        StopMovement();
        
        // Start spawning turrets rapidly
        StartCoroutine(SpawnMultipleTurrets());
    }
    
    void StopMovement()
    {
        // Stop the rigidbody
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
        
        // Disable the Enemy script to prevent it from moving
        if (enemyComponent != null)
        {
            enemyComponent.enabled = false;
        }
        
        Debug.Log("Boss movement stopped");
    }
    
    void ResumeMovement()
    {
        // Restore original rigidbody settings
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = originalConstraints;
        }
        
        // Re-enable the Enemy script
        if (enemyComponent != null)
        {
            enemyComponent.enabled = true;
        }
        
        Debug.Log("Boss movement resumed");
    }
    
    IEnumerator SpawnMultipleTurrets()
    {
        turretsSpawned = 0;
        
        // Make sure turret spawner is enabled
        if (turretSpawner != null)
        {
            turretSpawner.enabled = true;
            isSpawningActive = true;
        }
        
        // Wait for turrets to spawn
        for (int i = 0; i < numberOfTurretsToSpawn; i++)
        {
            // Force a turret spawn
            turretSpawner.SpawnTurret();
            
            turretsSpawned++;
            Debug.Log($"Spawned turret {turretsSpawned} of {numberOfTurretsToSpawn}");
            
            // Wait a short time between spawns
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("All defense turrets spawned, resuming movement");
        
        // Resume movement after all turrets are spawned
        ResumeMovement();
    }
}
