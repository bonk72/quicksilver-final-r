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

    public weapon weapon;
    private Vector2 mousePosition;

    private float activeMoveSpeed;
    public float dashSpeed;

    public float dashLength, dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;

    void Start()
    {
        activeMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (isDashing) {
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
        if (isDashing)
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
        canDash = false;
        isDashing = true;

        // Calculate dash direction towards mouse
        Vector2 dashDirection = (mousePosition - rb2d.position).normalized;
        rb2d.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashLength);
        
        isDashing = false;
        coll.enabled = true;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void ResetDash(){
        canDash = true;
    }
}
