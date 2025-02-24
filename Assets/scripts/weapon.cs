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
    void Start()
    {
        bulletSpeed = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireSpeed;
        fireRate = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireRate;
    }
    public void Fire() {
        if (Time.time >= nextFireTime) {

            Vector2 direction = firePoint.up;
            

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, angle - 90);
            

            GameObject bulletObj = Instantiate(bulletPrefabs[currentWeapon], firePoint.position, bulletRotation);
            bullet bulletScript = bulletObj.GetComponent<bullet>();
            if (bulletScript != null) {
                bulletScript.Initialize(direction, bulletSpeed);
            }
            nextFireTime = Time.time + fireRate;
        }
    }
    public void ChangeWeapon(int index){
        currentWeapon = index;
        bulletSpeed = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireSpeed;
        fireRate = bulletPrefabs[currentWeapon].GetComponent<bullet>().fireRate;

    }
    //public void ResetFire(){
    //    nextFireTime = 0f;
//
    //}
}
