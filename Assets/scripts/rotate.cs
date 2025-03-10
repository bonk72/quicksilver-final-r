using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public float rotationsPerMinute;
    private float direction;
    public bool randomRot;
    // Start is called before the first frame update
    void Start()
    {
        if(randomRot){
            if(Random.Range(0, 2) == 1){
                direction = -1f;
            }
            else{
                direction = 1f;
            }
        }
        else{
            direction = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0,direction * 6.0f * rotationsPerMinute * Time.deltaTime);
    }
}
