using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnTurrets : MonoBehaviour
{
    public List<GameObject> turrets;
    public Transform anchorPoint;

    public int minSpawnDelay;
    public int maxSpawnDelay;
    public float spawnRadius = 5f;
    public float turretLifetime = 10f;

    private bool isSpawning = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnTurretsRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnTurretsRoutine()
    {
        while (isSpawning)
        {
            // Wait for a random time between min and max delay
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay + 1);
            yield return new WaitForSeconds(delay);

            // Spawn a turret at a random position within the radius
            SpawnTurret();
        }
    }

    public void SpawnTurret()
    {
        if (turrets.Count == 0 || anchorPoint == null)
            return;

        // Get a random turret from the list
        GameObject turretPrefab = turrets[Random.Range(0, turrets.Count)];
        
        // Calculate a random position within the spawn radius
        Vector2 randomDirection = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = anchorPoint.position + new Vector3(randomDirection.x, randomDirection.y, 0);
        
        // Instantiate the turret
        GameObject spawnedTurret = Instantiate(turretPrefab, spawnPosition, Quaternion.identity);
        
        // Destroy the turret after its lifetime
        Destroy(spawnedTurret, turretLifetime);
    }
}
