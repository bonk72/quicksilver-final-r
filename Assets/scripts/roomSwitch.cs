using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomSwitch : MonoBehaviour
{
    public bool newRoom;
    public bool finalRoom;
    public bool disable =  true;
    public bool reset;


    


    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("cameramove")){
            gameObject.GetComponent<PlayerTime>().timeTick = true;
            newRoom = true;
        }
        if (other.CompareTag("finalRoom")){
            finalRoom = true;
        }
        if(other.CompareTag("newDungeon")){
            
            transform.position = new Vector3 (0, 0, 0);
            reset = true;
            gameObject.GetComponent<PlayerTime>().timeTick = false;
        }
    }

}

