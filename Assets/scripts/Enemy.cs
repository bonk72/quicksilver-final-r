using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float wallDetectionRange = 1f;
    public float detectionRadius = 10f;  // How far enemy can see player
    public float optimalAttackRange = 2f;   // Renamed from stoppingDistance - How close enemy gets to player
    private float extendedAttackRange;   // Temporarily increased attack range when player enters
    private bool isUsingExtendedRange = false; // Flag to track if using extended range
    private float attackRangeExtensionMultiplier = 1.5f; // How much to extend the attack range by
    public LayerMask wallLayer;
    public float timeDamage = 5f;  // How much time is deducted from player on collision
    public float detectionDelay = 0.5f; // Delay before starting to chase player

    // Strafing variables - different ranges from RangedEnemy
    public float minStrafingSpeed = 1.8f;      // Minimum strafing speed
    public float maxStrafingSpeed = 3.5f;      // Maximum strafing speed
    private float currentStrafingSpeed;        // Current strafing speed
    public float minStrafingDirectionChangeTime = 1.2f; // Minimum time before changing direction
    public float maxStrafingDirectionChangeTime = 2.8f; // Maximum time before changing direction
    private float currentStrafingDirectionChangeTime; // Current direction change time
    private float strafingTimer = 0f;
    private int strafingDirection = 1;    // 1 for right, -1 for left
    private float strafeAngleVariation = 20f; // Variation in strafe angle (degrees) - different from RangedEnemy
    private float currentStrafeAngle;     // Current strafe angle
    
    // Occasional forward movement during strafing (unique to Enemy)
    public float forwardMoveChance = 0.1f;  // Chance to move forward during strafing
    public float forwardMoveDuration = 0.3f; // Duration of forward movement
    private bool isMovingForward = false;

    // Dash variables
    public float dashSpeed = 15f;         // Speed of dash towards player
    public float dashDuration = 0.2f;     // Duration of dash
    public float minDashCooldown = 2f;    // Minimum time between dashes
    public float maxDashCooldown = 4f;    // Maximum time between dashes
    private float currentDashCooldown;    // Current dash cooldown time
    private float dashTimer = 0f;
    private bool isDashing = false;
    private bool canDash = true;

    // Stun variables
    public float stunDuration = 0.5f;
    private bool isStunned = false;
    private bool isWaitingToChase = false;
    private RigidbodyConstraints2D originalConstraints;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool hasDetectedPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalConstraints = rb.constraints;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Initialize attack ranges
        extendedAttackRange = optimalAttackRange * attackRangeExtensionMultiplier;
        
        // Initialize random strafing parameters
        InitializeStrafingParameters();
        
        // Initialize random dash cooldown
        currentDashCooldown = Random.Range(minDashCooldown, maxDashCooldown);
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
        if (player == null || isStunned || isDashing) return;

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
                // Check if player is entering the optimal attack range
                if (distanceToPlayer <= optimalAttackRange && !isUsingExtendedRange)
                {
                    // Player has entered the optimal range, extend the attack range
                    isUsingExtendedRange = true;
                }
                // Check if player is exiting the extended range
                else if (distanceToPlayer > extendedAttackRange && isUsingExtendedRange)
                {
                    // Player has exited the extended range, reset back to normal
                    isUsingExtendedRange = false;
                }
                
                // Use the appropriate attack range based on state
                float currentAttackRange = isUsingExtendedRange ? extendedAttackRange : optimalAttackRange;
                
                if (distanceToPlayer <= currentAttackRange)
                {
                    // At optimal range - strafe and occasionally dash
                    if (!isMovingForward)
                    {
                        Strafe();
                        // Random chance to move forward briefly (unique to Enemy)
                        if (Random.value < forwardMoveChance * Time.deltaTime)
                        {
                            StartCoroutine(MoveForwardBriefly());
                        }
                    }
                    
                    // Update dash timer and dash if ready
                    if (canDash)
                    {
                        dashTimer += Time.deltaTime;
                        if (dashTimer >= currentDashCooldown)
                        {
                            StartCoroutine(DashTowardsPlayer());
                            dashTimer = 0f;
                        }
                    }
                }
                else
                {
                    // Chase player to get in range
                    ChasePlayer();
                    strafingTimer = 0f; // Reset strafing timer when not strafing
                    dashTimer = 0f;     // Reset dash timer when not in range
                    isMovingForward = false; // Cancel any forward movement when not in range
                }
            }
        }
        else
        {
            // Stop if player is too far
            moveDirection = Vector2.zero;
            strafingTimer = 0f; // Reset strafing timer when not strafing
            dashTimer = 0f;     // Reset dash timer when not in range
            isMovingForward = false; // Cancel any forward movement when not in range
        }
    }
    
    
    private IEnumerator MoveForwardBriefly()
    {
        isMovingForward = true;
        
        // Move directly toward player
        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        moveDirection = directionToPlayer;
        
        yield return new WaitForSeconds(forwardMoveDuration);
        
        isMovingForward = false;
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

    private IEnumerator DashTowardsPlayer()
    {
        isDashing = true;
        canDash = false;
        
        // Get direction to player
        Vector2 dashDirection = ((Vector2)player.position - rb.position).normalized;
        
        // Add slight randomness to dash direction (unique to Enemy)
        float randomAngle = Random.Range(-10f, 10f);
        dashDirection = Quaternion.Euler(0, 0, randomAngle) * dashDirection;
        
        // Check for walls in dash path
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, wallDetectionRange * 2, wallLayer);
        if (hit.collider != null)
        {
            // Wall in the way, don't dash
            isDashing = false;
            
            // Set a shorter cooldown if dash fails
            float failedDashCooldown = Random.Range(minDashCooldown, maxDashCooldown) * 0.5f;
            yield return new WaitForSeconds(failedDashCooldown);
            
            canDash = true;
            yield break;
        }
        
        // Apply dash velocity
        rb.velocity = dashDirection * dashSpeed;
        
        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);
        
        // End dash and apply self-stun using the normal stun mechanic
        isDashing = false;
        Stun();
        
        // Set a new random dash cooldown and reset after stun
        StartCoroutine(ResetDashCooldown());
    }

    private IEnumerator ResetDashCooldown()
    {
        // Set a new random dash cooldown
        currentDashCooldown = Random.Range(minDashCooldown, maxDashCooldown);
        
        // Wait for stun to finish plus the cooldown time
        yield return new WaitForSeconds(currentDashCooldown);
        
        // Reset dash ability
        canDash = true;
    }

    private IEnumerator DetectionDelayCoroutine()
    {
        isWaitingToChase = true;
        yield return new WaitForSeconds(detectionDelay);
        hasDetectedPlayer = true;
        isWaitingToChase = false;
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
        if (isDashing) return; // Velocity is controlled by dash coroutine
        
        if (!isStunned && hasDetectedPlayer && !isWaitingToChase)
        {
            // Apply movement with appropriate speed
            float currentSpeed;
            
            // Use the appropriate attack range based on state
            float currentAttackRange = isUsingExtendedRange ? extendedAttackRange : optimalAttackRange;
            
            if (Vector2.Distance(transform.position, player.position) <= currentAttackRange)
            {
                // In optimal range
                if (isMovingForward)
                {
                    // Use a speed between strafing and normal when moving forward
                    currentSpeed = (currentStrafingSpeed + moveSpeed) * 0.5f;
                }
                else
                {
                    // Normal strafing
                    currentSpeed = currentStrafingSpeed;
                }
            }
            else
            {
                // Normal chase speed
                currentSpeed = moveSpeed;
            }
            
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
        isDashing = false; // Cancel any dash in progress
        rb.velocity = Vector2.zero;
        
        // Freeze position during stun
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        
        yield return new WaitForSeconds(stunDuration);
        
        // Restore original constraints
        rb.constraints = originalConstraints;
        isStunned = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StunCoroutine());
        }
    }

    void OnDrawGizmos()
    {
        // Visualize detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualize attack ranges
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, optimalAttackRange);
        
        // Visualize extended attack range if in use
        if (Application.isPlaying && isUsingExtendedRange)
        {
            Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.5f); // Light green with transparency
            Gizmos.DrawWireSphere(transform.position, extendedAttackRange);
        }

        // Visualize wall detection rays
        if (player != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + directionToPlayer * wallDetectionRange);
            
            // Draw alternative directions
            Vector2 rightDirection = Quaternion.Euler(0, 0, 45) * directionToPlayer;
            Vector2 leftDirection = Quaternion.Euler(0, 0, -45) * directionToPlayer;
            
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDirection * wallDetectionRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDirection * wallDetectionRange);
            
            // Draw strafe direction if in range
            float currentAttackRange = isUsingExtendedRange ? extendedAttackRange : optimalAttackRange;
            if (Vector2.Distance(transform.position, player.position) <= currentAttackRange)
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
                
                // Draw dash direction
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + directionToPlayer * wallDetectionRange * 2);
            }
        }
    }
}
