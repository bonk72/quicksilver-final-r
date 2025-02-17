using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float wallDetectionRange = 1f;
    public float detectionRadius = 10f;  // How far enemy can see player
    public float stoppingDistance = 5f;   // Optimal shooting distance
    public float runAwayDistance = 3f;    // Distance at which enemy runs away
    public float detectionDelay = 0.5f;   // Delay before starting to chase player
    public LayerMask wallLayer;
    public GameObject projectilePrefab;   // Projectile to shoot
    public float shootingCooldown = 2f;   // Time between shots

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
                // Already detected and delay is over
                if (distanceToPlayer <= runAwayDistance)
                {
                    // Run away if player is too close
                    RunAwayFromPlayer();
                }
                else if (distanceToPlayer <= stoppingDistance)
                {
                    // At optimal range - stop and shoot
                    moveDirection = Vector2.zero;
                    if (canShoot)
                    {
                        StartCoroutine(ShootAtPlayer());
                    }
                }
                else
                {
                    // Chase player to get in range
                    ChasePlayer();
                }
            }
        }
        else
        {
            // Player out of detection range
            moveDirection = Vector2.zero;
        }
    }

    private IEnumerator DetectionDelayCoroutine()
    {
        isWaitingToChase = true;
        yield return new WaitForSeconds(detectionDelay);
        hasDetectedPlayer = true;
        isWaitingToChase = false;
    }

    private void RunAwayFromPlayer()
    {
        Vector2 directionFromPlayer = ((Vector2)transform.position - (Vector2)player.position).normalized;
        moveDirection = directionFromPlayer;
    }

    private IEnumerator ShootAtPlayer()
    {
        canShoot = false;

        // Calculate direction to player
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        // Instantiate and setup projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.velocity = directionToPlayer * 10f; // Adjust projectile speed as needed
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
            // Apply movement
            rb.velocity = moveDirection * moveSpeed;
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

    void OnDrawGizmos()
    {
        // Visualize detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualize stopping distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Visualize run away distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, runAwayDistance);

        // Visualize wall detection rays
        if (player != null)
        {
            Gizmos.color = Color.white;
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + directionToPlayer * wallDetectionRange);
            
            // Draw alternative directions
            Vector2 rightDirection = Quaternion.Euler(0, 0, 45) * directionToPlayer;
            Vector2 leftDirection = Quaternion.Euler(0, 0, -45) * directionToPlayer;
            
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDirection * wallDetectionRange);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDirection * wallDetectionRange);
        }
    }
}