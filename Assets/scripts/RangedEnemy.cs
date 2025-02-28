using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float wallDetectionRange = 1f;
    public float detectionRadius = 10f;  // How far enemy can see player
    public float optimalAttackRange = 5f;   // Renamed from stoppingDistance - Optimal shooting distance
    public float detectionDelay = 0.5f;   // Delay before starting to chase player
    public LayerMask wallLayer;
    public GameObject projectilePrefab;   // Projectile to shoot
    public float shootingCooldown = 2f;   // Time between shots
    public float projectileSpeed = 10f;   // Speed of the projectile
    public float minSpreadAngle = -5f;    // Minimum projectile spread angle
    public float maxSpreadAngle = 5f;     // Maximum projectile spread angle
    public float projectileInvisibleTime = 0.05f; // Duration projectile is invisible when spawned
    
    // Strafing variables
    public float minStrafingSpeed = 1.5f;      // Minimum strafing speed
    public float maxStrafingSpeed = 3f;        // Maximum strafing speed
    private float currentStrafingSpeed;        // Current strafing speed
    public float minStrafingDirectionChangeTime = 1f; // Minimum time before changing direction
    public float maxStrafingDirectionChangeTime = 3f; // Maximum time before changing direction
    private float currentStrafingDirectionChangeTime; // Current direction change time
    private float strafingTimer = 0f;
    private int strafingDirection = 1;    // 1 for right, -1 for left
    private float strafeAngleVariation = 15f; // Variation in strafe angle (degrees)
    private float currentStrafeAngle;     // Current strafe angle
    
    // Strafing pause variables
    public float pauseChance = 0.2f;      // Chance to pause during strafing (0-1)
    public float minPauseDuration = 0.2f; // Minimum pause duration
    public float maxPauseDuration = 0.8f; // Maximum pause duration
    private bool isPausing = false;

    // Stun variables
    public float stunDuration = 0.5f;
    private bool isStunned = false;
    private bool isWaitingToChase = false;
    private bool canShoot = true;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool hasDetectedPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Initialize random strafing parameters
        InitializeStrafingParameters();
    }
    
    private void InitializeStrafingParameters()
    {
        // Random initial direction
        strafingDirection = Random.value < 0.5f ? 1 : -1;
        
        // Random initial speed
        currentStrafingSpeed = Random.Range(minStrafingSpeed, maxStrafingSpeed);
        
        // Random initial direction change time
        currentStrafingDirectionChangeTime = Random.Range(minStrafingDirectionChangeTime, maxStrafingDirectionChangeTime);
        
        // Random initial strafe angle
        currentStrafeAngle = 90f + Random.Range(-strafeAngleVariation, strafeAngleVariation);
    }

    void Update()
    {
        if (player == null || isStunned) return;

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is within detection radius
        if (distanceToPlayer <= detectionRadius)
        {
            if (!hasDetectedPlayer && !isWaitingToChase)
            {
                // First time detecting player
                StartCoroutine(DetectionDelayCoroutine());
            }
            else if (hasDetectedPlayer)
            {
                if (distanceToPlayer <= optimalAttackRange)
                {
                    // At optimal range - strafe and shoot
                    if (!isPausing)
                    {
                        Strafe();
                        
                        // Random chance to pause
                        if (Random.value < pauseChance * Time.deltaTime)
                        {
                            StartCoroutine(PauseStrafing());
                        }
                    }
                    
                    if (canShoot)
                    {
                        StartCoroutine(ShootAtPlayer());
                    }
                }
                else
                {
                    // Chase player to get in range
                    ChasePlayer();
                    strafingTimer = 0f; // Reset strafing timer when not strafing
                    isPausing = false;  // Cancel any pause when not in range
                }
            }
        }
        else
        {
            // Player out of detection range
            moveDirection = Vector2.zero;
            strafingTimer = 0f; // Reset strafing timer when not strafing
            isPausing = false;  // Cancel any pause when not in range
        }
    }
    
    private IEnumerator PauseStrafing()
    {
        isPausing = true;
        moveDirection = Vector2.zero;
        
        yield return new WaitForSeconds(Random.Range(minPauseDuration, maxPauseDuration));
        
        isPausing = false;
    }

    private void Strafe()
    {
        // Update strafing timer
        strafingTimer += Time.deltaTime;
        
        // Change strafing direction and parameters periodically
        if (strafingTimer >= currentStrafingDirectionChangeTime)
        {
            // Flip direction
            strafingDirection *= -1;
            
            // Randomize parameters
            currentStrafingSpeed = Random.Range(minStrafingSpeed, maxStrafingSpeed);
            currentStrafingDirectionChangeTime = Random.Range(minStrafingDirectionChangeTime, maxStrafingDirectionChangeTime);
            currentStrafeAngle = 90f + Random.Range(-strafeAngleVariation, strafeAngleVariation);
            
            strafingTimer = 0f;
        }
        
        // Get direction to player
        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        
        // Calculate strafe direction with variable angle
        Vector2 strafeDirection;
        if (currentStrafeAngle == 90f)
        {
            // Standard perpendicular strafe
            strafeDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x) * strafingDirection;
        }
        else
        {
            // Angled strafe
            float angleInRadians = currentStrafeAngle * Mathf.Deg2Rad * strafingDirection;
            float cos = Mathf.Cos(angleInRadians);
            float sin = Mathf.Sin(angleInRadians);
            strafeDirection = new Vector2(
                directionToPlayer.x * cos - directionToPlayer.y * sin,
                directionToPlayer.x * sin + directionToPlayer.y * cos
            ).normalized;
        }
        
        // Check if strafing would hit a wall
        RaycastHit2D strafeHit = Physics2D.Raycast(transform.position, strafeDirection, wallDetectionRange, wallLayer);
        
        if (strafeHit.collider != null)
        {
            // Wall detected in strafe direction, reverse direction
            strafingDirection *= -1;
            strafeDirection *= -1;
            
            // Double check the other direction
            strafeHit = Physics2D.Raycast(transform.position, strafeDirection, wallDetectionRange, wallLayer);
            if (strafeHit.collider != null)
            {
                // Both directions blocked, don't move
                moveDirection = Vector2.zero;
                return;
            }
        }
        
        // Set movement direction to strafe
        moveDirection = strafeDirection;
    }

    private IEnumerator DetectionDelayCoroutine()
    {
        isWaitingToChase = true;
        yield return new WaitForSeconds(detectionDelay);
        hasDetectedPlayer = true;
        isWaitingToChase = false;
    }

    private IEnumerator ShootAtPlayer()
    {
        canShoot = false;

        // Calculate direction to player
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        // Add random spread
        float spreadAngle = Random.Range(minSpreadAngle, maxSpreadAngle);
        Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * directionToPlayer;
        
        // Calculate rotation to face the direction of travel
        float angle = Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(0, 0, angle - 90);

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, projectileRotation);
        
        // Get components and initialize bullet
        bullet bulletScript = projectile.GetComponent<bullet>();
        SpriteRenderer projectileSprite = projectile.GetComponent<SpriteRenderer>();
        
        // Initialize bullet with spread direction
        if (bulletScript != null)
        {
            bulletScript.Initialize(spreadDirection, projectileSpeed);
        }
        
        // Make projectile temporarily invisible
        if (projectileSprite != null)
        {
            StartCoroutine(TemporaryInvisibility(projectileSprite));
        }

        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }

    private void ChasePlayer()
    {
        // Get direction to player
        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        
        // Check for walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, wallDetectionRange, wallLayer);
        
        if (hit.collider != null)
        {
            // Wall detected, try to find alternative path
            Vector2 rightDirection = Quaternion.Euler(0, 0, 45) * directionToPlayer;
            Vector2 leftDirection = Quaternion.Euler(0, 0, -45) * directionToPlayer;
            
            // Check both directions
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, rightDirection, wallDetectionRange, wallLayer);
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, leftDirection, wallDetectionRange, wallLayer);
            
            // Choose direction with no wall
            if (!rightHit.collider)
                moveDirection = rightDirection;
            else if (!leftHit.collider)
                moveDirection = leftDirection;
            else
                moveDirection = Vector2.zero; // Both directions blocked
        }
        else
        {
            moveDirection = directionToPlayer;
        }
    }

    void FixedUpdate()
    {
        if (!isStunned && hasDetectedPlayer && !isWaitingToChase)
        {
            // Apply movement with appropriate speed
            float currentSpeed = (moveDirection.sqrMagnitude > 0 && Vector2.Distance(transform.position, player.position) <= optimalAttackRange) 
                ? currentStrafingSpeed 
                : moveSpeed;
            
            rb.velocity = moveDirection * currentSpeed;
        }
        else
        {
            // Stop movement while stunned or during detection delay
            rb.velocity = Vector2.zero;
        }
    }

    public void Stun()
    {
        if (!isStunned)
        {
            StartCoroutine(StunCoroutine());
        }
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
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

        // Visualize optimal attack range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, optimalAttackRange);

        // Visualize wall detection rays
        if (player != null && Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + directionToPlayer * wallDetectionRange);
            
            // Draw alternative directions
            Vector2 rightDirection = Quaternion.Euler(0, 0, 45) * directionToPlayer;
            Vector2 leftDirection = Quaternion.Euler(0, 0, -45) * directionToPlayer;
            
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDirection * wallDetectionRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDirection * wallDetectionRange);
            
            // Draw strafe direction if in range
            if (Vector2.Distance(transform.position, player.position) <= optimalAttackRange)
            {
                Gizmos.color = Color.blue;
                
                // Calculate strafe direction with variable angle
                Vector2 strafeDirection;
                if (currentStrafeAngle == 90f)
                {
                    // Standard perpendicular strafe
                    strafeDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x) * strafingDirection;
                }
                else
                {
                    // Angled strafe
                    float angleInRadians = currentStrafeAngle * Mathf.Deg2Rad * strafingDirection;
                    float cos = Mathf.Cos(angleInRadians);
                    float sin = Mathf.Sin(angleInRadians);
                    strafeDirection = new Vector2(
                        directionToPlayer.x * cos - directionToPlayer.y * sin,
                        directionToPlayer.x * sin + directionToPlayer.y * cos
                    ).normalized;
                }
                
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + strafeDirection * wallDetectionRange);
            }
        }
    }
}