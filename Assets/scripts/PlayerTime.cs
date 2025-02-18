using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerTime : MonoBehaviour
{
    public float maxTime = 60f; // Starting time in seconds
    public float currentTime;
    public float timeDecreaseRate = 1f; // How much time decreases per second

    private bool isDead = false;
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 1f; // Duration of invulnerability after taking damage

    private Rigidbody2D rb;
    private CircleCollider2D coll;
    public TMP_Text text;

    private IEnumerator TemporaryInvulnerability()
    {
        isInvulnerable = true;

        
        yield return new WaitForSeconds(invulnerabilityDuration);
        
        isInvulnerable = false;

    }


    void Start()
    {
        coll = GetComponent<CircleCollider2D>();
        
        currentTime = maxTime;
        text.text = currentTime.ToString();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(TimeDecrease());
        
    }

    void Update()
    {

        if (currentTime <= 0 && !isDead)
        {
            Die();
        }

        // Update the time bar

    }

    private IEnumerator TimeDecrease()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            if (!isDead)
            {
                currentTime -= timeDecreaseRate;
                text.text = currentTime.ToString();
                currentTime = Mathf.Max(currentTime, 0); // Prevent negative time
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && !isInvulnerable)
        {
            if (collision.gameObject.CompareTag("enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Apply time damage
                    TakeDamage(enemy.timeDamage, true);

                    // Stun the enemy
                    enemy.Stun();
                }
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemyProjectile"))
            {
                bullet bul = collision.gameObject.GetComponent<bullet>();
                if (bul != null)
                {
                    TakeDamage(bul.damage, false);
                }
            }
    }

    public void TakeDamage(float damage, bool contact)
    {
        if (!isInvulnerable && !isDead)
        {
            currentTime -= damage;
            text.text = currentTime.ToString();
            currentTime = Mathf.Max(currentTime, 0); // Prevent negative time
            
            if (currentTime <= 0)
            {
                Die();
            }
            else if(contact)
            {
                StartCoroutine(TemporaryInvulnerability());
            }
        }
    }

    private void Die()
    {
        isDead = true;
        // Stop player movement
        Movement movement = GetComponent<Movement>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        // Stop weapon
        weapon weaponComponent = GetComponent<weapon>();
        if (weaponComponent != null)
        {
            weaponComponent.enabled = false;
        }
        // Disable collider
        //Collider2D collider = GetComponent<Collider2D>();
        //if (collider != null)
        //{
        //    collider.enabled = false;
        //}
        // Optional: Add death animation or particle effect here
        
        // Optional: Add game over screen or restart mechanism here
        SceneManager.LoadScene("Menu");
        Debug.Log("Player has died!");
    }

    // Optional: Method to add time (could be used for pickups or rewards)
    public void AddTime(float amount)
    {
        if (!isDead)
        {
            currentTime += amount;
            text.text = currentTime.ToString();

        }
    }
}
