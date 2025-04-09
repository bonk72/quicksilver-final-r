using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public floatinghealthbar healthbar;
    private float threshold;
    public float timeToAdd;
    public int goldToAdd;
    public bool invulnerable;
    [Header("Particle Effects")]
    public GameObject particleEffect; // Particle effect to play on death
    public GameObject damageParticleEffect; // Separate particle effect for when taking damage
    public Color deathParticleColor = Color.white; // Color of death particles, defaults to white
    public float deathParticleScale = 3f; // Scale multiplier for death particles (larger than damage particles)
    private Color damageParticleColor = new Color(0.8f, 0.0f, 0.0f, 1.0f); // Blood red color for damage particles
    
    
    // Event that will be triggered when damage is taken
    public event Action<float> OnDamageTaken;
    
    void Start()
    {
        threshold = GetComponentInChildren<execute>().executeThreshold;
        currentHealth = maxHealth;
        healthbar = GetComponentInChildren<floatinghealthbar>();
        healthbar.UpdateHealthBar(currentHealth, maxHealth, threshold, invulnerable);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            bullet bulletScript = collision.gameObject.GetComponent<bullet>();
            if (bulletScript != null)
            {
                TakeDamage(bulletScript.damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // If invulnerable, don't take damage
        if (invulnerable)
        {
            // Still update the healthbar to show the invulnerable state
            healthbar.UpdateHealthBar(currentHealth, maxHealth, threshold, invulnerable);
            return;
        }

        currentHealth -= damage;
        healthbar.UpdateHealthBar(currentHealth, maxHealth, threshold, invulnerable);

        // Play damage particle effect if assigned
        if (damage > 0)
        {
            // Use damageParticleEffect if available, otherwise fall back to particleEffect
            GameObject effectToUse = damageParticleEffect != null ? damageParticleEffect : particleEffect;
            
            if (effectToUse != null)
            {
                GameObject particleInstance = Instantiate(effectToUse, transform.position, Quaternion.identity);
                
                // Set particle color to red (blood) for damage
                ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    var main = particleSystem.main;
                    main.startColor = damageParticleColor; // Always red for damage
                }
            }
        }

        // Trigger the OnDamageTaken event
        OnDamageTaken?.Invoke(damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Check if this GameObject has a dropOnDeath component
        dropOnDeath dropComponent = GetComponent<dropOnDeath>();
        if (dropComponent != null)
        {
            // Call DropDeath before destroying the GameObject
            dropComponent.DropDeath();
        }
        
        // Play death particle effect if assigned (with larger size)
        if (particleEffect != null)
        {
            GameObject particleInstance = Instantiate(particleEffect, transform.position, Quaternion.identity);
            
            // Make death particles larger
            particleInstance.transform.localScale *= deathParticleScale;
            
            // Set particle color if available
            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = deathParticleColor; // Use custom color for death
                
                // Also increase the particle size in the system itself for better scaling
                main.startSize = main.startSize.constant * deathParticleScale;
            }
        }
        
        // Destroy the enemy GameObject
        Destroy(gameObject);
    }
    
    public void execute(){
        // If invulnerable, don't execute
        if (invulnerable)
        {
            return;
        }
        
        // Check if this GameObject has a dropOnDeath component
        dropOnDeath dropComponent = GetComponent<dropOnDeath>();
        if (dropComponent != null)
        {
            // Call DropDeath before destroying the GameObject
            dropComponent.DropDeath();
        }
        
        // Play death particle effect if assigned (with larger size)
        if (particleEffect != null)
        {
            GameObject particleInstance = Instantiate(particleEffect, transform.position, Quaternion.identity);
            
            // Make death particles larger
            particleInstance.transform.localScale *= deathParticleScale;
            
            // Set particle color if available
            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = deathParticleColor; // Use custom color for death
                
                // Also increase the particle size in the system itself for better scaling
                main.startSize = main.startSize.constant * deathParticleScale;
            }
        }
        
        Destroy(gameObject);
    }
}
