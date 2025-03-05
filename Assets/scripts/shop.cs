using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class shop : MonoBehaviour
{
    public Slider timeSlider;
    public PlayerTime playerTime;
    public playerGold gold;
    public TMP_Text price;
    private int cost;
    private int timex = 0;


    public int[] costs;

    private float timeRatio;
    public PlayerTime time;
    public Movement movement;


    public bool timeShop;
    public bool speedShop;

    public int timeGain;
    public float speedGain;
    
    // Unique ID to identify this shop instance
    private string shopId;
    
    // Awake is called before Start
    private void Awake()
    {
        // Generate a unique ID for this shop if it doesn't have one yet
        if (string.IsNullOrEmpty(shopId))
        {
            shopId = System.Guid.NewGuid().ToString();
        }
        
        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        cost = costs[timex];
        timeSlider.value = timeRatio;
        price.text = cost.ToString();
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
        if(playerGold.Gold >= cost && timex < costs.Length){
            gold.loseGold(cost);

            cost = costs[timex];
            timex++;
            price.text = cost.ToString();
            UpdateStats();
            UpdateTimeSlider();
        }
        
        
    }
    
    // Updates the timeSlider fill based on player's current time
    private void UpdateTimeSlider()
    {
        if (timeSlider != null && playerTime != null)
        {
            timeRatio = timeSlider.value;
            timeRatio += 0.1f;
            timeSlider.value = timeRatio;
        }
    }
    private void UpdateStats(){
        if(timeShop){
            time.AddMaxTime(timeGain);
        }
        else if (speedShop){
            movement.increaseMaxTime(speedGain);
        }
        
    }
}
