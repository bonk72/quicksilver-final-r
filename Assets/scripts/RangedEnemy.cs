using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float wallDetectionRange = 1f;
    public float detectionRadius = 10f;  // How far enemy can see player
    public float stoppingDistance = 5f;   // Optimal shooting distance
    public float detectionDelay = 0.5f;   // Delay before starting to chase player
    public LayerMask wallLayer;
    public GameObject projectilePrefab;   // Projectile to shoot
    public float shootingCooldown = 2f;   // Time between shots
    public float projectileSpeed = 10f;   // Speed of the projectile
    public float minSpreadAngle = -5f;    // Minimum projectile spread angle
    public float maxSpreadAngle = 5f;     // Maximum projectile spread angle
    public float projectileInvisibleTime = 0.05f; // Duration projectile is invisible when spawned

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
                if (distanceToPlayer <= stoppingDistance)
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

        // Visualize stopping distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

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