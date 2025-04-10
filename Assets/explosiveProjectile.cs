using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosiveProjectile : MonoBehaviour
{
    [Header("Movement Settings")]
    private Vector2 direction;
    public float speed = 5f;
    public float lifetime = 3f; // How long the projectile lives before exploding
    private float timeAlive = 0f;
    public bool collideWithWalls = true; // Toggle whether projectile collides with walls
    
    [Header("Explosion Settings")]
    public float explosionRadius = 3f; // Radius of the explosion
    public float explosionDamage = 15f; // Damage dealt by the explosion
    public float explosionForce = 500f; // Force applied to rigidbodies
    public GameObject explosionEffectPrefab; // Optional visual effect for explosion
    
    // Reference to EnemyHealth component if present
    private EnemyHealth healthComponent;
    
    // Start is called before the first frame update
    void Start()
    {
        // If direction hasn't been set via Initialize, default to right
        if (direction == Vector2.zero)
        {
            direction = transform.right;
        }
        
        // Check if this object also has an EnemyHealth component
        healthComponent = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the projectile
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Update lifetime and explode if exceeded
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            Explode();
        }
    }
    
    // Initialize the projectile with direction and speed
    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        timeAlive = 0f;
    }
    
    // Called when the projectile collides with something
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Special case: If this object has EnemyHealth component and is hit by a bullet
        // Let the EnemyHealth component handle the bullet collision
        if (healthComponent != null && collision.CompareTag("bullet"))
        {
            // Don't explode - EnemyHealth will handle the damage
            return;
        }
        
        // Don't explode when hitting other projectiles
        if (!collision.CompareTag("bullet") && !collision.CompareTag("enemyProjectile"))
        {
            // Check if it's a wall collision
            bool isWall = collision.gameObject.layer == LayerMask.NameToLayer("normalWall");
            
            // If it's a wall and collideWithWalls is false, ignore the collision
            if (isWall && !collideWithWalls)
            {
                // Do nothing, pass through the wall
                return;
            }
            
            // Otherwise, explode on impact without dealing direct hit damage
            Explode();
        }
    }
    
    // Create an explosion effect
    void Explode()
    {
        // Spawn explosion visual effect if assigned
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // Find all colliders in explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D hit in colliders)
        {
            // Apply damage to player
            if (hit.CompareTag("Player"))
            {
                PlayerTime playerTime = hit.GetComponent<PlayerTime>();
                if (playerTime != null)
                {
                    // Apply full explosion damage regardless of distance
                    playerTime.TakeDamage(explosionDamage, true);
                }
            }
            
            // Apply explosion force to rigidbodies
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = hit.transform.position - transform.position;
                float distance = direction.magnitude;
                
                // Normalize direction and apply force inversely proportional to distance
                if (distance > 0)
                {
                    float forceMagnitude = explosionForce * (1 - distance / explosionRadius);
                    rb.AddForce(direction.normalized * forceMagnitude);
                }
            }
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }
    
    // Visualize the explosion radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
