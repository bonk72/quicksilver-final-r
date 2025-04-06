using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseWeapon : MonoBehaviour
{
    // Static dictionary to store purchase states across scene changes
    // Key: weaponId, Value: purchased state
    private static Dictionary<string, bool> purchaseStates = new Dictionary<string, bool>();
    
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
    
    // Unique identifier for this weapon purchase object
    public string weaponId;

    // Start is called before the first frame update
    void Start()
    {
        // If weaponId is not set, use the object's name as a fallback
        if (string.IsNullOrEmpty(weaponId))
        {
            weaponId = gameObject.name;
        }
        
        // Load the purchase state from static dictionary
        LoadPurchaseState();
        
        // Update UI based on loaded state
        if (purchased)
        {
            toBuy.SetActive(false);
        }
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
            
            // Save the purchase state to the static dictionary
            SavePurchaseState();
            
            toBuy.SetActive(false);
            weaponImage.GetComponent<Image>().sprite = weaponSold.GetComponent<SpriteRenderer>().sprite;
            toActivate.SetActive(true);
        }
    }
    
    // Save the purchase state to the static dictionary
    private void SavePurchaseState()
    {
        purchaseStates[weaponId] = purchased;
    }
    
    // Load the purchase state from the static dictionary
    private void LoadPurchaseState()
    {
        // If the weapon has been purchased before, set purchased to true
        if (purchaseStates.ContainsKey(weaponId))
        {
            purchased = purchaseStates[weaponId];
        }
        // Otherwise, leave it as the default value (false)
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

