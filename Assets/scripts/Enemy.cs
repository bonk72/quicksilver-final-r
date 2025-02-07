using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float wallDetectionRange = 1f;
    public float detectionRadius = 10f;  // How far enemy can see player
    public float stoppingDistance = 2f;   // How close enemy gets to player
    public LayerMask wallLayer;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Only move if player is within detection radius and further than stopping distance
        if (distanceToPlayer <= detectionRadius && distanceToPlayer > stoppingDistance)
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
        else
        {
            // Stop if player is too far or too close
            moveDirection = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.velocity = moveDirection * moveSpeed;
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
            Gizmos.color = Color.red;
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
