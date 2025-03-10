using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Cinemachine;
using UnityEngine;

public class followRooms : MonoBehaviour
{
    public GameObject player;
    public CinemachineVirtualCamera cam;
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private bool isHandlingRoomChange = false;
    private const float MOVE_DISTANCE = 28f;
    private const float BIG_DISTANCE = 114.5f;

    void Update()
    {
        if (player.GetComponent<roomSwitch>().newRoom && !isHandlingRoomChange)
        {
            StartCoroutine(HandleRoomChange(MOVE_DISTANCE));
        }
        if (player.GetComponent<roomSwitch>().finalRoom && !isHandlingRoomChange)
        {
            cam.m_Lens.OrthographicSize = 16;

            StartCoroutine(HandleRoomChange(BIG_DISTANCE));
        }
        if (player.GetComponent<roomSwitch>().reset && !isHandlingRoomChange){
            cam.m_Lens.OrthographicSize = 14;
            StartCoroutine(Reset());
        }
    }

    private IEnumerator HandleRoomChange(float dist)
    {
        isHandlingRoomChange = true;
        
        // Always move upward regardless of player movement direction
        yield return StartCoroutine(MoveUpward(dist));
        
        // Only reset newRoom after movement is complete
        player.GetComponent<roomSwitch>().newRoom = false;
        player.GetComponent<roomSwitch>().finalRoom = false;
        isHandlingRoomChange = false;
    }
    
    private IEnumerator Reset(){
        isHandlingRoomChange = true;
        
        // Immediately set position to (0, 0) while keeping the same z value
        transform.position = new Vector3(0, 0, 0);
        
        // Wait for a frame to ensure everything updates properly
        yield return null;
        
        // Reset the flag in roomSwitch
        player.GetComponent<roomSwitch>().reset = false;
        isHandlingRoomChange = false;
    }

    private IEnumerator MoveDownward(float dist)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (Vector3.down * dist);
        float elapsedTime = 0f;
        float duration = dist / (moveSpeed * (dist / 28));

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly the target position
        transform.position = targetPosition;
        isMoving = false;
    }
    
    private IEnumerator MoveUpward(float dist)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (Vector3.up * dist);
        float elapsedTime = 0f;
        float duration = dist / (moveSpeed * (dist / 28));

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly 28 units up
        transform.position = targetPosition;
        isMoving = false;
    }
}
