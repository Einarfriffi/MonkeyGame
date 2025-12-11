using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.05f; // time to reach full speed
    public float deccelerationTime = 0.05f; // time to stop

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    [Header("Jump Timers")]
    public float jumpBufferTime = 0.1f;
    public float coyoteTime = 0.1f;
    [Header("Double Jump")]
    public int maxExtraJumps = 1;
    [Header("Wall Jump")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;
    public float wallStickTime = 0.2f;
    public float wallJumpForce = 10f;
    public Vector2 wallJumpDirection = new Vector2(1f, 1f);
    public LayerMask wallLayer;

    [Header("Wall stick control")]
    public float wallStickCooldown = 0.2f;

    [Header("Aiming")]
    public Transform visualTransform;
    public Transform handTransform;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentVelocityX;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private int extraJumpsLeft;
    private bool isTouchingWall;
    private bool previousWallTouch;
    private float wallStickCounter;
    private bool canWallStick = true;
    private float wallStickCooldownTimer;
    private Camera mainCamera;
    private Vector2? lastWallJumpPosition = null;
    private int wallContactDirection = 0;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AimAtMouse();
    }

    // called by Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpBufferCounter = jumpBufferTime;
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.fixedDeltaTime;
        

        // horizontal motion
        float targetSpeedX = moveInput.x * maxSpeed;

        // SmoothDamp gradually changes current velocity to target velocity
        float smoothedSpeedX = Mathf.SmoothDamp(
            rb.linearVelocity.x, // current velocity
            targetSpeedX, // target velocity
            ref currentVelocityX, // ref velocity
            moveInput.x != 0 ? accelerationTime : deccelerationTime // time to reach target velocity
        );

        float newYVelocity = rb.linearVelocity.y;
        Vector2 finalVelocity = new Vector2(smoothedSpeedX, newYVelocity);
        
        // jump
        if (jumpBufferCounter > 0f)
        {
            if (wallStickCounter > 0f)
            {
                float wallDirection = wallContactDirection;

                Vector2 jumpDir = new Vector2(wallJumpDirection.x * wallDirection, wallJumpDirection.y).normalized;

                rb.linearVelocity = new Vector2(jumpDir.x * wallJumpForce, jumpDir.y * wallJumpForce);

                jumpBufferCounter = 0f;
                wallStickCounter = 0f;
                extraJumpsLeft = 0;

                if (isTouchingWall)
                    lastWallJumpPosition = wallCheck.position;

                return;
            }
            else if (coyoteTimeCounter > 0f)
            {
                // Ground/coyote jump
                newYVelocity = jumpForce;
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
            else if (extraJumpsLeft > 0)
            {
                // Air jump
                newYVelocity = jumpForce;
                extraJumpsLeft--;
                jumpBufferCounter = 0f;
            }

        }

        // apply wall stick
        if (wallStickCounter > 0f && isTouchingWall && !IsGrounded())
        {
            finalVelocity.x = 0f;
            finalVelocity.y = 0f;
        }
        else
        {
            finalVelocity.y = newYVelocity;
        }

        rb.linearVelocity = finalVelocity;
    }

    private void CheckGround()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer))
        {
            coyoteTimeCounter = coyoteTime;
            extraJumpsLeft = maxExtraJumps;
            canWallStick = true;
            wallStickCooldownTimer = 0f;
            lastWallJumpPosition = null;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    private void CheckWall()
    {
        // Raycast in the direction the player is facing
        Vector2 boxSize = new Vector2(1f, 1f);
        Collider2D hit = Physics2D.OverlapBox(wallCheck.position, boxSize, 0f, wallLayer);
        isTouchingWall = hit != null;

        if (hit != null)
        {
            isTouchingWall = true;

            // det which side the wall is on relative to player
            if (hit.transform.position.x < transform.position.x)
                wallContactDirection = -1;
            else
                wallContactDirection = 1;
        }
        else
        {
            isTouchingWall = false;
            wallContactDirection = 0;
        }
        
        if (isTouchingWall && hit != null && !IsGrounded())
        {
            if (lastWallJumpPosition.HasValue)
            {
                float xDistance = Mathf.Abs(wallCheck.position.x - lastWallJumpPosition.Value.x);
                if (xDistance < 0.1f)
                {
                    // still same vertical wall
                    isTouchingWall = false;
                }
            }
        }

        if (wallStickCooldownTimer > 0f)
            wallStickCooldownTimer -= Time.fixedDeltaTime;

        bool cooldownActive = wallStickCooldownTimer > 0f;

        if (canWallStick && !cooldownActive && isTouchingWall && !IsGrounded())
        {
            if (!previousWallTouch)
            {
                wallStickCounter = wallStickTime;
                Debug.Log("wall stick Activated");
            }
            else
            {
                wallStickCounter -= Time.fixedDeltaTime;

                if (wallStickCounter <= 0f)
                {
                    canWallStick = false;
                    wallStickCooldownTimer = wallStickCooldown;
                }
            }
        }
        else if (!isTouchingWall || IsGrounded())
        {
            wallStickCounter = 0f;
        }
        
        previousWallTouch = isTouchingWall;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


    private void AimAtMouse()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        // aim direction
        Vector2 direction = mouseWorldPos - handTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // det dir based on mouse X
        float playerDirection = mouseWorldPos.x < transform.position.x ? -1f : 1f;

        // flip visual's scale
        visualTransform.localScale = new Vector3(playerDirection, 1f, 1f);

        Vector3 localPos = visualTransform.localPosition;
        // Mirror pos offset
        float offsetRight = 0f;
        float offsetLeft = 0.65f;

        localPos.x = (playerDirection > 0) ? offsetRight : offsetLeft;
        visualTransform.localPosition = localPos;

        // if player flipped, invert angle to mirror aiming
        if (playerDirection == -1f)
            angle += 180f;

        handTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 size = new Vector3(1f, 1f, 0f);
            Gizmos.DrawWireCube(wallCheck.position, size);
        }
    }
}
