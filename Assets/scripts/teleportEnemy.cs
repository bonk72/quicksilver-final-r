using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportEnemy : MonoBehaviour
{
    public Transform centerPoint;
    public float dashRadius = 5f;    // How far from center point to pick dash target
    public float dashInterval = 2f;   // Time between dashes
    public float dashSpeed = 20f;     // Speed of the dash
    public float dashDistance = 5f;   // Maximum distance of a single dash

    private RangedEnemy rangedEnemy;
    private bool isDashing = false;
    private bool isMoving = false;
    private Transform player;
    private Vector3 dashTarget;

    void Start()
    {
        if (centerPoint == null)
        {
            // If no center point is set, use the enemy's starting position
            GameObject center = new GameObject("TeleportCenter");
            center.transform.position = transform.position;
            centerPoint = center.transform;
        }

        rangedEnemy = GetComponent<RangedEnemy>();
        if (rangedEnemy == null)
        {
            Debug.LogError("TeleportEnemy requires a RangedEnemy component!");
            enabled = false;
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(CheckForPlayerDetection());
    }

    private IEnumerator CheckForPlayerDetection()
    {
        while (true)
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                
                // Start dashing when player is within detection radius
                if (distanceToPlayer <= rangedEnemy.detectionRadius && !isDashing && !isMoving)
                {
                    isDashing = true;
                    StartCoroutine(DashRoutine());
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DashRoutine()
    {
        while (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > rangedEnemy.detectionRadius)
            {
                break; // Stop dashing if player is out of range
            }

            yield return new WaitForSeconds(dashInterval);
            
            // Generate random position within circle
            Vector2 randomOffset = Random.insideUnitCircle * dashRadius;
            Vector3 targetPosition = centerPoint.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // Ensure minimum dash distance
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            
            // If random target is closer than minimum dash distance, extend it
            if (distanceToTarget < dashDistance)
            {
                dashTarget = transform.position + directionToTarget * dashDistance;
            }
            else
            {
                dashTarget = targetPosition;
            }
            
            // Start the dash movement
            isMoving = true;
            
            // Perform the dash
            while (isMoving)
            {
                float distanceToDashTarget = Vector3.Distance(transform.position, dashTarget);
                if (distanceToDashTarget > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
                    yield return null;
                }
                else
                {
                    isMoving = false;
                }
            }
        }
        isDashing = false;
    }
}
