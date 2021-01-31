using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSimple : MonoBehaviour
{
    public float jumpForce = 1;
    public float moveSpeed = 1;
    public Transform groundCheck;
    public LayerMask groundObjects;
    public LayerMask WallObjects;
    public float checkRadius;
    public int maxJumpCount;
    public Transform wallCheckerFront;
    public Transform wallCheckerBack;
    public float dashForce = 50f;
    public Transform headChecker;
    public Transform footChecker;
    public float dashCooldown;
    public float dashStartCooldown;
    
    public bool canIDash = true;
    public bool facingRight = true;
    public bool isJumping = false;
    public bool isGrounded;
    public bool isWalledFront;
    public bool isWalledBack;
    public bool isCoyoteJumping;
    public Vector2 savedVelocity;
    
    






    private Rigidbody2D rb;
    private float moveDirection;
    private int jumpCount;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    //Start is called before the first frame update
    void Start()
    {
        jumpCount = maxJumpCount;
        dashCooldown = dashStartCooldown;



    }

    //Update is called once per frame
    void Update()
    {
        getInput();  // Inputok, jobb-bal



        AnimateFlip();
        //Debug.Log(jumpCount);
        if (isGrounded || isWalledBack || isWalledFront)
        {
            jumpCount = maxJumpCount;
        }
        else if (isGrounded == false || isWalledBack == false || isWalledFront == false)
        {
            StartCoroutine(CoyoteJump());
        }



    }

    private void FixedUpdate()
    {
        Move(); // mozog, "meglökjük"
        Dash();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundObjects);
        isWalledFront = Physics2D.OverlapCircle(wallCheckerFront.position, checkRadius, WallObjects) || Physics2D.OverlapCircle(headChecker.position, checkRadius, WallObjects) || Physics2D.OverlapCircle(footChecker.position, checkRadius, WallObjects);
        isWalledBack = Physics2D.OverlapCircle(wallCheckerBack.position, checkRadius, WallObjects);








    }


    private void Move()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        if (isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpCount--;
        }
        isJumping = false;









    }

    private void AnimateFlip()
    {
        if (moveDirection > 0 && !facingRight)
        {
            FlipCharacter();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipCharacter();
        }
    }

    private void getInput()
    {
        moveDirection = Input.GetAxis("Horizontal");


        if (Input.GetButtonDown("Jump") && jumpCount >= 1) //  vagy ha csak egyet akarok ugrani :  isGrounded
        {
            isJumping = true;
        }

    }

    private void FlipCharacter()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180, 0f);
    }

    private void Dash()
    {
        if (dashCooldown > 0)
        {
            canIDash = false;
            dashCooldown -= Time.fixedDeltaTime;
            //if I put saved velocity here, it will continue to move at savedVelocity until dashCooldown = 0? so it can't go here? right?
        }
        if (dashCooldown <= 0)
        {
            canIDash = true;
            //if I put savedVelocity here it doesn't return to savedVelocity until dashCooldown <=0 so... it doesn't go here either right...?
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canIDash == true)
        {
            //saves velocity prior to dashing
            savedVelocity = rb.velocity;
            //this part is the actual dash itself
            rb.velocity = new Vector2(rb.velocity.x * dashForce, rb.velocity.y);
            //sets up a cooldown so you have to wait to dash again
            dashCooldown = dashStartCooldown;
        }



    }

    IEnumerator CoyoteJump()
    {
        rb.gravityScale = 0f;
        isCoyoteJumping = true;
        yield return new WaitForSeconds(1);
        isCoyoteJumping = false;
    }
    

    
}
