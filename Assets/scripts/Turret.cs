using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Target Settings")]
    public float detectionRadius = 10f;  // How far turret can see player
    public float activationDelay = 1.0f; // Delay before turret starts shooting after detecting player
    
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;   // Projectile to shoot
    public float projectileSpeed = 10f;   // Speed of the projectile
    public float minSpreadAngle = -5f;    // Minimum projectile spread angle
    public float maxSpreadAngle = 5f;     // Maximum projectile spread angle
    public float projectileInvisibleTime = 0.05f; // Duration projectile is invisible when spawned
    
    [Header("Fire Rate Settings")]
    public float minFireRate = 1f;        // Minimum time between shots (higher value = slower fire rate)
    public float maxFireRate = 3f;        // Maximum time between shots (higher value = slower fire rate)
    private float currentFireRate;        // Current randomized fire rate
    
    [Header("Burst Fire Settings")]
    public bool shootsInBursts = false;   // Whether turret shoots in bursts (like a shotgun)
    public int burstProjectileCount = 3;  // Number of projectiles in a burst
    public float burstAngleSpread = 30f;  // Total angle spread for burst projectiles
    
    private Transform player;
    private bool canShoot = true;
    private bool playerInRange = false;
    private bool isActivated = false;
    private bool isActivating = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Set initial random fire rate
        RandomizeFireRate();
    }
    
    void Update()
    {
        if (player == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is within detection radius
        if (distanceToPlayer <= detectionRadius)
        {
            playerInRange = true;
            
            // Look at player
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90 to adjust for sprite orientation
            
            // Start activation sequence if not already activated or activating
            if (!isActivated && !isActivating)
            {
                StartCoroutine(ActivationSequence());
            }
            
            // Shoot if able and activated
            if (canShoot && isActivated)
            {
                StartCoroutine(ShootAtPlayer());
            }
        }
        else
        {
            playerInRange = false;
        }
    }
    
    private IEnumerator ActivationSequence()
    {
        isActivating = true;
        
        // Optional: Add visual or audio cue that turret is activating
        
        // Wait for activation delay
        yield return new WaitForSeconds(activationDelay);
        
        isActivated = true;
        isActivating = false;
    }

    private void RandomizeFireRate()
    {
        // Generate a random fire rate between min and max values
        currentFireRate = Random.Range(minFireRate, maxFireRate);
    }

    private IEnumerator ShootAtPlayer()
    {
        canShoot = false;

        // Calculate base direction to player
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        if (shootsInBursts)
        {
            // Burst fire mode (shotgun-like)
            ShootBurst(directionToPlayer);
        }
        else
        {
            // Single projectile mode with random spread
            ShootSingleProjectile(directionToPlayer);
        }

        // Randomize fire rate for next shot
        RandomizeFireRate();
        
        // Wait for the randomized cooldown
        yield return new WaitForSeconds(currentFireRate);
        canShoot = true;
    }
    
    private void ShootSingleProjectile(Vector2 directionToPlayer)
    {
        // Add random spread
        float spreadAngle = Random.Range(minSpreadAngle, maxSpreadAngle);
        Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * directionToPlayer;
        
        // Create and initialize projectile
        CreateProjectile(spreadDirection);
    }
    
    private void ShootBurst(Vector2 directionToPlayer)
    {
        // Calculate the angle between each projectile
        float angleStep = burstProjectileCount > 1 ? burstAngleSpread / (burstProjectileCount - 1) : 0;
        float startAngle = -burstAngleSpread / 2;
        
        // Create projectiles in a spread pattern
        for (int i = 0; i < burstProjectileCount; i++)
        {
            // Calculate angle for this projectile
            float currentAngle = startAngle + (angleStep * i);
            
            // Apply the angle to the direction
            Vector2 spreadDirection = Quaternion.Euler(0, 0, currentAngle) * directionToPlayer;
            
            // Create and initialize projectile
            CreateProjectile(spreadDirection);
        }
    }
    
    private void CreateProjectile(Vector2 direction)
    {
        // Calculate rotation to face the direction of travel
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(0, 0, angle - 90);

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, projectileRotation);
        
        // Get components and initialize bullet
        bullet bulletScript = projectile.GetComponent<bullet>();
        SpriteRenderer projectileSprite = projectile.GetComponent<SpriteRenderer>();
        
        // Initialize bullet with direction
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, projectileSpeed);
        }
        
        // Make projectile temporarily invisible
        if (projectileSprite != null)
        {
            StartCoroutine(TemporaryInvisibility(projectileSprite));
        }
    }

    private IEnumerator TemporaryInvisibility(SpriteRenderer sprite)
    {
        sprite.enabled = false;
        yield return new WaitForSeconds(projectileInvisibleTime);
        if(sprite != null){
            sprite.enabled = true;
        }
    }

    void OnDrawGizmos()
    {
        // Visualize detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Visualize firing direction if player is in range
        if (player != null && playerInRange && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + directionToPlayer * 2f);
            
            // Visualize burst spread if enabled
            if (shootsInBursts)
            {
                Gizmos.color = Color.magenta;
                
                float angleStep = burstProjectileCount > 1 ? burstAngleSpread / (burstProjectileCount - 1) : 0;
                float startAngle = -burstAngleSpread / 2;
                
                for (int i = 0; i < burstProjectileCount; i++)
                {
                    float currentAngle = startAngle + (angleStep * i);
                    Vector2 spreadDirection = Quaternion.Euler(0, 0, currentAngle) * directionToPlayer;
                    Gizmos.DrawLine(transform.position, (Vector2)transform.position + spreadDirection * 1.5f);
                }
            }
        }
    }
}