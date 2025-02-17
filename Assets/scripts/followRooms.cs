using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followRooms : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private bool isHandlingRoomChange = false;
    private const float MOVE_DISTANCE = 28f;

    void Update()
    {
        if (player.GetComponent<roomSwitch>().newRoom && !isHandlingRoomChange)
        {
            StartCoroutine(HandleRoomChange());
        }
    }

    private IEnumerator HandleRoomChange()
    {
        isHandlingRoomChange = true;
        
        // Start the movement
        yield return StartCoroutine(MoveUpward());
        
        // Only reset newRoom after movement is complete
        player.GetComponent<roomSwitch>().newRoom = false;
        isHandlingRoomChange = false;
    }

    private IEnumerator MoveUpward()
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (Vector3.up * MOVE_DISTANCE);
        float elapsedTime = 0f;
        float duration = MOVE_DISTANCE / moveSpeed;

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
