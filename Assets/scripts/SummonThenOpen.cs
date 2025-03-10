using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonThenOpen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject powerup;
    public GameObject door;
    private GameObject pup;
    void Start()
    {
        // Instantiate the powerup
        if (powerup != null) {
            pup = Instantiate(powerup, transform.position, Quaternion.identity);

        } 
    }

    // Update is called once per frame
    void Update()
    {
        // Check if pup is not null
        if (pup == null)
            return;
            
        // Get the powerUpScreen component
        powerUpScreen powerUpScreenComponent = pup.GetComponentInChildren<powerUpScreen>();
        
        // Check if the powerUpScreen component exists and is selected
        if (powerUpScreenComponent != null && powerUpScreenComponent.selected) {
            door.SetActive(false);
            powerUpScreenComponent.Disable();
            gameObject.SetActive(false);
        }
    }
}
