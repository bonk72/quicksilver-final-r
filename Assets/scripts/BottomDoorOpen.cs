using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomDoorOpen : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemySpawner spwner;
    public GameObject bottomDoor;

    // Update is called once per frame
    void Update()
    {
        if (spwner.hasSpawned == true){
            bottomDoor.SetActive(true); 

        }
    }
}
