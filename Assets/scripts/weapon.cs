using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f; // Time between shots in seconds
    private float nextFireTime = 0f;

    public void Fire() {
        if (Time.time >= nextFireTime) {

            Vector2 direction = firePoint.up;
            

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle - 90);
            

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            bullet bulletScript = bulletObj.GetComponent<bullet>();
            if (bulletScript != null) {
                bulletScript.Initialize(direction, bulletSpeed);
            }
            nextFireTime = Time.time + fireRate;
        }
    }
    //public void ResetFire(){
    //    nextFireTime = 0f;
//
    //}
}
