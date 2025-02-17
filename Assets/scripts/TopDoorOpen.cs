using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDoorOpen : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public GameObject topDoor;

    void Update()
    {
        if (enemySpawner != null && enemySpawner.allEnemiesDefeated)
        {
            topDoor.SetActive(false);
        }
    }   
}
