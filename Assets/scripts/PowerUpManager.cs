using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{

    public Transform player;



    public GameObject fireball;
    public GameObject turret;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;

    }
    public void fireballActivate(){
        fireball.SetActive(true);
    }
    public void turretActivate(){

    }

}
