using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public floatinghealthbar healthbar;
    private float threshold;
    public float timeToAdd;
    public int goldToAdd;
    
    

    void Start()
    {
        threshold = GetComponentInChildren<execute>().executeThreshold;
        currentHealth = maxHealth;
        healthbar = GetComponentInChildren<floatinghealthbar>();
        healthbar.UpdateHealthBar(currentHealth, maxHealth, threshold);
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.UpdateHealthBar(currentHealth, maxHealth, threshold);

        
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
        
        // Add any death effects or score updates here
        Destroy(gameObject);
    }
    
    public void execute(){
        // Check if this GameObject has a dropOnDeath component
        dropOnDeath dropComponent = GetComponent<dropOnDeath>();
        if (dropComponent != null)
        {
            // Call DropDeath before destroying the GameObject
            dropComponent.DropDeath();
        }
        
        Destroy(gameObject);
    }
}
