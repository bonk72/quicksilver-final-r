using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb2d;
    private Vector2 moveInput;
    public CircleCollider2D coll;
    public CircleCollider2D executeColl;

    public weapon weapon;
    private Vector2 mousePosition;

    private float activeMoveSpeed;
    public float dashSpeed;

    public float dashLength, dashCooldown = 1f;
    private float temp;

    private bool isDashing;
    private bool canDash = true;
    private bool dashR = false;

    // Bounce back variables
    public float bounceForce = 10f;
    public float bounceLength = 0.2f;
    private bool isBouncing = false;


    public PlayerTime time;
    void Start()
    {
        executeColl.enabled = false;
        activeMoveSpeed = moveSpeed;
        temp = dashCooldown;
    }

    void Update()
    {
        if (isDashing || isBouncing) {
            return;
        }

        float movex = Input.GetAxisRaw("Horizontal");
        float movey = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(movex, movey).normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0)) {
            weapon.Fire();
        }
        if (Input.GetMouseButtonDown(1) && canDash) {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing || isBouncing)
        {
            return;
        }
        rb2d.velocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);

        Vector2 aimDirection = mousePosition - rb2d.position;
        float aimangle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb2d.rotation = aimangle;
    }

    private IEnumerator Dash() {
        coll.enabled = false;
        executeColl.enabled = true;
        canDash = false;
        isDashing = true;
        dashR = false;
        dashCooldown = temp;

        // Calculate dash direction towards mouse
        Vector2 dashDirection = (mousePosition - rb2d.position).normalized;
        rb2d.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashLength);
        
        isDashing = false;
        coll.enabled = true;
        executeColl.enabled = false;
        if (dashR){
            dashCooldown = 0;
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator BounceBack(Vector2 enemyPosition)
    {
        isBouncing = true;
        
        // Calculate bounce direction away from enemy
        Vector2 bounceDirection = (rb2d.position - enemyPosition).normalized;
        rb2d.velocity = bounceDirection * bounceForce;

        yield return new WaitForSeconds(bounceLength);
        
        isBouncing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy") && !isDashing && !isBouncing)
        {

            StartCoroutine(BounceBack(collision.transform.position));
        }
    }

    public void ResetDash(){
        dashR = true;
    }
}
