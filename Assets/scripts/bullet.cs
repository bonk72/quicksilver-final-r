using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int damage;
    public float lifetime = 3f; // How long the bullet exists before self-destructing
    private float startTime;
    private int originalDamage;
    public bool falloffDamage;


    void Start()
    {
        startTime = Time.time;
        originalDamage = damage;
        // Destroy the bullet after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (falloffDamage){
            // Calculate how long the bullet has existed
            float elapsedTime = Time.time - startTime;
            // Calculate damage reduction based on time (linear falloff)
            float damageMultiplier = 1f - (elapsedTime / lifetime);
            // Update current damage
            damage = Mathf.RoundToInt(originalDamage * damageMultiplier);

        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Destroy(gameObject);
    }

}
