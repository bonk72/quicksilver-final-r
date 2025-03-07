using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class powerUpScreen : MonoBehaviour
{
    public List<GameObject> powerUps;

    public Transform[] spawnPoints;
    private List<GameObject> temp = new List<GameObject>();

    void Start()
    {

        
        foreach(RectTransform point in spawnPoints){
            GameObject powerup = powerUps[Random.Range(0, powerUps.Count)];
            //temp.Add(powerup);
            //powerUps.Remove(powerup);
            GameObject newButton = Instantiate(powerup, point.anchoredPosition, Quaternion.identity);
            newButton.transform.SetParent(this.gameObject.transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
