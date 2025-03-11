using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.SceneManagement; // Add this for scene management

public class powerUpScreen : MonoBehaviour
{
    // Power-up lists by rarity
    public List<GameObject> commonPowerUps;
    public List<GameObject> uncommonPowerUps;
    public List<GameObject> rarePowerUps;

    // Rarity chances (percentages)
    [Range(0, 100)]
    public float commonChance = 70f;
    [Range(0, 100)]
    public float uncommonChance = 20f;
    [Range(0, 100)]
    public float rareChance = 10f;

    public RectTransform[] spawnPoints;
    private List<GameObject> temp = new List<GameObject>();

    public bool selected = false;

    // Static pools for each rarity
    private static List<GameObject> commonPowerUpPool;
    private static List<GameObject> uncommonPowerUpPool;
    private static List<GameObject> rarePowerUpPool;
    private static bool alrSet = false;
    
    // Static constructor to register the scene loaded event
    static powerUpScreen()
    {
        // Register the ResetPowerUpPool method to be called when a scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    // Static method to reset the power-up pools when a scene is loaded
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the static variables when a new scene is loaded
        commonPowerUpPool = null;
        uncommonPowerUpPool = null;
        rarePowerUpPool = null;
        alrSet = false;
    }

    private GameObject pickedPowerUp;

    // Method to select a power-up based on rarity from the main pools
    private GameObject SelectPowerUpByRarity()
    {
        // This method now uses the main pools (not the temporary ones)
        // It's used when we need to select a power-up outside of the Start method
        
        // Normalize chances if they don't add up to 100
        float totalChance = commonChance + uncommonChance + rareChance;
        if (totalChance <= 0) {
            // Default to equal chances if all are zero
            commonChance = uncommonChance = rareChance = 33.33f;
            totalChance = 100f;
        } else if (totalChance != 100f) {
            // Normalize to 100%
            float factor = 100f / totalChance;
            commonChance *= factor;
            uncommonChance *= factor;
            rareChance *= factor;
        }
        
        // Roll for rarity
        float roll = Random.Range(0f, 100f);
        
        // Determine which pool to use based on the roll
        List<GameObject> selectedPool;
        
        if (roll < commonChance) {
            // Common (70% chance)
            selectedPool = commonPowerUpPool;
        } else if (roll < commonChance + uncommonChance) {
            // Uncommon (20% chance)
            selectedPool = uncommonPowerUpPool;
        } else {
            // Rare (10% chance)
            selectedPool = rarePowerUpPool;
        }
        
        // If the selected pool is empty or null, fall back to other pools
        if (selectedPool == null || selectedPool.Count == 0) {
            // Try each pool in order of availability
            if (commonPowerUpPool != null && commonPowerUpPool.Count > 0)
                selectedPool = commonPowerUpPool;
            else if (uncommonPowerUpPool != null && uncommonPowerUpPool.Count > 0)
                selectedPool = uncommonPowerUpPool;
            else if (rarePowerUpPool != null && rarePowerUpPool.Count > 0)
                selectedPool = rarePowerUpPool;
            else
                return null; // No power-ups available
        }
        
        // Select a random power-up from the chosen pool
        if (selectedPool.Count > 0) {
            int randomIndex = Random.Range(0, selectedPool.Count);
            return selectedPool[randomIndex];
        }
        
        return null; // No power-ups available
    }
    
    // List to track power-ups that have been selected for the current set of options
    private List<GameObject> selectedPowerUps = new List<GameObject>();
    
    void Start()
    {
        // Initialize pools if they're null or not yet set
        if (!alrSet) {
            // Create new lists to avoid modifying the originals
            commonPowerUpPool = new List<GameObject>(commonPowerUps);
            uncommonPowerUpPool = new List<GameObject>(uncommonPowerUps);
            rarePowerUpPool = new List<GameObject>(rarePowerUps);
            alrSet = true;
        }
        
        // Ensure we have power-ups to choose from in each pool
        if (commonPowerUpPool == null && commonPowerUps.Count > 0)
            commonPowerUpPool = new List<GameObject>(commonPowerUps);
            
        if (uncommonPowerUpPool == null && uncommonPowerUps.Count > 0)
            uncommonPowerUpPool = new List<GameObject>(uncommonPowerUps);
            
        if (rarePowerUpPool == null && rarePowerUps.Count > 0)
            rarePowerUpPool = new List<GameObject>(rarePowerUps);
        
        // Clear the list of selected power-ups
        selectedPowerUps.Clear();
        
        // Create a temporary copy of each pool to work with
        List<GameObject> tempCommonPool = new List<GameObject>(commonPowerUpPool);
        List<GameObject> tempUncommonPool = new List<GameObject>(uncommonPowerUpPool);
        List<GameObject> tempRarePool = new List<GameObject>(rarePowerUpPool);
        
        // Count how many power-ups are available in total
        int totalAvailablePowerUps =
            (tempCommonPool != null ? tempCommonPool.Count : 0) +
            (tempUncommonPool != null ? tempUncommonPool.Count : 0) +
            (tempRarePool != null ? tempRarePool.Count : 0);
        
        // Determine how many power-ups to spawn
        int powerUpsToSpawn = Mathf.Min(spawnPoints.Length, totalAvailablePowerUps);
        
        // Spawn power-ups at each spawn point
        for (int i = 0; i < powerUpsToSpawn; i++) {
            RectTransform point = spawnPoints[i];
            
            // Select a power-up based on rarity, ensuring it hasn't been selected already
            GameObject powerup = SelectUniqueRandomPowerUp(tempCommonPool, tempUncommonPool, tempRarePool);
            
            if (powerup != null) {
                // Add to temp list for tracking
                temp.Add(powerup);
                
                // Add to selected power-ups list
                selectedPowerUps.Add(powerup);
                
                // Remove from the appropriate temporary pool to prevent duplicates
                if (tempCommonPool.Contains(powerup)) {
                    tempCommonPool.Remove(powerup);
                }
                else if (tempUncommonPool.Contains(powerup)) {
                    tempUncommonPool.Remove(powerup);
                }
                else if (tempRarePool.Contains(powerup)) {
                    tempRarePool.Remove(powerup);
                }
                
                // Instantiate and set parent (using RectTransform for proper UI positioning)
                GameObject newButton = Instantiate(powerup, point.anchoredPosition, Quaternion.identity);
                newButton.transform.SetParent(this.gameObject.transform, false);
                
                // Reset the position to match the spawn point's anchored position
                RectTransform buttonRect = newButton.GetComponent<RectTransform>();
                if (buttonRect != null) {
                    buttonRect.anchoredPosition = point.anchoredPosition;
                }
                
                // Store the mapping between button and prefab
                buttonToPrefabMap[newButton] = powerup;
            }
        }
        
        // We no longer remove power-ups from the actual pools here
        // They will only be removed when they are actually selected by the player
        
        temp.Clear();
    }
    
    // Method to select a unique random power-up that hasn't been selected already
    private GameObject SelectUniqueRandomPowerUp(List<GameObject> tempCommonPool, List<GameObject> tempUncommonPool, List<GameObject> tempRarePool)
    {
        // First, check if we have any power-ups available at all
        bool hasCommon = tempCommonPool != null && tempCommonPool.Count > 0;
        bool hasUncommon = tempUncommonPool != null && tempUncommonPool.Count > 0;
        bool hasRare = tempRarePool != null && tempRarePool.Count > 0;
        
        // If no power-ups are available in any pool, return null
        if (!hasCommon && !hasUncommon && !hasRare) {
            return null;
        }
        
        // Calculate effective chances based on available power-ups
        float effectiveCommonChance = hasCommon ? commonChance : 0f;
        float effectiveUncommonChance = hasUncommon ? uncommonChance : 0f;
        float effectiveRareChance = hasRare ? rareChance : 0f;
        
        // Normalize chances if they don't add up to 100
        float totalChance = effectiveCommonChance + effectiveUncommonChance + effectiveRareChance;
        if (totalChance <= 0) {
            // If no chances are available, distribute evenly among available pools
            int availablePools = (hasCommon ? 1 : 0) + (hasUncommon ? 1 : 0) + (hasRare ? 1 : 0);
            float equalChance = 100f / availablePools;
            
            effectiveCommonChance = hasCommon ? equalChance : 0f;
            effectiveUncommonChance = hasUncommon ? equalChance : 0f;
            effectiveRareChance = hasRare ? equalChance : 0f;
            
            totalChance = 100f;
        } else if (totalChance != 100f) {
            // Normalize to 100%
            float factor = 100f / totalChance;
            effectiveCommonChance *= factor;
            effectiveUncommonChance *= factor;
            effectiveRareChance *= factor;
        }
        
        // Roll for rarity
        float roll = Random.Range(0f, 100f);
        
        // Determine which pool to use based on the roll
        List<GameObject> selectedPool = null;
        
        if (roll < effectiveCommonChance) {
            // Common chance
            selectedPool = tempCommonPool;
        } else if (roll < effectiveCommonChance + effectiveUncommonChance) {
            // Uncommon chance
            selectedPool = tempUncommonPool;
        } else {
            // Rare chance
            selectedPool = tempRarePool;
        }
        
        // Select a random power-up from the chosen pool
        if (selectedPool != null && selectedPool.Count > 0) {
            int randomIndex = Random.Range(0, selectedPool.Count);
            return selectedPool[randomIndex];
        }
        
        // If we get here, something went wrong with our logic
        // Try any available pool as a fallback
        if (hasCommon) {
            selectedPool = tempCommonPool;
        } else if (hasUncommon) {
            selectedPool = tempUncommonPool;
        } else if (hasRare) {
            selectedPool = tempRarePool;
        }
        
        // Select a random power-up from the fallback pool
        if (selectedPool != null && selectedPool.Count > 0) {
            int randomIndex = Random.Range(0, selectedPool.Count);
            return selectedPool[randomIndex];
        }
        
        return null; // This should never happen if the checks above are correct
    }

    // Update is called once per frame
    void Update()
    {
        // We don't need to do anything here anymore since we're handling
        // the removal of power-ups in the Start method and ButtonClicked method
    }
    
    // Dictionary to map instantiated buttons to their original prefabs
    private Dictionary<GameObject, GameObject> buttonToPrefabMap = new Dictionary<GameObject, GameObject>();
    
    public void ButtonClicked(GameObject buttonInstance){
        selected = true;
        
        // Store the button instance
        pickedPowerUp = buttonInstance;
        
        // Find the original prefab for this button using the mapping we created
        if (buttonToPrefabMap.TryGetValue(buttonInstance, out GameObject prefab)) {
            // Now remove the selected power-up from the appropriate pool
            if (commonPowerUpPool != null && commonPowerUpPool.Contains(prefab)) {
                commonPowerUpPool.Remove(prefab);
            }
            else if (uncommonPowerUpPool != null && uncommonPowerUpPool.Contains(prefab)) {
                uncommonPowerUpPool.Remove(prefab);
            }
            else if (rarePowerUpPool != null && rarePowerUpPool.Contains(prefab)) {
                rarePowerUpPool.Remove(prefab);
            }
        }
    }
    public void Disable(){
        powerUP parentPowerUp = GetComponentInParent<powerUP>();
        parentPowerUp.EnableMovement();
        gameObject.SetActive(false);
    }
    
    // Called when the application is quitting
    void OnApplicationQuit()
    {
        // Unregister from the scene loaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
