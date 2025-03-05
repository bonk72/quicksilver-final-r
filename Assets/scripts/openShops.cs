using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class openShops : MonoBehaviour
{
    public GameObject shops;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Activate the shop canvas
            shops.SetActive(true);
            
            // Disable shooting and rotation
            weapon playerWeapon = collision.gameObject.GetComponentInChildren<weapon>();
            if (playerWeapon != null)
            {
                playerWeapon.canShoot = false;
            }
            
            Movement playerMovement = collision.gameObject.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.canRotate = false;
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deactivate the shop canvas
            shops.SetActive(false);
            
            // Re-enable shooting and rotation
            weapon playerWeapon = collision.gameObject.GetComponentInChildren<weapon>();
            if (playerWeapon != null)
            {
                playerWeapon.canShoot = true;
            }
            
            Movement playerMovement = collision.gameObject.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.canRotate = true;
            }
        }
    }
}
