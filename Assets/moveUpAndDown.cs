using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moveUpAndDown : MonoBehaviour
{
    private RectTransform rectTransform;
    
    [Header("Floating Settings")]
    [SerializeField] private float maxDistance = 15f;       // Maximum distance from initial position
    [SerializeField] private float moveSpeed = 10f;         // Base movement speed
    [SerializeField] private float noiseSpeed = 0.5f;       // How quickly the noise changes
    [SerializeField] private float rotationAmount = 0f;     // Max rotation in degrees (set to 0 for no rotation)
    
    private Vector2 initialPosition;
    private float timeOffset;
    private float rotationTimeOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        
        // Random starting position in the noise
        timeOffset = Random.Range(0f, 100f);
        rotationTimeOffset = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        // Get perlin noise values for X and Y
        float xNoise = Mathf.PerlinNoise(timeOffset + Time.time * noiseSpeed, 0);
        float yNoise = Mathf.PerlinNoise(0, timeOffset + Time.time * noiseSpeed);
        
        // Convert from 0-1 range to -1 to 1 range
        float xOffset = (xNoise * 2 - 1) * maxDistance;
        float yOffset = (yNoise * 2 - 1) * maxDistance;
        
        // Calculate target position
        Vector2 targetPosition = new Vector2(
            initialPosition.x + xOffset,
            initialPosition.y + yOffset
        );
        
        // Smoothly move toward the target position
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
        
        // Add slight rotation for a more natural floating effect
        if (rotationAmount > 0)
        {
            float rotationNoise = Mathf.PerlinNoise(rotationTimeOffset + Time.time * noiseSpeed * 0.5f, 0);
            float angle = (rotationNoise * 2 - 1) * rotationAmount;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
