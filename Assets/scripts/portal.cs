using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour
{
    public bool isWarp = false;
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
            if(isWarp){
                SceneManager.LoadScene("Main");
            }
            else{
                collision.gameObject.transform.position = new Vector3(0, collision.gameObject.transform.position.y + 98f, 0);
            }
        }
    }
}
