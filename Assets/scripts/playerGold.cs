using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class playerGold : MonoBehaviour
{
    [Header("Gold Settings")]
    public static int Gold;
    public TMP_Text goldAmnt;
    public int goldMult = 1;
    private bool canPurchase;
    
    [Header("UI Pulse Effect")]
    public Image goldImage; // UI Image that will pulse when gold is gained
    public float pulseDuration = 0.3f; // Duration of the pulse effect
    public float minPulseScale = 1.1f; // Minimum scale during pulse
    public float maxPulseScale = 1.5f; // Maximum scale during pulse
    public float pulseScaleFactor = 0.01f; // How much to increase scale per unit of gold gained
    private Vector3 originalScale; // Original scale of the UI Image
    private bool isPulsing = false; // Whether the image is currently pulsing
    private float currentPulseMagnitude = 0f; // Current magnitude of the pulse effect
    private Coroutine currentPulseCoroutine = null; // Reference to the current pulse coroutine
    // Start is called before the first frame update
    void Start()
    {
        goldAmnt.text = Gold.ToString();
        
        // Initialize UI image effect
        if (goldImage != null)
        {
            // Store original scale
            originalScale = goldImage.rectTransform.localScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void gainGold(int amnt){
        int actualGain = amnt * goldMult;
        Gold += actualGain;
        goldAmnt.text = Gold.ToString();
        
        // Trigger pulse effect proportional to gold gained
        TriggerPulse(actualGain);
    }
    
    // Method to start or update the pulse effect based on gold gained
    public void TriggerPulse(float goldAmount)
    {
        if (goldImage == null) return;
        
        // If not currently pulsing or the new gold amount is greater than or equal to the current pulse
        if (!isPulsing || goldAmount >= currentPulseMagnitude)
        {
            // If already pulsing, stop the current pulse
            if (isPulsing && currentPulseCoroutine != null)
            {
                StopCoroutine(currentPulseCoroutine);
            }
            
            // Start a new pulse with the new magnitude
            currentPulseMagnitude = goldAmount;
            currentPulseCoroutine = StartCoroutine(PulseImage(goldAmount));
        }
    }
    
    // Method to pulse the UI Image when gold is gained
    private IEnumerator PulseImage(float goldAmount)
    {
        if (goldImage != null)
        {
            isPulsing = true;
            
            // Calculate pulse scale based on gold gained, with min and max limits
            float calculatedScale = 1f + (goldAmount * pulseScaleFactor);
            float actualPulseScale = Mathf.Clamp(calculatedScale, minPulseScale, maxPulseScale);
            
            // Store the current scale
            Vector3 startScale = goldImage.rectTransform.localScale;
            
            // Pulse out
            float timer = 0f;
            while (timer < pulseDuration / 2)
            {
                timer += Time.deltaTime;
                float progress = timer / (pulseDuration / 2);
                goldImage.rectTransform.localScale = Vector3.Lerp(startScale, originalScale * actualPulseScale, progress);
                yield return null;
            }
            
            // Pulse in
            timer = 0f;
            while (timer < pulseDuration / 2)
            {
                timer += Time.deltaTime;
                float progress = timer / (pulseDuration / 2);
                goldImage.rectTransform.localScale = Vector3.Lerp(originalScale * actualPulseScale, originalScale, progress);
                yield return null;
            }
            
            // Ensure we end at the original scale
            goldImage.rectTransform.localScale = originalScale;
            
            // Reset pulse state
            isPulsing = false;
            currentPulseMagnitude = 0f;
            currentPulseCoroutine = null;
        }
    }
    public void loseGold(int amnt){
        if (Gold - amnt >= 0){
            Gold-= amnt;
            goldAmnt.text = Gold.ToString();
        }

    }
    public void incrGoldMult(int amnt){
        goldMult += amnt;
    }
}
