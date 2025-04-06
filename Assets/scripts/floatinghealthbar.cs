using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatinghealthbar : MonoBehaviour
{
    public Slider Slider;
    public Canvas cameraa;
    public Transform target;
    public Vector3 offset;

    public void UpdateHealthBar(float currentvalue, float maxvalue, float threshold, bool invulnerable = false)
    {
        float healthRatio = currentvalue / maxvalue;
        Slider.value = healthRatio;
        
        // Get the fill image component
        Image fillImage = Slider.fillRect.GetComponent<Image>();
        
        // If invulnerable, set to silver/metal color
        if (invulnerable)
        {
            // Silver/metal color (RGB: 192, 192, 192)
            fillImage.color = new Color(0.75f, 0.75f, 0.75f);
        }
        else
        {
            // Normal color logic based on health ratio
            // Red if below threshold, green otherwise
            fillImage.color = healthRatio <= threshold ? Color.red : Color.green;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = cameraa.transform.rotation;
        transform.position = target.position + offset;
    }
}
