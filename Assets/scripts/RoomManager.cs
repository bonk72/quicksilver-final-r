using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public int maxRooms = 10;
    public int minRooms = 8;
    public List<GameObject> possibleRoomPrefabs = new List<GameObject>();
    public List<GameObject> verticalHallwayPrefabs = new List<GameObject>();
    public GameObject startingRoomPrefab;
    public GameObject bossRoomPrefab;

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

    private enum RoomType
    {
        None,
        Normal,
        Hallway,
        Boss,
        Start
    }

    private void Start()
    {
        if (startingRoomPrefab == null || possibleRoomPrefabs.Count == 0 || bossRoomPrefab == null)
        {
            Debug.LogError("Please assign all required room prefabs in the inspector!");
            return;
        }

        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        roomGrid = new bool[10, 15]; // Increased height for boss room
        roomTypes = new RoomType[10, 15];
        consecutiveNormalRooms = 0;
        
        // Place starting room at (0,0) in grid coordinates
        startRoomPos = new Vector2Int(5, 0);
        PlaceRoom(startRoomPos, startingRoomPrefab, RoomType.Start);

        // Generate minimum number of rooms first
        int roomsToGenerate = Random.Range(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = 1000; // Increased max attempts
        int roomsGenerated = 1;

        // First phase: Generate upward path
        while (roomsGenerated < roomsToGenerate && attempts < maxAttempts)
        {
            Vector2Int? nextPos = GetNextPosition();
            if (nextPos.HasValue && !IsRestrictedPosition(nextPos.Value))
            {
                // Try to place a hallway first if conditions are met
                if (verticalHallwayPrefabs.Count > 0 &&
                    (consecutiveNormalRooms >= 3 || Random.value < 0.4f) &&
                    CanPlaceHallway(nextPos.Value))
                {
                    GameObject hallwayPrefab = verticalHallwayPrefabs[Random.Range(0, verticalHallwayPrefabs.Count)];
                    PlaceRoom(nextPos.Value, hallwayPrefab, RoomType.Hallway);
                    roomsGenerated++;
                    consecutiveNormalRooms = 0;
                }
                // Otherwise place a normal room
                else
                {
                    GameObject randomRoomPrefab = possibleRoomPrefabs[Random.Range(0, possibleRoomPrefabs.Count)];
                    PlaceRoom(nextPos.Value, randomRoomPrefab, RoomType.Normal);
                    roomsGenerated++;
                    consecutiveNormalRooms++;
                }

                // Reset attempts periodically to prevent early termination
                if (attempts % 20 == 0)
                {
                    attempts = Mathf.Max(0, attempts - 10);
                }
            }
            attempts++;
        }

        PlaceBossRoom();
    }

    private bool IsRestrictedPosition(Vector2Int pos)
    {
        // Never allow rooms below the starting room
        if (pos.y < startRoomPos.y) return true;

        // Don't allow rooms directly left or right of starting room
        if (pos.y == startRoomPos.y &&
            (pos.x == startRoomPos.x + 1 || pos.x == startRoomPos.x - 1))
        {
            return true;
        }

        // Don't allow rooms in the same column as the starting room (except above)
        if (pos.x == startRoomPos.x && pos.y <= startRoomPos.y)
        {
            return true;
        }

        // Prevent rooms from generating too far horizontally
        int maxHorizontalDistance = 2;
        if (Mathf.Abs(pos.x - startRoomPos.x) > maxHorizontalDistance)
        {
            return true;
        }

        return false;
    }

    private bool CanPlaceHallway(Vector2Int pos)
    {
        // Must have at least one normal room above or below
        bool hasNormalRoomAbove = IsInGrid(new Vector2Int(pos.x, pos.y + 1)) &&
                                 roomTypes[pos.x, pos.y + 1] == RoomType.Normal;
        bool hasNormalRoomBelow = IsInGrid(new Vector2Int(pos.x, pos.y - 1)) &&
                                 roomTypes[pos.x, pos.y - 1] == RoomType.Normal;

        // Initially allow hallway with just one connection
        if (!hasNormalRoomAbove && !hasNormalRoomBelow) return false;

        // Check for clear space on both sides
        bool hasSpaceLeft = !IsInGrid(new Vector2Int(pos.x - 1, pos.y)) ||
                          !roomGrid[pos.x - 1, pos.y];
        bool hasSpaceRight = !IsInGrid(new Vector2Int(pos.x + 1, pos.y)) ||
                           !roomGrid[pos.x + 1, pos.y];

        // If we have one connection, check if we can potentially connect to another room
        if (hasNormalRoomAbove && !hasNormalRoomBelow)
        {
            // Check if we can place a room below later
            Vector2Int belowPos = new Vector2Int(pos.x, pos.y - 1);
            return hasSpaceLeft && hasSpaceRight && IsValidPosition(belowPos);
        }
        else if (!hasNormalRoomAbove && hasNormalRoomBelow)
        {
            // Check if we can place a room above later
            Vector2Int abovePos = new Vector2Int(pos.x, pos.y + 1);
            return hasSpaceLeft && hasSpaceRight && IsValidPosition(abovePos);
        }

        return hasSpaceLeft && hasSpaceRight;
    }

    private Vector2Int? GetNextPosition()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        List<Vector2Int> upwardPositions = new List<Vector2Int>();
        List<Vector2Int> horizontalPositions = new List<Vector2Int>();

        for (int x = 0; x < roomGrid.GetLength(0); x++)
        {
            for (int y = 0; y < roomGrid.GetLength(1); y++)
            {
                if (roomGrid[x, y])
                {
                    Vector2Int currentPos = new Vector2Int(x, y);
                    Vector2Int[] adjacentPositions = new Vector2Int[]
                    {
                        new Vector2Int(x, y + 1),  // Up
                        new Vector2Int(x + 1, y),  // Right
                        new Vector2Int(x - 1, y),  // Left
                    };

                    foreach (Vector2Int pos in adjacentPositions)
                    {
                        if (IsValidPosition(pos) && !IsRestrictedPosition(pos))
                        {
                            validPositions.Add(pos);
                            
                            if (pos.y > y) // Upward position
                            {
                                // Add multiple times to increase upward probability
                                for (int i = 0; i < 2; i++)
                                {
                                    upwardPositions.Add(pos);
                                }
                            }
                            else if (pos.x != startRoomPos.x) // Horizontal position, not aligned with start room
                            {
                                horizontalPositions.Add(pos);
                            }
                        }
                    }
                }
            }
        }

        // 70% chance to go upward if possible
        if (upwardPositions.Count > 0 && Random.value < 0.7f)
        {
            return upwardPositions[Random.Range(0, upwardPositions.Count)];
        }
        
        // 30% chance to go horizontal if possible
        if (horizontalPositions.Count > 0)
        {
            return horizontalPositions[Random.Range(0, horizontalPositions.Count)];
        }
        
        // Fallback to any valid position
        if (validPositions.Count > 0)
        {
            return validPositions[Random.Range(0, validPositions.Count)];
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
        // Find the highest normal room
        int highestY = 0;
        int bestX = startRoomPos.x;
        bool foundRoom = false;

        // Find the highest normal room
        for (int y = roomGrid.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < roomGrid.GetLength(0); x++)
            {
                if (roomGrid[x, y] && roomTypes[x, y] == RoomType.Normal)
                {
                    highestY = y;
                    bestX = x;
                    foundRoom = true;
                    y = -1;
                    break;
                }
            }
        }

        if (!foundRoom) return;

        // Try to place boss room at increasing heights until successful
        for (int heightOffset = 3; heightOffset <= 5; heightOffset++)
        {
            // Try multiple positions at each height
            Vector2Int[] possiblePositions = new Vector2Int[]
            {
                new Vector2Int(bestX, highestY + heightOffset),      // Directly above
                new Vector2Int(bestX - 1, highestY + heightOffset),  // Above and left
                new Vector2Int(bestX + 1, highestY + heightOffset),  // Above and right
                new Vector2Int(bestX - 2, highestY + heightOffset),  // Further left
                new Vector2Int(bestX + 2, highestY + heightOffset)   // Further right
            };

            foreach (Vector2Int bossPos in possiblePositions)
            {
                if (IsValidBossPosition(bossPos))
                {
                    bossRoomPos = bossPos;
                    roomGrid[bossPos.x, bossPos.y] = true;
                    roomTypes[bossPos.x, bossPos.y] = RoomType.Boss;
                    
                    Vector2Int worldPos = GetPositionFromGridIndex(bossPos);
                    float xOffset = (bossRoomWidth - roomWidth) / 2f;
                    float yOffset = (bossRoomHeight - roomHeight) / 2f;
                    Vector3 adjustedPos = new Vector3(worldPos.x + xOffset, worldPos.y + yOffset, 0);
                    
                    GameObject bossRoom = Instantiate(bossRoomPrefab, adjustedPos, Quaternion.identity);
                    roomObjects.Add(bossRoom);
                    return;
                }
            }
        }

        // If no position worked, force place at highest possible position
        Vector2Int forcedPos = new Vector2Int(bestX, roomGrid.GetLength(1) - 3);
        if (IsInGrid(forcedPos))
        {
            bossRoomPos = forcedPos;
            roomGrid[forcedPos.x, forcedPos.y] = true;
            roomTypes[forcedPos.x, forcedPos.y] = RoomType.Boss;
            
            Vector2Int worldPos = GetPositionFromGridIndex(forcedPos);
            float xOffset = (bossRoomWidth - roomWidth) / 2f;
            float yOffset = (bossRoomHeight - roomHeight) / 2f;
            Vector3 adjustedPos = new Vector3(worldPos.x + xOffset, worldPos.y + yOffset, 0);
            
            GameObject bossRoom = Instantiate(bossRoomPrefab, adjustedPos, Quaternion.identity);
            roomObjects.Add(bossRoom);
        }
    }

    private bool IsValidBossPosition(Vector2Int pos)
    {
        if (!IsInGrid(pos)) return false;

        // Check a larger area around and below the boss room
        for (int x = pos.x - 2; x <= pos.x + 2; x++)
        {
            for (int y = pos.y - 2; y <= pos.y; y++) // Only check up to current position
            {
                if (IsInGrid(new Vector2Int(x, y)))
                {
                    // Don't allow any rooms in the checking area
                    if (roomGrid[x, y])
                    {
                        return false;
                    }
                }
            }
        }

        // Ensure we're not too close to horizontal edges
        if (pos.x < 2 || pos.x >= roomGrid.GetLength(0) - 2)
        {
            return false;
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
            roomWidth * (gridX - 5),
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
}
