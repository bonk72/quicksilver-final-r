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

    // Start is called before the first frame update
    void Start()
    {


        activeMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) {
            return;
        }

        float movex = Input.GetAxisRaw("Horizontal");
        float movey = Input.GetAxisRaw("Vertical");


        moveInput = new Vector2(movex, movey).normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            weapon.Fire();
        }
        if (Input.GetKeyDown(KeyCode.Space) && canDash) {
            StartCoroutine(Dash());
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (dashCooldownTime <= 0 && dashCounter <= 0)
        //    {
        //        activeMoveSpeed = dashSpeed;
        //        dashCounter = dashLength;


            //    }
            //}
            //if (dashCounter > 0)   
            //{
            //    dashCounter -= Time.deltaTime;

            //    if (dashCounter <= dashLength / 2)
            //    {
            //        coll.enabled = true;
            //    }
            //    if (dashCounter <= 0)
            //    {
            //        activeMoveSpeed = moveSpeed;
            //        dashCooldownTime = dashCooldown;
            //    }
            //}
            //if (dashCooldownTime > 0)
            //{
            //    coll.enabled = true;
            //   dashCooldownTime -= Time.deltaTime;
            //}
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
        canDash = false;
        isDashing = true;
        rb2d.velocity = new Vector2(moveInput.x * dashSpeed, moveInput.y * dashSpeed);
        yield return new WaitForSeconds(dashLength);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

}


