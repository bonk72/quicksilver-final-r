using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropOnDeath : MonoBehaviour
{
    public GameObject item;
    public Transform location;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DropDeath(){
        if (item != null && location != null) {
            Instantiate(item, location.position, Quaternion.identity);
        } else if (item != null) {
            // If location is not specified, use this object's position
            Instantiate(item, transform.position, Quaternion.identity);
        }
    }
}
