using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 horizonalInput = Vector2.zero;
    //public to debug
    public PlayerMovementState playerMovementState = PlayerMovementState.Grounded;
    public float playerSpeed = 5000f;
    public float groundedDrag = 3.0f;
    public float fallSpeed = 3000f;
    private float baseSpeed = 0f;
    private bool jump;
    public float jumpPower = 5000f;
    public float jumpSpeed = 3000f;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private bool doOnce = true;
    private Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        rb.freezeRotation = true;
        baseSpeed = playerSpeed;
        groundCheck = this.gameObject.transform.GetChild(0);
    }

    void Update()
    {
        horizonalInput = new Vector2(Input.GetAxis("Horizontal"), 0f);
        jump = Input.GetKey(KeyCode.Space);

        //Check to see if the player is jumping or grounded.
        switch (playerMovementState)
        {
            case PlayerMovementState.Jumping:
                playerSpeed = jumpSpeed;
                rb.drag = 0f;

                /* First Code for jumping currently commented out for testing other coding samples
                
                RaycastHit2D jumpHit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
                Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.red);
                //If player is close enough to ground
                
                if (jumpHit.collider != null)
                {
                    Debug.Log(jumpHit.collider.gameObject.layer);
                    if (jumpHit.collider.gameObject.layer == 7 || jumpHit.collider.gameObject.layer == 8)
                    {
                        playerMovementState = PlayerMovementState.Grounded;
                    }
                }
                
                */
                break;
                //When grounded, go back to normal speed.
            case PlayerMovementState.Grounded:
                playerSpeed = baseSpeed;
                rb.drag = groundedDrag;
                doOnce = true;
                break;
            case PlayerMovementState.Falling:
                playerSpeed = fallSpeed;
                rb.drag = 0f;

                /* First Code for jumping currently commented out for testing other coding samples
                
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
                Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.red);
                //If player is close enough to ground
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.layer);
                    if (hit.collider.gameObject.layer == 7 || hit.collider.gameObject.layer == 8)
                    {
                        playerMovementState = PlayerMovementState.Grounded;
                    }
                }
                */

                break;

            //case PlayerMovementState.inMagnet:
            //    rb.drag = 0;
            //    playerSpeed = jumpSpeed;
            //    break;
            default:
                break;
        }

        if (rb.velocity.y < -0.2f && !IsGrounded())
        {
            playerMovementState = PlayerMovementState.Falling;
        }
        else if (IsGrounded())
        {
            playerMovementState = PlayerMovementState.Grounded;
        }
        //Bugged when player jumps on an object within a magnet, or walks into a magnet.
        //Trade off is that magnets need to be jumped into to properly get the right drag. 
        //else if (!jump && rb.velocity.y > 0.1f && playerMovementState == PlayerMovementState.Grounded)
        //{
        //    playerMovementState = PlayerMovementState.inMagnet;
        //}
    }



    private void FixedUpdate()
    {
        //Processing the horizontal movement.
        if (Input.GetButton("Horizontal"))
        {
            rb.AddForce(horizonalInput.normalized * playerSpeed);
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
               playerSprite.flipX = true;
            }
            else
            {
                playerSprite.flipX = false;
            }
        }


        //Animation processing
        if(rb.velocity.x != 0 && IsGrounded())
        {
            playerAnimator.SetBool("IsWalking", true);
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
        }
        //Added a jump
        if (jump && doOnce && playerMovementState == PlayerMovementState.Grounded)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            playerMovementState = PlayerMovementState.Jumping;
            doOnce = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}

//States for movement.
public enum PlayerMovementState
{
    //inMagnet,
    Jumping,
    Grounded,
    Falling,
}
