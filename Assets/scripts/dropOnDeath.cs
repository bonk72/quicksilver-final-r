using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropOnDeath : MonoBehaviour
{
    public GameObject item;
    public Transform location;
    public int dropAmount = 1; // Integer variable to control how many items to drop
    public bool scatterDrops = false; // Boolean to determine if drops should be scattered
    public float scatterRadius = 1.5f; // Radius for scattering drops
    public float spawnDelay = 0.1f; // Delay between spawning items when scattered
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DropDeath(){
        if (item == null) return; // Exit if no item to drop
        
        Vector3 basePosition;
        if (location != null) {
            basePosition = location.position;
        } else {
            // If location is not specified, use this object's position
            basePosition = transform.position;
        }
        
        if (scatterDrops) {
            // Use coroutine for sequential, scattered spawning
            StartCoroutine(DropItemsSequentially(basePosition));
        } else {
            // Instantiate all items at the same position immediately
            for (int i = 0; i < dropAmount; i++) {
                Instantiate(item, basePosition, Quaternion.identity);
            }
        }
    }
    
    private IEnumerator DropItemsSequentially(Vector3 basePosition) {
        for (int i = 0; i < dropAmount; i++) {
            // Create a random offset within the scatter radius
            Vector2 randomOffset = Random.insideUnitCircle * scatterRadius;
            Vector3 spawnPosition = basePosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // Instantiate the item at the scattered position
            Instantiate(item, spawnPosition, Quaternion.identity);
            
            // Wait before spawning the next item
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
