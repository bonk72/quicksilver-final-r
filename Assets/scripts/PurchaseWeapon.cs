using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseWeapon : MonoBehaviour
{
    public static int currentWeapon = 0;
    public bool purchased = false;
    public GameObject toBuy;
    public TMP_Text price;
    public GameObject weaponSold;

    public GameObject weaponImage;
    public GameObject toActivate;
    
    private bool playerInRange = false;
    public weapon weap;
    public playerGold gold;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange && purchased && Input.GetKeyDown(KeyCode.O))
        {
            weap.ChangeWeapon(weaponSold.GetComponent<bullet>().index);
            toActivate.SetActive(false);
        }
        if(playerInRange && !purchased && Input.GetKeyDown(KeyCode.P) && (weaponSold.GetComponent<bullet>().price <= playerGold.Gold))
        {

            gold.loseGold(weaponSold.GetComponent<bullet>().price);
            purchased = true;
            toBuy.SetActive(false);
            weaponImage.GetComponent<Image>().sprite = weaponSold.GetComponent<SpriteRenderer>().sprite;
            toActivate.SetActive(true);

        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            if(!purchased){
                toBuy.SetActive(true);
                price.text = weaponSold.GetComponent<bullet>().price.ToString();
            }
            else{
                if(weapon.currentWeapon != weaponSold.GetComponent<bullet>().index)
                    toActivate.SetActive(true);
                    weaponImage.GetComponent<Image>().sprite = weaponSold.GetComponent<SpriteRenderer>().sprite;
            }
        }
        
    }
    void OnTriggerExit2D(Collider2D collision)
    {
       if(collision.gameObject.CompareTag("Player")){
            playerInRange = false; 
            if(!purchased)
            {
                toBuy.SetActive(false);
            } 
            else{
                toActivate.SetActive(false);
            }

       }
            
    }
}

