using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class shop : MonoBehaviour
{
    public Slider slider;
    public PlayerTime playerTime;
    public playerGold gold;
    public TMP_Text price;
    private int cost;
    private static int timex = 0;
    private static int speedx = 0;
    private static bool rev;


    public int[] costs;

    private static float timeRatio;
    private static float speedRatio;
    private static float revRatio;
    public PlayerTime time;
    public Movement movement;


    public bool timeShop;
    public bool speedShop;
    public bool revShop;

    public int timeGain;
    public float speedGain;
    
    // Start is called before the first frame update
    void Start()
    {
        if(timeShop){
            if(timex != costs.Length){
                cost = costs[timex];
                slider.value = timeRatio;
                price.text = cost.ToString();
            }
            else{
                slider.value = 1;
                price.text = "";
            }
        }
        else if (speedShop){
            if(speedx != costs.Length){
                cost = costs[speedx];
                slider.value = speedRatio;
                price.text = cost.ToString();
            }
            else{
                slider.value = 1;
                price.text = "";
            }
        }
        else if (revShop){
            if(!rev){
                cost = costs[0];
                slider.value = revRatio;
                price.text = cost.ToString();
            }
            else {
                slider.value = 1;
                price.text = "";
            }
        }
        
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Call this method when a button on canvas is pressed
    public void OnTimeButtonPressed()
    {
        // TODO: Add click validation here
        // if (clickIsValid)
        if(timeShop){
            if(playerGold.Gold >= cost && timex < costs.Length + 1){
                gold.loseGold(cost);
                timex += 1;
                if(timex < costs.Length){
                    cost = costs[timex];
                    price.text = cost.ToString();
                }
                else{
                slider.value = 1;
                price.text = "";
            }
                UpdateStats();
                Updateslider();
            }

        }
        else if(speedShop){
            if(playerGold.Gold >= cost && speedx < costs.Length){
                gold.loseGold(cost);
                speedx += 1;
                if(speedx < costs.Length){
                    cost = costs[speedx];
                    price.text = cost.ToString();
                }
                else{
                    slider.value = 1;
                    price.text = "";
                }
                UpdateStats();
                Updateslider();
            }
            else if (speedx == costs.Length){

            }


        }
        else if (revShop){
            if(playerGold.Gold >= cost && !rev){
                gold.loseGold(cost);

                cost = costs[0];
                rev = true;
                price.text = "";
                UpdateStats();
                Updateslider();
            }
        }
        
        
    }
    
    // Updates the slider fill based on player's current time
    private void Updateslider()
    {
        if(timeShop){
            if (slider != null && playerTime != null)
            {
                timeRatio = slider.value;
                timeRatio += 0.1f;
                slider.value = timeRatio;

            }
        }
        else if(speedShop){
            if (slider != null && playerTime != null)
            {
                speedRatio = slider.value;
                speedRatio += 0.1f;
                slider.value = speedRatio;

            }
        }
        else if(revShop){
            if (slider != null && playerTime != null)
            {
                revRatio = 1;
                slider.value = revRatio;

            }
        }
    }
    private void UpdateStats(){
        if(timeShop){
            time.AddMaxTime(timeGain);
        }
        else if (speedShop){
            movement.increaseMaxSpeed(speedGain);
        }
        else if (revShop){
            time.addRev();
        }
        
    }
}
