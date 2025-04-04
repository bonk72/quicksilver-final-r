using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    public static float maxMoveSpeed = 10f;
    private float currentMoveSpeed;
    public Rigidbody2D rb2d;
    private Vector2 moveInput;
    public CircleCollider2D coll;
    public CircleCollider2D executeColl;

    public weapon weapon;
    private Vector2 mousePosition;

    public float dashSpeed;

    public float dashLength, dashCooldown = 1f;
    private float temp;

    public bool isDashing;
    private bool canDash = true;
    private bool dashR = false;

    // Bounce back variables
    public float bounceForce = 10f;
    public float bounceLength = 0.2f;
    private bool isBouncing = false;

    // Boolean to disable rotation (e.g., when in shop)
    public bool canRotate = true;
    
    // Master control for all movement (dash, move, rotate)
    private bool canMove = true;
    
    // Store original rigidbody constraints
    private RigidbodyConstraints2D originalConstraints;

    public PlayerTime time;
    void Start()
    {
        executeColl.enabled = false;
        currentMoveSpeed = maxMoveSpeed;
        temp = dashCooldown;
        
        // Store the original rigidbody constraints
        if (rb2d != null)
        {
            originalConstraints = rb2d.constraints;
        }
    }

    void Update()
    {
        if (isDashing || isBouncing || !canMove) {
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
        if (isDashing || isBouncing || !canMove)
        {
            return;
        }
        rb2d.velocity = new Vector2(moveInput.x * currentMoveSpeed, moveInput.y * currentMoveSpeed);

        // Only rotate if canRotate is true and canMove is true
        if (canRotate)
        {
            Vector2 aimDirection = mousePosition - rb2d.position;
            float aimangle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
            rb2d.rotation = aimangle;
        }
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
    public void increaseMaxSpeed(float amount){
        maxMoveSpeed += amount;
        currentMoveSpeed += amount;
    }
    public void increaseCurrMoveSpeed(float amount){
        currentMoveSpeed += amount;
    }
    
    // Accessor method to toggle movement capabilities
    public void ToggleMovement()
    {
        canMove = !canMove;
        UpdateRigidbodyConstraints();
    }
    
    // Accessor method to set movement capabilities directly
    public void SetMovement(bool canPlayerMove)
    {
        canMove = canPlayerMove;
        UpdateRigidbodyConstraints();
    }
    
    // Helper method to update rigidbody constraints based on canMove
    private void UpdateRigidbodyConstraints()
    {
        if (rb2d != null)
        {
            if (!canMove)
            {
                // Freeze position and rotation when movement is disabled
                rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
                
                // Also set velocity to zero to prevent any ongoing movement
                rb2d.velocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
            }
            else
            {
                // Restore original constraints when movement is enabled
                rb2d.constraints = originalConstraints;
            }
        }
    }
    
    // Accessor method to get current movement state
    public bool GetMovement()
    {
        return canMove;
    }
    public void reduceDashCD(float amount){

        temp = temp - amount;
    }
}
