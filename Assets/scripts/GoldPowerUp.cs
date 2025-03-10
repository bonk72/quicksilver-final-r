using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPowerUp : MonoBehaviour
{
    public int goldMult;
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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerGold gold = player.GetComponent<playerGold>();

        gold.incrGoldMult(goldMult);
        GetComponentInParent<powerUpScreen>().ButtonClicked(this.gameObject);

        

    }
}
