using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class PlayerTime : MonoBehaviour
{
    [Header("Time Settings")]
    public static float maxTime = 150f; // Starting time in seconds
    public float currentTime;
    public float timeDecreaseRate = 1f; // How much time decreases per second

    [Header("Player State")]
    private bool isDead = false;
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 1f; // Duration of invulnerability after taking damage

    [Header("UI References")]
    public TMP_Text text;
    public TMP_Text revText;
    public GameObject reviveBar;
    public bool timeTick;

    [Header("Revival System")]
    public static int revCount;
    private int currentRev;
    public GameObject reviveParticleEffect; // Particle effect to play when revived

    [Header("Low Time Effect Settings")]
    public float lowTimeThreshold = 30f; // Time threshold for activating effects
    public float lowTimeExitBuffer = 15f; // Additional time needed to exit the low time state
    public float damageMultiplierBoost = 1.5f; // How much to increase damage multiplier
    public float speedBoost = 3f; // How much to increase movement speed
    public float dashDistanceBoost = 1.5f; // How much to increase dash distance
    [Header("Screen Effect")]
    public GameObject redOverlayCanvas; // Assign a Canvas with red overlay effect
    
    [Header("UI Image Effect Settings")]
    public UnityEngine.UI.Image targetImage; // UI Image that will change color and shake
    public float colorTransitionSpeed = 3f; // Speed of color transition
    public float shakeIntensity = 0.1f; // Intensity of the shake effect
    public float shakeSpeed = 10f; // Speed of the shake effect
    public float pulseDuration = 0.3f; // Duration of the pulse effect
    public float minPulseScale = 1.1f; // Minimum scale during pulse
    public float maxPulseScale = 1.5f; // Maximum scale during pulse
    public float pulseScaleFactor = 0.05f; // How much to increase scale per unit of time lost
    private Color originalColor; // Original color of the UI Image
    private Vector3 originalPosition; // Original position of the UI Image
    private Vector3 originalScale; // Original scale of the UI Image
    private bool isTransitioning = false; // Whether the image is transitioning color
    private float transitionProgress = 0f; // Progress of the color transition (0-1)
    private bool isPulsing = false; // Whether the image is currently pulsing
    private float currentPulseMagnitude = 0f; // Current magnitude of the pulse effect
    private Coroutine currentPulseCoroutine = null; // Reference to the current pulse coroutine
    
    [Header("Component References")]
    private Rigidbody2D rb;
    private CircleCollider2D coll;
    private weapon weaponComponent;
    private Movement movementComponent;
    private bool isLowTime = false;

    private IEnumerator TemporaryInvulnerability()
    {
        isInvulnerable = true;

        
        yield return new WaitForSeconds(invulnerabilityDuration);
        
        isInvulnerable = false;

    }


    void Start()
    {
        // Initialize components
        coll = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        weaponComponent = GetComponent<weapon>();
        movementComponent = GetComponent<Movement>();
        
        // Initialize revival system
        currentRev = revCount;
        if (revCount > 0){
            reviveBar.SetActive(true);
        }
        revText.text = currentRev.ToString();
        
        // Initialize time
        currentTime = maxTime;
        text.text = currentTime.ToString();
        StartCoroutine(TimeDecrease());
        timeTick = false;
        
        // Initialize red overlay canvas
        if (redOverlayCanvas != null)
        {
            // Set initial state (disabled)
            redOverlayCanvas.SetActive(false);
        }
        
        // Initialize UI image effect
        if (targetImage != null)
        {
            // Store original color, position, and scale
            originalColor = targetImage.color;
            originalPosition = targetImage.rectTransform.position;
            originalScale = targetImage.rectTransform.localScale;
        }
    }

    void Update()
    {
        // Check if player is dead
        if (currentTime <= 0 && !isDead)
        {
            Die();
            return;
        }
        
        // Check if player is in low time state
        if (currentTime <= lowTimeThreshold && !isLowTime && !isDead)
        {
            // Enter low time state
            EnterLowTimeState();
        }
        else if (currentTime > (lowTimeThreshold + lowTimeExitBuffer) && isLowTime)
        {
            // Exit low time state only when exceeding threshold + buffer
            ExitLowTimeState();
        }
        
        // Update UI image effects if in low time state
        if (isLowTime && targetImage != null)
        {
            // Update color transition
            if (isTransitioning)
            {
                transitionProgress += Time.deltaTime * colorTransitionSpeed;
                if (transitionProgress >= 1f)
                {
                    transitionProgress = 1f;
                    isTransitioning = false;
                }
                
                // Interpolate between original color and target color (C81212 - deep red)
                Color targetColor = new Color(0.78f, 0.07f, 0.07f); // C81212 in RGB (200, 18, 18) normalized
                targetImage.color = Color.Lerp(originalColor, targetColor, transitionProgress);
            }
            
            // Apply shake effect
            if (targetImage != null)
            {
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * shakeIntensity;
                randomOffset.z = 0; // Keep z position unchanged
                targetImage.rectTransform.position = originalPosition + randomOffset;
            }
        }
    }
    
    // Apply low time effects
    private void EnterLowTimeState()
    {
        isLowTime = true;
        
        // Increase damage multiplier
        if (weaponComponent != null)
        {
            weaponComponent.incrAtkMult(damageMultiplierBoost);
        }
        
        // Increase movement speed
        if (movementComponent != null)
        {
            movementComponent.increaseCurrMoveSpeed(speedBoost);
        }
        
        // Increase dash distance
        if (movementComponent != null)
        {
            // Store original dash speed
            float originalDashSpeed = movementComponent.dashSpeed;
            // Increase dash speed by the boost factor
            movementComponent.dashSpeed *= dashDistanceBoost;
        }
        
        // Activate red overlay canvas
        if (redOverlayCanvas != null)
        {
            redOverlayCanvas.SetActive(true);
        }
        
        // Start UI image color transition and shake effect
        if (targetImage != null)
        {
            isTransitioning = true;
            transitionProgress = 0f;
        }
    }
    
    // Remove low time effects
    private void ExitLowTimeState()
    {
        isLowTime = false;
        
        // Reset damage multiplier
        if (weaponComponent != null)
        {
            weaponComponent.ResetAtkMult();
        }
        
        // Reset movement speed
        if (movementComponent != null)
        {
            // Since we can't access currentMoveSpeed directly, we'll reset it by
            // first storing the original speed boost value as a negative value
            movementComponent.increaseCurrMoveSpeed(-speedBoost);
        }
        
        // Reset dash distance
        if (movementComponent != null)
        {
            // Reset dash speed to original value
            movementComponent.dashSpeed /= dashDistanceBoost;
        }
        
        // Deactivate red overlay canvas
        if (redOverlayCanvas != null)
        {
            redOverlayCanvas.SetActive(false);
        }
        
        // Reset UI image color and position
        if (targetImage != null)
        {
            // Reset color
            targetImage.color = originalColor;
            
            // Reset position
            targetImage.rectTransform.position = originalPosition;
            
            // Reset transition state
            isTransitioning = false;
            transitionProgress = 0f;
        }
    }
    
    // Method to start or update the pulse effect based on time lost
    public void TriggerPulse(float timeLost)
    {
        if (targetImage == null) return;
        
        // If not currently pulsing or the new damage is greater than the current pulse
        if (!isPulsing || timeLost > currentPulseMagnitude)
        {
            // If already pulsing, stop the current pulse
            if (isPulsing && currentPulseCoroutine != null)
            {
                StopCoroutine(currentPulseCoroutine);
            }
            
            // Start a new pulse with the new magnitude
            currentPulseMagnitude = timeLost;
            currentPulseCoroutine = StartCoroutine(PulseImage(timeLost));
        }
    }
    
    // Method to pulse the UI Image when time decreases
    private IEnumerator PulseImage(float timeLost)
    {
        if (targetImage != null)
        {
            isPulsing = true;
            
            // Calculate pulse scale based on time lost, with min and max limits
            float calculatedScale = 1f + (timeLost * pulseScaleFactor);
            float actualPulseScale = Mathf.Clamp(calculatedScale, minPulseScale, maxPulseScale);
            
            // Store the current scale
            Vector3 startScale = targetImage.rectTransform.localScale;
            
            // Pulse out
            float timer = 0f;
            while (timer < pulseDuration / 2)
            {
                timer += Time.deltaTime;
                float progress = timer / (pulseDuration / 2);
                targetImage.rectTransform.localScale = Vector3.Lerp(startScale, originalScale * actualPulseScale, progress);
                yield return null;
            }
            
            // Pulse in
            timer = 0f;
            while (timer < pulseDuration / 2)
            {
                timer += Time.deltaTime;
                float progress = timer / (pulseDuration / 2);
                targetImage.rectTransform.localScale = Vector3.Lerp(originalScale * actualPulseScale, originalScale, progress);
                yield return null;
            }
            
            // Ensure we end at the original scale
            targetImage.rectTransform.localScale = originalScale;
            
            // Reset pulse state
            isPulsing = false;
            currentPulseMagnitude = 0f;
            currentPulseCoroutine = null;
        }
    }

    private IEnumerator TimeDecrease()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            if (!isDead && timeTick)
            {
                // Store previous time to check if we cross the threshold
                float previousTime = currentTime;
                
                // Decrease time
                currentTime -= timeDecreaseRate;
                text.text = currentTime.ToString();
                currentTime = Mathf.Max(currentTime, 0); // Prevent negative time
                
                // Trigger pulse effect when time decreases naturally
                TriggerPulse(timeDecreaseRate);
                
                // Check if we crossed the low time threshold (entering)
                // Only enter if we're not already in the low time state
                if (previousTime > lowTimeThreshold && currentTime <= lowTimeThreshold && !isLowTime)
                {
                    EnterLowTimeState();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && !isInvulnerable)
        {
            if (collision.gameObject.CompareTag("enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Apply time damage
                    TakeDamage(enemy.timeDamage, true);

                    // Stun the enemy
                    enemy.Stun();
                }
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemyProjectile"))
            {
                bullet bul = collision.gameObject.GetComponent<bullet>();
                if (bul != null)
                {
                    TakeDamage(bul.damage, false);
                }
            }
    }

    public void TakeDamage(float damage, bool contact)
    {
        if (!isInvulnerable && !isDead)
        {
            // Store previous time to check if we cross the threshold
            float previousTime = currentTime;
            
            // Apply damage
            currentTime -= damage;
            text.text = currentTime.ToString();
            currentTime = Mathf.Max(currentTime, 0); // Prevent negative time
            
            // Trigger pulse effect proportional to damage taken
            TriggerPulse(damage);
            
            // Check if we died
            if (currentTime <= 0)
            {
                Die();
            }
            else
            {
                // Check if we crossed the low time threshold (entering)
                // Only enter if we're not already in the low time state
                if (previousTime > lowTimeThreshold && currentTime <= lowTimeThreshold && !isLowTime)
                {
                    EnterLowTimeState();
                }
                
                // Apply invulnerability if from contact
                if (contact)
                {
                    StartCoroutine(TemporaryInvulnerability());
                }
            }
        }
    }

    private void Die()
    {
        // If we're in low time state, exit it
        if (isLowTime)
        {
            ExitLowTimeState();
        }
        
        if(currentRev > 0){
            currentRev -=1;
            currentTime = maxTime / 2;
            revText.text = currentRev.ToString();
            
            // Play revive particle effect if assigned
            if (reviveParticleEffect != null) {
                Instantiate(reviveParticleEffect, transform.position, Quaternion.identity);
            }
        }
        else{
            isDead = true;
            // Stop player movement
            Movement movement = GetComponent<Movement>();
            if (movement != null)
            {
                movement.enabled = false;
            }
            // Stop weapon
            weapon weaponComponent = GetComponent<weapon>();
            if (weaponComponent != null)
            {
                weaponComponent.enabled = false;
            }
            // Disable collider
            //Collider2D collider = GetComponent<Collider2D>();
            //if (collider != null)
            //{
            //    collider.enabled = false;
            //}
            // Optional: Add death animation or particle effect here

            // Optional: Add game over screen or restart mechanism here
            SceneManager.LoadScene("Starting Area");
        }
        //Debug.Log("Player has died!");
    }

    // Optional: Method to add time (could be used for pickups or rewards)
    public void AddTime(float amount)
    {
        if (!isDead)
        {
            // Store previous time to check if we cross the threshold
            float previousTime = currentTime;
            
            // Add time
            currentTime += amount;
            text.text = currentTime.ToString();
            
            // Check if we crossed the exit threshold (threshold + buffer)
            if (previousTime <= (lowTimeThreshold + lowTimeExitBuffer) &&
                currentTime > (lowTimeThreshold + lowTimeExitBuffer) &&
                isLowTime)
            {
                ExitLowTimeState();
            }
        }
    }
    public void AddMaxTime(int amount){
        // Store previous time to check if we cross the threshold
        float previousTime = currentTime;
        
        // Add max time
        maxTime += amount;
        currentTime = maxTime;
        text.text = currentTime.ToString();
        
        // Check if we crossed the exit threshold (threshold + buffer)
        if (previousTime <= (lowTimeThreshold + lowTimeExitBuffer) &&
            currentTime > (lowTimeThreshold + lowTimeExitBuffer) &&
            isLowTime)
        {
            ExitLowTimeState();
        }
    }
    public void addRev(){
        revCount += 1;
        currentRev = revCount;
        revText.text = currentRev.ToString();
        reviveBar.SetActive(true);
    }
    public void addCurrentRev(int amount){
        currentRev += amount;
        revText.text = currentRev.ToString();
        reviveBar.SetActive(true);
    }
}
