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

    public void UpdateHealthBar(float currentvalue, float maxvalue, float threshold)
    {
        float healthRatio = currentvalue / maxvalue;
        Slider.value = healthRatio;
        
        // Change color based on health ratio
        Image fillImage = Slider.fillRect.GetComponent<Image>();
        fillImage.color = healthRatio <= threshold ? Color.red : Color.green;
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
