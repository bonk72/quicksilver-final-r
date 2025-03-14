using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> turretPrefabs = new List<GameObject>();

    [SerializeField]
    private Transform[] spawnPoints = new Transform[4];

    [SerializeField]
    private GameObject linkedEnemySpawnerObject; // Reference to the GameObject with EnemySpawner component

    private EnemySpawner linkedEnemySpawner; // Cached reference to the EnemySpawner component
    public bool hasSpawned { get; private set; } = false;
    private List<GameObject> spawnedTurrets = new List<GameObject>();

    private void Start()
    {
        // Get and cache the EnemySpawner component
        if (linkedEnemySpawnerObject != null)
        {
            linkedEnemySpawner = linkedEnemySpawnerObject.GetComponent<EnemySpawner>();
        }
        
        // Validate that we have a linked enemy spawner
        if (linkedEnemySpawner == null)
        {
            Debug.LogError("TurretSpawner requires a linked EnemySpawner reference!");
        }
    }

    private void Update()
    {
        // Check if the enemy spawner has spawned enemies but we haven't spawned turrets yet
        if (linkedEnemySpawner != null && linkedEnemySpawner.hasSpawned && !hasSpawned)
        {
            SpawnTurrets();
        }
        
        // Check if the linked enemy spawner has all enemies defeated
        if (linkedEnemySpawner != null && linkedEnemySpawner.allEnemiesDefeated && spawnedTurrets.Count > 0)
        {
            // Destroy all turrets when enemies are defeated
            DestroyAllTurrets();
        }
    }

    private void SpawnTurrets()
    {
        if (hasSpawned) return;

        // Spawn one turret at each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && turretPrefabs.Count > 0)
            {
                // Get random turret prefab
                int randomTurretIndex = Random.Range(0, turretPrefabs.Count);
                GameObject turretPrefab = turretPrefabs[randomTurretIndex];

                // Instantiate the turret at the spawn point and store reference
                GameObject spawnedTurret = Instantiate(turretPrefab, spawnPoint.position, Quaternion.identity);
                spawnedTurrets.Add(spawnedTurret);
            }
        }

        hasSpawned = true;
        StartCoroutine(CleanupDestroyedTurrets());
    }

    private IEnumerator CleanupDestroyedTurrets()
    {
        while (spawnedTurrets.Count > 0)
        {
            // Remove any destroyed turrets from the list
            spawnedTurrets.RemoveAll(turret => turret == null);
            
            yield return new WaitForSeconds(0.5f); // Check every half second
        }
    }

    private void DestroyAllTurrets()
    {
        // Destroy all remaining turrets
        foreach (GameObject turret in spawnedTurrets)
        {
            if (turret != null)
            {
                Destroy(turret);
            }
        }
        
        // Clear the list
        spawnedTurrets.Clear();
    }
}