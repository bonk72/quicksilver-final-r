using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class execute : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyHealth enemy;
    public float executeThreshold;
    public floatinghealthbar healthbar;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "execute")
        {
            // Check if the enemy is invulnerable
            if (enemy.invulnerable)
            {
                // If invulnerable, don't take damage or execute
                // Just update the healthbar to show the invulnerable state
                healthbar.UpdateHealthBar(enemy.currentHealth, enemy.maxHealth, executeThreshold, true);
                return;
            }

            if (enemy.currentHealth <= enemy.maxHealth * executeThreshold)
            {
                Movement dashreset = other.gameObject.GetComponentInParent<Movement>();
                PlayerTime time = other.gameObject.GetComponentInParent<PlayerTime>();
                dashreset.ResetDash();
                //weaponScript.ResetFire();
                time.AddTime(enemy.timeToAdd);
                enemy.execute();
            }
            else{
                enemy.currentHealth -= enemy.maxHealth * (executeThreshold/ 2);
                healthbar.UpdateHealthBar(enemy.currentHealth, enemy.maxHealth, executeThreshold, enemy.invulnerable);
            }
        }
    }
}
