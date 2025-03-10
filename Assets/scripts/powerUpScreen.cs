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

    public Transform[] spawnPoints;
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

    // Method to select a power-up based on rarity
    private GameObject SelectPowerUpByRarity()
    {
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
            // Try each pool in order of rarity (common -> uncommon -> rare)
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
        
        // Spawn power-ups at each spawn point
        foreach(RectTransform point in spawnPoints) {
            // Select a power-up based on rarity
            GameObject powerup = SelectPowerUpByRarity();
            
            if (powerup != null) {
                // Add to temp list and remove from appropriate pool
                temp.Add(powerup);
                
                // Remove from the appropriate pool
                if (commonPowerUpPool != null && commonPowerUpPool.Contains(powerup))
                    commonPowerUpPool.Remove(powerup);
                else if (uncommonPowerUpPool != null && uncommonPowerUpPool.Contains(powerup))
                    uncommonPowerUpPool.Remove(powerup);
                else if (rarePowerUpPool != null && rarePowerUpPool.Contains(powerup))
                    rarePowerUpPool.Remove(powerup);
                
                // Instantiate and set parent
                GameObject newButton = Instantiate(powerup, point.anchoredPosition, Quaternion.identity);
                newButton.transform.SetParent(this.gameObject.transform, false);
            }
        }
        
        // Return power-ups to their respective pools
        foreach(GameObject powerup in temp) {
            // Add back to the appropriate pool
            if (commonPowerUps.Contains(powerup))
                commonPowerUpPool.Add(powerup);
            else if (uncommonPowerUps.Contains(powerup))
                uncommonPowerUpPool.Add(powerup);
            else if (rarePowerUps.Contains(powerup))
                rarePowerUpPool.Add(powerup);
        }
        temp.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!selected && powerUps != null && powerUps.Count > 0)
        //{
        //    // Check each power-up in the list
        //    for (int i = 0; i < powerUps.Count; i++)
        //    {
        //        // Make sure the power-up exists before trying to access it
        //        if (powerUps[i] != null)
        //        {
        //            AttackPwrUp attackPowerUp = powerUps[i].GetComponent<AttackPwrUp>();
        //            
        //            // Check if the component exists and its status
        //            if (attackPowerUp != null && attackPowerUp.getStatus())
        //            {
        //                // Try to get the parent powerUP component
        //                powerUP parentPowerUp = GetComponentInParent<powerUP>();
        //                if (parentPowerUp != null)
        //                {
        //                    parentPowerUp.EnableMovement();
        //                }
        //                
        //                selected = true;
        //                gameObject.SetActive(false);
        //                break; // Exit the loop once we've found a selected power-up
        //            }
        //        }
        //    }
        //}
        if (selected && pickedPowerUp != null) {
            // Remove the picked power-up from the appropriate pool
            if (commonPowerUpPool != null && commonPowerUpPool.Contains(pickedPowerUp)) {
                commonPowerUpPool.Remove(pickedPowerUp);
            }
            else if (uncommonPowerUpPool != null && uncommonPowerUpPool.Contains(pickedPowerUp)) {
                uncommonPowerUpPool.Remove(pickedPowerUp);
            }
            else if (rarePowerUpPool != null && rarePowerUpPool.Contains(pickedPowerUp)) {
                rarePowerUpPool.Remove(pickedPowerUp);
            }
        }
    }
    public void ButtonClicked(GameObject powerup){
        selected = true;
        pickedPowerUp = powerup;
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
