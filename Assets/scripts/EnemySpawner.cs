using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();

    [SerializeField]
    private Transform[] spawnPoints = new Transform[4];

    public bool hasSpawned { get; private set; } = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    public bool allEnemiesDefeated { get; private set; } = false;
    public bool delColliders;

    public GameObject rewardPrefab; // The prefab to spawn after enemies are defeated
    public int minRewardSpawnCount = 3; // Number of rewards to spawn
    public int maxRewardsSpawnCount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's the player and enemies haven't spawned yet
        if (other.CompareTag("Player") && !hasSpawned)
        {
            SpawnEnemies();
            if (delColliders){
                GetComponent<Collider2D>().enabled = false;                
            }

            hasSpawned = true;
            StartCoroutine(CheckEnemiesStatus());
            // Disable the collider after spawning
            
        }
    }

    private void SpawnEnemies()
    {
        // Spawn one enemy at each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && enemyPrefabs.Count > 0)
            {
                // Get random enemy prefab
                int randomEnemyIndex = Random.Range(0, enemyPrefabs.Count);
                GameObject enemyPrefab = enemyPrefabs[randomEnemyIndex];

                // Instantiate the enemy at the spawn point and store reference
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                spawnedEnemies.Add(spawnedEnemy);
            }
        }
    }

    private IEnumerator CheckEnemiesStatus()
    {
        while (!allEnemiesDefeated)
        {
            // Remove any destroyed enemies from the list
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            // Check if all enemies are destroyed
            if (spawnedEnemies.Count == 0 && hasSpawned)
            {
                allEnemiesDefeated = true;
                StartCoroutine(SpawnRewardsSequentially()); // Start sequential spawning
            }

            yield return new WaitForSeconds(0.5f); // Check every half second
        }
    }

    public float spawnDelay = 0.2f; // Delay between each reward spawn

    private IEnumerator SpawnRewardsSequentially()
    {
        int rewardSpawnCount = Random.Range(minRewardSpawnCount, maxRewardsSpawnCount);
        if (rewardPrefab == null || rewardSpawnCount <= 0) yield break;

        for (int i = 0; i < rewardSpawnCount; i++)
        {
            // Create a random position around the enemy spawner
            Vector2 randomOffset = Random.insideUnitCircle * 3f; // 3 units radius for better spread
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Instantiate the reward
            Instantiate(rewardPrefab, spawnPosition, Quaternion.identity);

            // Wait before spawning the next reward
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
