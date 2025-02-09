using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int maxRooms =10;
    [SerializeField] private int minRooms =8 ;


    int roomWidth = 34;
    int roomHeight = 12;

    int gridSizeX = 10;
    int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private int[,] roomGrid;

    private int roomCount;

    private void Start(){

        roomGrid = new int[gridSizeX, gridSizeY];

    }
    private Vector2Int GetPositionFromGridIndex(Vector2Int gridIndex){
        int gridX= gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector2Int(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY/2)) ;

    }
    private void OnDrawGizmos(){

        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++){

            for (int y = 0; y < gridSizeY; y++){

                Vector2Int position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(new Vector3(position.x, position.y), new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}
