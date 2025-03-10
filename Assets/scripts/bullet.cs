using UnityEngine;

public class bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private bool isEnemyBullet;
    public float defDamage;
    public float damage; // Added damage property
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


    public bool permaAlive;

    void Start()
    {
        // Initialize damage from default damage value if not already set
        if (damage <= 0 && defDamage > 0) {
            damage = defDamage;
        }
    }
    
    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
        timeAlive = 0f;
        
        // Initialize damage from default damage value
        if (defDamage > 0) {
            damage = defDamage;
        }
    }

    void Update()
    {
        // Move the bullet using Transform in local space
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Update lifetime and destroy if exceeded
        timeAlive += Time.deltaTime;
        if (!permaAlive && timeAlive >= lifetime)
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
