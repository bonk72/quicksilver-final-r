using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class execute : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyHealth enemy;
    

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
            if (enemy.currentHealth <= enemy.maxHealth * .3)
            {
                Movement dashreset =  other.gameObject.GetComponentInParent<Movement>();
                dashreset.ResetDash();
                
                enemy.execute();
            }

        }
            
    }
}
