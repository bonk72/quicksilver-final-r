using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public list to hold all StraightRoomManager objects - assign in Inspector
    public List<StraightRoomManager> straightRoomManagers = new List<StraightRoomManager>();
    
    // Event detection variables
    public bool useKeyForTesting = true;
    public KeyCode testEventKey = KeyCode.G;
    public KeyCode deleteEventKey = KeyCode.H;
    private int index;
    
    // Camera reference for background color changes
    private Camera mainCamera;

    public bool generatenew;
    // Flag to track if dungeons have been generated
    private bool dungeonsGenerated = false;
    
    void Start()
    {
        // Get reference to the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
        
        GenerateNewFloor(index);
    }
    
    void Update()
    {
        // For testing: Generate dungeons with a key press
        if (useKeyForTesting && Input.GetKeyDown(testEventKey) && !dungeonsGenerated)
        {
            GenerateDungeon(index);
        }
        
        // For testing: Delete dungeons with a key press
        if (useKeyForTesting && Input.GetKeyDown(deleteEventKey) && dungeonsGenerated)
        {
            DeleteAllDungeons();
        }
        if(generatenew){
            GenerateNewFloor(index);
        } 
    }
    
    // Generate dungeons in all StraightRoomManager objects
    public void GenerateDungeon(int i)
    {
        StraightRoomManager manager =  straightRoomManagers[i];
        if (manager != null)
        {
            manager.GenerateDungeonOnDemand();
        }

        dungeonsGenerated = true;
        Debug.Log("Generated dungeons in all StraightRoomManager objects");
    }
    
    // Handle new floor generation - can be called from newFloor objects
    public void GenerateNewFloor(int ind)
    {
        // First delete any existing dungeons
        DeleteAllDungeons();
        
        // Then generate new dungeons
        GenerateDungeon(ind);
        generatenew = false;
        
        // Set camera background color based on current index
        SetCameraBackgroundColor(ind);
        
        index++;

        Debug.Log("New floor generated!");
    }
    
    // Sets the camera background color based on the provided index
    private void SetCameraBackgroundColor(int colorIndex)
    {
        if (mainCamera == null) return;
        
        // ==================== CAMERA BACKGROUND COLORS ====================
        // Add new colors here by adding new cases to the switch statement
        // Format: case [index]: color = new Color(...); break;
        
        Color color;
        switch (colorIndex)
        {
            case 0:
                // FFFFFF (White)
                color = new Color(1f, 1f, 1f);
                break;
            case 1:
                // 4F0000 (Dark Red)
                color = new Color(0.31f, 0f, 0f);
                break;
            case 2: 
                // 0A0071 (Dark blue
                color = new Color(0.039f, 0f, 0.443f);
                break;
            
            // Add more cases here for additional colors
            // case 3: color = new Color(...); break;
            // case 4: color = new Color(...); break;
            
            default:
                // Default to black if index is out of range
                color = Color.black;
                break;
        }
        // ==================== END CAMERA BACKGROUND COLORS ====================
        
        // Apply the color to the camera
        mainCamera.backgroundColor = color;
        Debug.Log($"Camera background color set to index {colorIndex}");
    }
    
    // Delete dungeons in all StraightRoomManager objects
    public void DeleteAllDungeons()
    {
        foreach (StraightRoomManager manager in straightRoomManagers)
        {
            if (manager != null)
            {
                manager.DeleteDungeon();
            }
        }
        
        dungeonsGenerated = false;
        Debug.Log("Deleted dungeons in all StraightRoomManager objects");
    }

}
