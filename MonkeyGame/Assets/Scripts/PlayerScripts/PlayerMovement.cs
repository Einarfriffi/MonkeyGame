using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Maximum horizontal speed in world units per second.")]
    public float maxSpeed = 5f;

    [Tooltip("Seconds to accelerate from 0 to the target speed on the ground.")]
    public float accelerationTime = 0.05f; // time to reach full speed

    [Tooltip("Seconds to decelerate to 0 when no horizontal input.")]
    public float deccelerationTime = 0.05f; // time to stop

    [Header("Input")]
    [Tooltip("Ignore tiny stick/mouse drift; values below this are treated as 0.")]
    public float inputDeadzone = 0.15f;

    [Header("Jump Settings")]
    [Tooltip("Upward velocity applied when a jump occurs (higher = higher jump).")]
    public float jumpForce = 10f;

    [Tooltip("Transform used as the center for ground checks.")]
    public Transform groundCheck;

    [Tooltip("Radius of the ground check circle (in world units).")]
    public float groundCheckRadius = 0.1f;

    [Tooltip("Layers that count as ground for ground checks.")]
    public LayerMask groundLayer;

    [Header("Jump Timers")]
    [Tooltip("Time window to buffer a jump pressed slightly before landing.")]
    public float jumpBufferTime = 0.1f;

    [Tooltip("Coyote time: grace period after leaving ground where jump still works.")]
    public float coyoteTime = 0.1f;

    [Header("Double Jump")]
    [Tooltip("How many extra jumps are allowed while airborne.")]
    public int maxExtraJumps = 1;

    [Header("Wall Jump")]
    [Tooltip("Transform used as the origin for the wall overlap box.")]
    public Transform wallCheck;

    [Tooltip("How long the player initially 'sticks' to a wall when touching it in air.")]
    public float wallStickTime = 0.2f;

    [Tooltip("Strength of the wall jump impulse (applied in wallJumpDirection).")]
    public float wallJumpForce = 10f;

    [Tooltip("Direction of the wall jump relative to the wall (normalized X,Y).")]
    public Vector2 wallJumpDirection = new Vector2(1f, 1f);

    [Tooltip("Layers that count as walls for wall detection.")]
    public LayerMask wallLayer;

    [Header("Wall stick control")]
    [Tooltip("Cooldown after wall stick ends before the player can stick again.")]
    public float wallStickCooldown = 0.2f;

    [Header("Aiming")]
    [Tooltip("Child transform for visuals (flipped/scaled). Do NOT assign the root with the Rigidbody2D.")]
    public Transform visualTransform;

    [Tooltip("Hand/Weapon pivot used for aiming and as the fallback muzzle.")]
    public Transform handTransform;

    [Header("Shooting")]
    [Tooltip("Projectile prefab (must have a Rigidbody2D + Collider2D).")]
    public GameObject projectilePrefab;

    [Tooltip("Spawn position at the gun tip, If null, uses handTransform.")]
    public Transform muzzleTransform;

    [Tooltip("Projectiles per second while holding fire.")]
    public float fireRate = 8f;

    [Tooltip("Initial projectile speed in world units per second.")]
    public float projectileSpeed = 18f;

    [Tooltip("Rigidbody2D gravity scale applied to the projectile (0 = no drop).")]
    public float projectileGravityScale = 0f;

    [Tooltip("Seconds before a spawned projectile auto-destroys.")]
    public float projectileLifetime = 3f;

    [Tooltip("Player velocity for projectile to inherit")]
    public float projectileInheritVelocity = 1f;

    [Tooltip("If true, only inherit horizontal velocity")]
    public bool inheritHorizontalOnly = true;


    // my privates
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
    private bool fireHeld;
    private float nextFireTime;
    private bool dead = false;

    private float wall_grace_peroid = 0.5f;
    private float wall_grace_peroid_time = 0;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        if (visualTransform == null || visualTransform == transform)
            Debug.LogError("VisualTransform must be a Child of the player");
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

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) fireHeld = true;
        else if (context.canceled) fireHeld = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.fixedDeltaTime;

        // horizontal movement
        float xInput = Mathf.Abs(moveInput.x) < inputDeadzone ? 0f : moveInput.x;

        float targetSpeedX = xInput * maxSpeed;
        float smoothedSpeedX = Mathf.SmoothDamp(
        rb.linearVelocity.x,
        targetSpeedX,
        ref currentVelocityX,
        (Mathf.Abs(targetSpeedX) > 0.01f) ? accelerationTime : deccelerationTime
       );

        if (xInput == 0f && Mathf.Abs(smoothedSpeedX) < 0.02f)
        {
            smoothedSpeedX = 0f;
            currentVelocityX = 0f;
        }

        if (Mathf.Abs(xInput) < 0.0001f)
            smoothedSpeedX = Mathf.MoveTowards(smoothedSpeedX, 0f, 100f * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(smoothedSpeedX, rb.linearVelocity.y);

        // jump
        if (jumpBufferCounter > 0f)
        {
            if (wallStickCounter > 0f)
            {
                // new shit
                wall_grace_peroid_time = wall_grace_peroid;

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
                var v = rb.linearVelocity;
                v.y = jumpForce;
                rb.linearVelocity = v;
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
            else if (extraJumpsLeft > 0)
            {
                var v = rb.linearVelocity;
                v.y = jumpForce;
                rb.linearVelocity = v;
                extraJumpsLeft--;
                jumpBufferCounter = 0f;
            }

        }

        // apply wall stick
        if (wallStickCounter > 0f && isTouchingWall && !IsGrounded())
        {
            rb.linearVelocity = Vector2.zero;
        }

        // shooting
        if (fireHeld && Time.time >= nextFireTime)
        {
            SpawnProjectile();
            nextFireTime = Time.time + (fireRate > 0f ? 1f / fireRate : 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // collide with hazards kills
        if (other.gameObject.CompareTag("Hazards"))
        {
            dead = true;
            // TODO: add animation
            GameManager.Instance.ShowDeathScreen();
        }

        if (other.gameObject.CompareTag("Win"))
        {
            GameManager.Instance.LevelWon();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // collide with hazards kills
        if (other.CompareTag("Hazards"))
        {
            dead = true;
            // TODO: add animation
            GameManager.Instance.ShowDeathScreen();
        }

        if (other.CompareTag("Win"))
        {
            GameManager.Instance.LevelWon();
        }
    }

    // ground check for movement and jumping
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

    // wall check for wall sticking
    private void CheckWall()
    {
        // Tight probe slightly ahead of the player
        Vector2 boxSize = new Vector2(1f, 1f);

        float x = moveInput.x;
        float facing = Mathf.Abs(x) > 0.01f ? Mathf.Sign(x) : 1f;
        Vector2 origin = (Vector2)wallCheck.position + new Vector2(0.05f * facing, 0f);

        // Single overlap query using the intended origin
        Collider2D hit = Physics2D.OverlapBox(origin, boxSize, 0f, wallLayer);

        // Set flags once
        isTouchingWall = hit != null;
        wallContactDirection = isTouchingWall
            ? (hit.transform.position.x < transform.position.x ? -1 : 1)
            : 0;

        // "Same wall" suppression after a wall-jump
        if (isTouchingWall && hit != null && !IsGrounded())
        {
            //Debug.Log(wallCheck.position.x);
            if (lastWallJumpPosition.HasValue)
            {
                float xDistance = Mathf.Abs(wallCheck.position.x - lastWallJumpPosition.Value.x);
                // TODO NEW FIX
                if (xDistance < 0.3f)
                {
                    isTouchingWall = false;
                    wallContactDirection = 0; // <- also clear the direction
                }
            }
        }

        // Stick timer / cooldown (unchanged)
        // if (wallStickCooldownTimer > 0f)
        //     wallStickCooldownTimer -= Time.fixedDeltaTime;

        // bool cooldownActive = wallStickCooldownTimer > 0f;

        //if (canWallStick && !cooldownActive && isTouchingWall && !IsGrounded())
        if (canWallStick && isTouchingWall && !IsGrounded())
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
        if (Time.timeScale == 0) return;
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

    private void SpawnProjectile()
    {
        if (projectilePrefab == null) return;

        Transform spawn = muzzleTransform != null ? muzzleTransform : handTransform;

        // aim toward mouse, but only use Direction
        Vector2 mouseScreen = Mouse.current.position.ReadValue();

        // project mouse onto the same Z plane as the muzzle
        float planeZ = spawn.position.z - mainCamera.transform.position.z;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, planeZ));

        // normalized 2D direction from muzzle to mouse
        Vector2 dir = (Vector2)(mouseWorld - spawn.position);
        if (dir.sqrMagnitude < 0.0001f) dir = (Vector2)spawn.right;
        else dir = dir.normalized;

        // small forward offset
        Vector3 spawnPos = spawn.position + (Vector3)(dir * 0.15f);

        GameObject go = Instantiate(
            projectilePrefab,
            spawnPos,
            Quaternion.FromToRotation(Vector2.right, dir)
        );

        var prb = go.GetComponent<Rigidbody2D>();
        if (prb != null)
        {
            prb.gravityScale = projectileGravityScale;
            // inherit player, but remove the component along the aim direction
            Vector2 inherited = rb.linearVelocity;
            if (inheritHorizontalOnly) inherited = new Vector2(inherited.x, 0f);

            // Split inherited into along-aim and perpendicular components
            float along = Vector2.Dot(inherited, dir);
            Vector2 inheritedPerp = inherited - dir * along;

            // Final: exact projectileSpeed along aim, plus perpendicular carry scaled to taste
            prb.linearVelocity = dir * projectileSpeed + inheritedPerp * projectileInheritVelocity;
        }

        if (projectileLifetime > 0f) Destroy(go, projectileLifetime);

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
            Vector2 boxSize = new Vector2(1f, 1f);
            float facing = 1f;
            Vector2 origin = (Vector2)wallCheck.position + new Vector2(0.05f * facing, 0f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(origin, boxSize);
        }
    }
}
