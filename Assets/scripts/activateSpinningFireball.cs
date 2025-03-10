using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateSpinningFireball : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonPress()
    {

        GameObject fireball = GameObject.FindGameObjectWithTag("powerupManager");
        fireball.GetComponent<PowerUpManager>().fireballActivate();

        GetComponentInParent<powerUpScreen>().ButtonClicked(this.gameObject);

    }
}
