using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StraightRoomManager : MonoBehaviour
{
    public int maxRooms = 10;
    public int minRooms = 8;
    public List<GameObject> possibleRoomPrefabs = new List<GameObject>();
    public List<GameObject> verticalHallwayPrefabs = new List<GameObject>();
    public GameObject startingRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject finalRoomPrefab;

    private int roomWidth = 34;
    private int roomHeight = 28;
    private int hallwayWidth = 17;
    private int hallwayHeight = 28;
    private int bossRoomWidth = 35;
    private int bossRoomHeight = 33;

    private List<GameObject> roomObjects = new List<GameObject>();
    private bool[,] roomGrid;
    private RoomType[,] roomTypes;
    private Vector2Int startRoomPos;
    private Vector2Int? bossRoomPos;
    private int consecutiveNormalRooms = 0;
    public bool DungeonGenerated = false;

    private enum RoomType
    {
        None,
        Normal,
        Hallway,
        Boss,
        Start,
        Final
    }

    private void Start()
    {
        // Validate prefabs but don't generate dungeon automatically
        if (startingRoomPrefab == null || possibleRoomPrefabs.Count == 0 ||
            bossRoomPrefab == null || finalRoomPrefab == null)
        {
            Debug.LogError("Please assign all required room prefabs in the inspector!");
            return;
        }
    }
    void Update()
    {
        //if (Input.GetKeyDown("space") && !DungeonGenerated){
        //    DungeonGenerated = true;
            
            
        //}
    }

    private void GenerateDungeon()
    {
        roomGrid = new bool[3, 20]; // Increased height to ensure space for boss and final rooms
        roomTypes = new RoomType[3, 20];
        consecutiveNormalRooms = 0;
        
        // Place starting room at (0,0) in grid coordinates
        startRoomPos = new Vector2Int(1, 0);
        PlaceRoom(startRoomPos, startingRoomPrefab, RoomType.Start);

        // Generate normal rooms (not counting boss room)
        int normalRoomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = 1000;
        int normalRoomsGenerated = 1; // Starting room counts as first normal room

        while (normalRoomsGenerated < normalRoomsToGenerate && attempts < maxAttempts)
        {
            Vector2Int? nextPos = GetNextPosition();
            if (nextPos.HasValue)
            {
                // Try to place a hallway first if conditions are met
                if (verticalHallwayPrefabs.Count > 0 &&
                    (consecutiveNormalRooms >= 3 || Random.value < 0.4f) &&
                    CanPlaceHallway(nextPos.Value))
                {
                    GameObject hallwayPrefab = verticalHallwayPrefabs[Random.Range(0, verticalHallwayPrefabs.Count)];
                    PlaceRoom(nextPos.Value, hallwayPrefab, RoomType.Hallway);
                    normalRoomsGenerated++;
                    consecutiveNormalRooms = 0;
                }
                else
                {
                    GameObject randomRoomPrefab = possibleRoomPrefabs[Random.Range(0, possibleRoomPrefabs.Count)];
                    PlaceRoom(nextPos.Value, randomRoomPrefab, RoomType.Normal);
                    normalRoomsGenerated++;
                    consecutiveNormalRooms++;
                }
            }
            attempts++;
        }

        PlaceFinalRoom();
        PlaceBossRoom();
    }

    private void PlaceFinalRoom()
    {
        // Find the highest room
        int highestY = 0;
        for (int y = roomGrid.GetLength(1) - 1; y >= 0; y--)
        {
            if (roomGrid[startRoomPos.x, y])
            {
                highestY = y;
                break;
            }
        }

        // Place final room above the highest room
        Vector2Int finalPos = new Vector2Int(startRoomPos.x, highestY + 1);
        
        if (IsValidPosition(finalPos))
        {
            roomGrid[finalPos.x, finalPos.y] = true;
            roomTypes[finalPos.x, finalPos.y] = RoomType.Final;
            
            Vector2Int worldPos = GetPositionFromGridIndex(finalPos);
            Vector3 position = new Vector3(worldPos.x, worldPos.y, 0);
            
            GameObject finalRoom = Instantiate(finalRoomPrefab, position, Quaternion.identity);
            roomObjects.Add(finalRoom);
        }
    }

    private bool CanPlaceHallway(Vector2Int pos)
    {
        // Must have normal rooms both above AND below
        bool hasNormalRoomAbove = IsInGrid(new Vector2Int(pos.x, pos.y + 1)) &&
                                 roomTypes[pos.x, pos.y + 1] == RoomType.Normal;
        bool hasNormalRoomBelow = IsInGrid(new Vector2Int(pos.x, pos.y - 1)) &&
                                 roomTypes[pos.x, pos.y - 1] == RoomType.Normal;

        return hasNormalRoomAbove || hasNormalRoomBelow;
    }

    private Vector2Int? GetNextPosition()
    {
        // Find the highest placed room
        int highestY = 0;
        for (int y = roomGrid.GetLength(1) - 1; y >= 0; y--)
        {
            if (roomGrid[startRoomPos.x, y])
            {
                highestY = y;
                break;
            }
        }

        // Try to place above the highest room
        Vector2Int nextPos = new Vector2Int(startRoomPos.x, highestY + 1);
        if (IsValidPosition(nextPos))
        {
            return nextPos;
        }

        return null;
    }

    private bool IsInGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < roomGrid.GetLength(0) &&
               pos.y >= 0 && pos.y < roomGrid.GetLength(1);
    }

    private void PlaceBossRoom()
    {
        // Find the highest room
        int highestY = 0;
        for (int y = roomGrid.GetLength(1) - 1; y >= 0; y--)
        {
            if (roomGrid[startRoomPos.x, y])
            {
                highestY = y;
                break;
            }
        }

        // Place boss room at a fixed distance above the highest room
        Vector2Int bossPos = new Vector2Int(startRoomPos.x, highestY + 4);
        
        // Ensure we're not too close to the grid edge
        if (bossPos.y >= roomGrid.GetLength(1) - 2)
        {
            bossPos.y = roomGrid.GetLength(1) - 3;
        }
        
        // Place the boss room
        bossRoomPos = bossPos;
        roomGrid[bossPos.x, bossPos.y] = true;
        roomTypes[bossPos.x, bossPos.y] = RoomType.Boss;
        
        Vector2Int worldPos = GetPositionFromGridIndex(bossPos);
        float xOffset = (bossRoomWidth - roomWidth) / 2f;
        float yOffset = (bossRoomHeight - roomHeight) / 2f;
        Vector3 adjustedPos = new Vector3(worldPos.x + xOffset, worldPos.y + yOffset, 0);
        
        GameObject bossRoom = Instantiate(bossRoomPrefab, adjustedPos, Quaternion.identity);
        roomObjects.Add(bossRoom);
    }

    private bool IsValidBossPosition(Vector2Int pos)
    {
        if (!IsInGrid(pos)) return false;

        // Check area below the boss room
        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 2; y <= pos.y; y++)
            {
                if (IsInGrid(new Vector2Int(x, y)))
                {
                    if (roomGrid[x, y])
                    {
                        return false;
                    }
                }
            }
        }

        // For vertical position, only ensure we're not at the very top
        if (pos.y >= roomGrid.GetLength(1) - 2)
        {
            return false;
        }

        return true;
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        return IsInGrid(pos) && !roomGrid[pos.x, pos.y];
    }

    private void PlaceRoom(Vector2Int gridPos, GameObject roomPrefab, RoomType type)
    {
        Vector2Int worldPos = GetPositionFromGridIndex(gridPos);
        Vector3 position = new Vector3(worldPos.x, worldPos.y, 0);

        // Center hallways on the x-position of the room above
        if (type == RoomType.Hallway)
        {
            Vector2Int abovePos = new Vector2Int(gridPos.x, gridPos.y + 1);
            if (IsInGrid(abovePos) && roomTypes[abovePos.x, abovePos.y] == RoomType.Normal)
            {
                Vector2Int aboveWorldPos = GetPositionFromGridIndex(abovePos);
                position.x = aboveWorldPos.x + (roomWidth - hallwayWidth) / 2f;
            }
        }

        GameObject room = Instantiate(roomPrefab, position, Quaternion.identity);
        roomObjects.Add(room);
        roomGrid[gridPos.x, gridPos.y] = true;
        roomTypes[gridPos.x, gridPos.y] = type;
    }

    private Vector2Int GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector2Int(
            roomWidth * (gridX - 1),
            roomHeight * gridY
        );
    }

    private void OnDrawGizmos()
    {
        if (roomGrid == null) return;

        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < roomGrid.GetLength(0); x++)
        {
            for (int y = 0; y < roomGrid.GetLength(1); y++)
            {
                Vector2Int position = GetPositionFromGridIndex(new Vector2Int(x, y));
                
                if (roomTypes != null)
                {
                    switch (roomTypes[x, y])
                    {
                        case RoomType.Normal:
                        case RoomType.Start:
                        case RoomType.Final:
                            Gizmos.DrawWireCube(new Vector3(position.x, position.y), 
                                              new Vector3(roomWidth, roomHeight, 1));
                            break;
                        case RoomType.Hallway:
                            Gizmos.DrawWireCube(new Vector3(position.x + (roomWidth - hallwayWidth)/2f, position.y), 
                                              new Vector3(hallwayWidth, hallwayHeight, 1));
                            break;
                        case RoomType.Boss:
                            Gizmos.DrawWireCube(new Vector3(position.x + (bossRoomWidth - roomWidth)/2f, 
                                                          position.y + (bossRoomHeight - roomHeight)/2f, 0),
                                              new Vector3(bossRoomWidth, bossRoomHeight, 1));
                            break;
                    }
                }
            }
        }
    }
    
    // Public method to generate the dungeon when called from GameManager
    public void GenerateDungeonOnDemand()
    {
        // Clear any existing dungeon first
        DeleteDungeon();
        
        // Generate a new dungeon
        GenerateDungeon();
        DungeonGenerated = true;
    }
    
    // Public method to delete the dungeon
    public void DeleteDungeon()
    {
        // Tag for gold collectibles that should not be destroyed

        
        // First, destroy all room objects and their children (except gold collectibles)
        foreach (GameObject room in roomObjects)
        {
            if (room != null)
            {
                // Get all child objects before destroying the parent
                Transform[] allChildren = room.GetComponentsInChildren<Transform>(true);
                
                // Create a list to hold objects that should be detached (gold collectibles)
                List<GameObject> objectsToDetach = new List<GameObject>();
                
                // Find all gold collectibles and detach them from the room hierarchy
                foreach (Transform child in allChildren)
                {
                    if (child.CompareTag("collectible"))
                    {
                        // Detach from parent so it doesn't get destroyed with the room
                        objectsToDetach.Add(child.gameObject);
                    }
                }
                
                // Detach gold collectibles from their parents
                foreach (GameObject obj in objectsToDetach)
                {
                    obj.transform.SetParent(null);
                }
                
                // Now destroy the room and all its remaining children
                Destroy(room);
            }
        }
        
        // Find and destroy any enemies or other objects that might have been spawned
        // but are not direct children of room objects
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in allEnemies)
        {
            // Skip if it's already destroyed
            if (enemy == null) continue;
            
            // Destroy the enemy
            Destroy(enemy);
        }
        
        
        // Clear the list
        roomObjects.Clear();
        
        // Reset the grid
        if (roomGrid != null)
        {
            for (int x = 0; x < roomGrid.GetLength(0); x++)
            {
                for (int y = 0; y < roomGrid.GetLength(1); y++)
                {
                    roomGrid[x, y] = false;
                    roomTypes[x, y] = RoomType.None;
                }
            }
        }
        
        DungeonGenerated = false;
        Debug.Log("Dungeon deleted, gold collectibles preserved");
    }
}