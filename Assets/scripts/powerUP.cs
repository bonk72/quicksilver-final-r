using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUP : MonoBehaviour
{
    public GameObject screen;

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
            screen.SetActive(true);
            collision.gameObject.GetComponent<Movement>().SetMovement(false);
        }
        
    }
    public void EnableMovement(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.gameObject.GetComponent<Movement>().SetMovement(true);
        Destroy(gameObject);
    }

    

}
