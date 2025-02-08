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
    
    

    void Start()
    {
        threshold = GetComponentInChildren<execute>().executeThreshold;
        currentHealth = maxHealth;
        healthbar = GetComponentInChildren<floatinghealthbar>();
        healthbar.UpdateHealthBar(currentHealth, maxHealth, threshold);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            bullet bulletScript = collision.gameObject.GetComponent<bullet>();
            if (bulletScript != null)
            {
                TakeDamage(bulletScript.damage);
            }
            Destroy(collision.gameObject);
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
        // Add any death effects or score updates here
        Destroy(gameObject);
    }
    public void execute(){
        Destroy(gameObject);

    }
}
