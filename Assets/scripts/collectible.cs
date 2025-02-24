using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class collectible : MonoBehaviour
{
    public bool isHeal;
    public bool isGold;
    public bool tracksPlayer;
    public int timeIncrease;
    public int goldGain;
    
    public float trackingDelay = 1f; // Time to wait before tracking
    public float trackingSpeed = 5f; // Speed at which to track the player

    private bool isTracking = false;
    private Transform playerTransform;

    void Start()
    {
        if (tracksPlayer)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            StartCoroutine(StartTracking());
        }
    }

    void Update()
    {
        if (isTracking && playerTransform != null)
        {
            // Move towards the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * trackingSpeed * Time.deltaTime;


        }
    }

    private IEnumerator StartTracking()
    {
        yield return new WaitForSeconds(trackingDelay);
        isTracking = true;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
            if(isHeal){
                collision.gameObject.GetComponent<PlayerTime>().AddTime(timeIncrease);
                Destroy(gameObject);
            }
            else if (isGold){
                collision.gameObject.GetComponent<playerGold>().gainGold(goldGain);
                Destroy(gameObject);
            }
        }
    }
}
