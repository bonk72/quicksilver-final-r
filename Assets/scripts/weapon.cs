using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public GameObject[] bulletPrefabs;
    public Transform firePoint;
    private float bulletSpeed;

    private float nextFireTime = 0f;

    private float fireRate;
    public static int currentWeapon = 0;
    private int projCount;
    private int angle;
    private bool isBurst;



    private float atkMult = 1;
    
    // Boolean to disable shooting (e.g., when in shop)
    public bool canShoot = true;
    
    void Start()
    {
        bulletSpeed = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireSpeed;
        fireRate = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireRate;
        isBurst = bulletPrefabs[currentWeapon].GetComponent<bullet>().shotgun;
        if(isBurst){
            angle = bulletPrefabs[currentWeapon].GetComponent<bullet>().spread;
            projCount = bulletPrefabs[currentWeapon].GetComponent<bullet>().projcount;
        }
    }

    public void Fire() {
        // Only fire if canShoot is true
        if (!canShoot) return;
        
        else if (Time.time >= nextFireTime) {
            Vector2 baseDirection = firePoint.up;
            
            if (isBurst) {
                // Burst fire mode - shoot multiple projectiles in a spread pattern
                float startAngle = -angle / 2f; // Start from the leftmost angle
                float angleStep = angle / (float)(projCount - 1); // Angle between each projectile
                
                // If there's only one projectile, don't divide by zero
                if (projCount <= 1) {
                    angleStep = 0;
                    startAngle = 0;
                }
                
                // Spawn each projectile in the burst
                for (int i = 0; i < projCount; i++) {
                    // Calculate the angle for this projectile
                    float currentAngle = startAngle + (angleStep * i);
                    
                    // Rotate the base direction by the calculated angle
                    Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                    Vector2 direction = rotation * baseDirection;
                    
                    // Calculate bullet rotation based on its direction
                    float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion bulletRotation = Quaternion.Euler(0, 0, bulletAngle - 90);
                    
                    // Instantiate and initialize the bullet
                    GameObject bulletObj = Instantiate(bulletPrefabs[currentWeapon], firePoint.position, bulletRotation);
                    bullet bulletScript = bulletObj.GetComponent<bullet>();
                    if (bulletScript != null) {
                        bulletScript.Initialize(direction, bulletSpeed);
                    }
                }
            } else {
                // Standard single shot mode
                float bulletAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
                Quaternion bulletRotation = Quaternion.Euler(0, 0, bulletAngle - 90);
                
                GameObject bulletObj = Instantiate(bulletPrefabs[currentWeapon], firePoint.position, bulletRotation);
                bullet bulletScript = bulletObj.GetComponent<bullet>();
                if (bulletScript != null) {
                    bulletScript.Initialize(baseDirection, bulletSpeed);
                }
            }
            
            // Set the cooldown for the next shot
            nextFireTime = Time.time + fireRate;
        }
    }
    public void ChangeWeapon(int index){
        currentWeapon = index;
        bulletSpeed = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireSpeed;
        fireRate = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireRate;
        isBurst = bulletPrefabs[currentWeapon].GetComponent<bullet>().shotgun;
        if(isBurst){
            angle = bulletPrefabs[currentWeapon].GetComponent<bullet>().spread;
            projCount = bulletPrefabs[currentWeapon].GetComponent<bullet>().projcount;
        }

    }
    public void incrAtkMult(float amount){
        atkMult += amount;
        for (int i = 0; i < bulletPrefabs.Length; i++){
            bulletPrefabs[i].GetComponent<bullet>().damage *= atkMult;
        }
    }
    //public void ResetFire(){
    //    nextFireTime = 0f;
//
    //}
}
