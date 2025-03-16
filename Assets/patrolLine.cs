using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolLine : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;           // First patrol point
    public Transform pointB;           // Second patrol point
    public float moveSpeed = 3.0f;     // Movement speed
    public float waitTime = 2.0f;      // Time to wait at each point
    
    [Header("Damage Settings")]
    public float timeDamage = 5.0f;    // How much time is deducted from player on collision
    
    private Transform currentTarget;   // Current target point
    private bool isWaiting = false;    // Flag to check if waiting at a point
    
    // Start is called before the first frame update
    void Start()
    {
        // Validate patrol points
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Patrol points not assigned! Please assign both pointA and pointB in the inspector.");
            enabled = false; // Disable this script if points are not assigned
            return;
        }
        
        // Set initial target
        currentTarget = pointB;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting && currentTarget != null)
        {
            // Move towards the current target
            transform.position = Vector3.MoveTowards(
                transform.position,
                currentTarget.position,
                moveSpeed * Time.deltaTime
            );
            
            // Check if reached the target
            if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                // Start waiting at the point
                StartCoroutine(WaitAtPoint());
            }
        }
    }
    
    // Wait at the current point before moving to the next one
    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        
        // Wait for the specified time
        yield return new WaitForSeconds(waitTime);
        
        // Switch target
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        
        isWaiting = false;
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
    
    // Visualize the patrol path in the editor
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            // Draw line between patrol points
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            
            // Draw spheres at patrol points
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointA.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.position, 0.5f);
        }
    }
}
