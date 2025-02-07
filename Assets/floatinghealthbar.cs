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
    public void UpdateHealthBar(float currentvalue, float maxvalue){
        Slider.value = currentvalue / maxvalue;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cameraa.transform.rotation;
        transform.position = target.position + offset;

    }
}
