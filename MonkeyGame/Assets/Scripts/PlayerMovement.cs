using UnityEngine;
using UnityEngine.InputSystem;

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

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentVelocityX;
    private bool jumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // called by Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpPressed = true;
    }

    private void FixedUpdate()
    {
        CheckGround();

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

        // jump
        if (jumpPressed && isGrounded)
        {
            newYVelocity = jumpForce;
            jumpPressed = false;
        }

        rb.linearVelocity = new Vector2(smoothedSpeedX, newYVelocity);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
