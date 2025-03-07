using UnityEngine;

public class bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private bool isEnemyBullet;
    public float damage = 1f; // Added damage property
    public bool isImpermeable;
    public float lifetime = 5f; // How long the bullet lives before being destroyed
    private float timeAlive = 0f;

    public int price;
    public float fireSpeed;
    public float fireRate;
    public int index;
    public int spread;
    public int projcount;
    public bool shotgun;

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
        timeAlive = 0f;
    }

    void Update()
    {
        // Move the bullet using Transform in local space
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Update lifetime and destroy if exceeded
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isImpermeable && !collision.CompareTag("bullet")){
            Destroy(gameObject);
        }
    }
   
}
