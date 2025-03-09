using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class powerUpScreen : MonoBehaviour
{
    public List<GameObject> powerUps;

    public Transform[] spawnPoints;
    private List<GameObject> temp = new List<GameObject>();

    private bool selected = false;

    private static List<GameObject> powerUpPool;
    private static bool alrSet;


    private GameObject pickedPowerUp;

    void Start()
    {
        if(!alrSet){
            powerUpPool = powerUps;
        }
        alrSet = true;
        
        foreach(RectTransform point in spawnPoints){
            GameObject powerup = powerUpPool[Random.Range(0, powerUps.Count)];
            temp.Add(powerup);
            powerUpPool.Remove(powerup);
            GameObject newButton = Instantiate(powerup, point.anchoredPosition, Quaternion.identity);
            newButton.transform.SetParent(this.gameObject.transform, false);
        }
        foreach(GameObject powerup in temp){
            powerUpPool.Add(powerup);
            
        }
        temp.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!selected && powerUps != null && powerUps.Count > 0)
        //{
        //    // Check each power-up in the list
        //    for (int i = 0; i < powerUps.Count; i++)
        //    {
        //        // Make sure the power-up exists before trying to access it
        //        if (powerUps[i] != null)
        //        {
        //            AttackPwrUp attackPowerUp = powerUps[i].GetComponent<AttackPwrUp>();
        //            
        //            // Check if the component exists and its status
        //            if (attackPowerUp != null && attackPowerUp.getStatus())
        //            {
        //                // Try to get the parent powerUP component
        //                powerUP parentPowerUp = GetComponentInParent<powerUP>();
        //                if (parentPowerUp != null)
        //                {
        //                    parentPowerUp.EnableMovement();
        //                }
        //                
        //                selected = true;
        //                gameObject.SetActive(false);
        //                break; // Exit the loop once we've found a selected power-up
        //            }
        //        }
        //    }
        //}
        if(selected){
            powerUP parentPowerUp = GetComponentInParent<powerUP>();
            parentPowerUp.EnableMovement();
            powerUpPool.Remove(pickedPowerUp);
            this.gameObject.SetActive(false);
        }
    }
    public void ButtonClicked(GameObject powerup){
        selected = true;
        pickedPowerUp = powerup;
    }
}
