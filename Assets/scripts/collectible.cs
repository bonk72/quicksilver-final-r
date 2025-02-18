using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class collectible : MonoBehaviour
{
    public int timeIncrease;

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
        if(collision.gameObject.CompareTag("Player")){
            collision.gameObject.GetComponent<PlayerTime>().AddTime(timeIncrease);
            Destroy(gameObject);
        }
    }
}
