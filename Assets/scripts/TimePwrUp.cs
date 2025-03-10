using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePwrUp : MonoBehaviour
{
    public int revAdd;
    public int timeAdd;
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
        PlayerTime Time = player.GetComponent<PlayerTime>();


        Time.AddTime(timeAdd);
        Time.addCurrentRev(revAdd);

        GetComponentInParent<powerUpScreen>().ButtonClicked(this.gameObject);
        

    }
}
