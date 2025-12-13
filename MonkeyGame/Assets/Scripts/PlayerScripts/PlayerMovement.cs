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
    
    [Tooltip("Wall-jump coyote time")]
    public float wallCoyoteTime = 0.12f;

    [Header("Wall stick control")]
    [Tooltip("Cooldown after wall stick ends before the player can stick again.")]
    public float wallStickCooldown = 0.2f;
    
    [Tooltip("Size of wall detection box")]
    public float wall_box_size = 1f;

    [Header("Air Control")]
    [Tooltip("Duration where horizontal input don't cancel momentum (unless player moves)")]
    public float wallJumpControlLock = 0.15f;

    [Tooltip("If true, any horizontal input cancels the lock early")]
    public bool cancelWallJumpLockOnInput = true;

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

    [Header("Sound Effects")]
    public AudioClip jumpSoundClip;
    public AudioClip doubleJumpSoundClip;
    public AudioClip wallJumpSoundClip;
    public AudioClip shootSoundClip;
    public AudioClip runningSoundClip;
    public AudioClip landSoundClip;



    // my privates
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentVelocityX;
    private bool isRunning = false;
    private bool wasGrounded;
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
    private float wallCoyoteLeft = 0f;
    private float wallCoyoteRight = 0f;
    private bool wallCoyoteConsumedLeft = false, wallCoyoteConsumedRight = false;
    private int lastWallSideTouched = 0;
    private float wallJumpControlTimer = 0f;




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

        bool groundedNow = IsGrounded();

        if (!wasGrounded && groundedNow)
        {
            // just landed
            SFXManager.instance.PlaySoundEffect(landSoundClip, transform, 1f);

            // optional: stop running loop briefly or restart based on isRunning logic
        }

        wasGrounded = groundedNow;


        CheckWall();

        // wall jump control tick down
        if (wallJumpControlTimer > 0f) wallJumpControlTimer -= Time.fixedDeltaTime;

        // Wall coyote tick down
        if (wallCoyoteLeft > 0f) wallCoyoteLeft -= Time.fixedDeltaTime;
        if (wallCoyoteRight > 0f) wallCoyoteRight -= Time.fixedDeltaTime;

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.fixedDeltaTime;

        // horizontal movement with post wall jump momentum lock
        float xInput = Mathf.Abs(moveInput.x) < inputDeadzone ? 0f : moveInput.x;
        bool lockActive = wallJumpControlTimer > 0f && !IsGrounded() &&
                        (!cancelWallJumpLockOnInput || Mathf.Abs(xInput) < 0.0001f);

        if (!lockActive)
        {
            float targetSpeedX = xInput * maxSpeed;
            float smoothedSpeedX = Mathf.SmoothDamp(
            rb.linearVelocity.x,
            targetSpeedX,
            ref currentVelocityX,
            (Mathf.Abs(targetSpeedX) > 0.01f) ? accelerationTime : deccelerationTime
        );

            // Grounded snap to zero (but dont fight air momentum)
            if (IsGrounded() && xInput == 0f && Mathf.Abs(smoothedSpeedX) < 0.02f)
            {
                smoothedSpeedX = 0f;
                currentVelocityX = 0f;        
            }


            rb.linearVelocity = new Vector2(smoothedSpeedX, rb.linearVelocity.y);

            // ---- Running state (put after horizontal movement code, before jump checks) ----
            bool wasRunning = isRunning;

            // treat "running" as grounded + actually moving horizontally
            isRunning = IsGrounded() && Mathf.Abs(rb.linearVelocity.x) > 0.1f;

            if (!wasRunning && isRunning)
            {
                // start loop
                SFXManager.instance.PlayLoop(runningSoundClip, 1f);
            }
            else if (wasRunning && !isRunning)
            {
                // stop loop
                SFXManager.instance.StopLoop();
            }
        }
        // jump
        if (jumpBufferCounter > 0f)
        {
            // allow wall-jump if sticking OR have an unconsumed wall-coyote on either side
            bool canCoyoteLeft  = (wallCoyoteLeft  > 0f) && !wallCoyoteConsumedLeft;
            bool canCoyoteRight = (wallCoyoteRight > 0f) && !wallCoyoteConsumedRight;
            bool canWallCoyote  = canCoyoteLeft || canCoyoteRight;

            if (wallStickCounter > 0f || canWallCoyote)
            {
                // choose side: if not currently sticking, use the coyote side that’s available
                int wallDirection = wallContactDirection;
                if (wallDirection == 0)
                    wallDirection = canCoyoteLeft ? -1 : 1;

                // always jump away
                int away =- wallDirection;

                Vector2 jumpDir = new Vector2(wallJumpDirection.x * away, wallJumpDirection.y).normalized;
                rb.linearVelocity = new Vector2(jumpDir.x * wallJumpForce, jumpDir.y * wallJumpForce);

                // play wall jump sound
                SFXManager.instance.PlaySoundEffect(wallJumpSoundClip, transform, 1f);

                wallJumpControlTimer = wallJumpControlLock;

                // clear smoothdamp for not snapping after lock
                currentVelocityX = 0f;

                // prevent instant restick
                canWallStick = false;
                wallStickCooldownTimer = wallStickCooldown;

                // consume buffer & mark the used coyote side as spent
                jumpBufferCounter = 0f;
                wallStickCounter = 0f;
                extraJumpsLeft = 0;

                if (wallDirection == -1) wallCoyoteConsumedLeft  = true;
                if (wallDirection ==  1) wallCoyoteConsumedRight = true;

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

                //play jump sound
                SFXManager.instance.PlaySoundEffect(jumpSoundClip, transform, 1f);
            }
            else if (extraJumpsLeft > 0)
            {
                var v = rb.linearVelocity;
                v.y = jumpForce;
                rb.linearVelocity = v;
                extraJumpsLeft--;
                jumpBufferCounter = 0f;

                //play double jump sound
                SFXManager.instance.PlaySoundEffect(doubleJumpSoundClip, transform, 1f);
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
            // play shoot sound
            SFXManager.instance.PlaySoundEffect(shootSoundClip, transform, 0.7f);
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
            wallCoyoteConsumedLeft = wallCoyoteConsumedRight =false;
            lastWallSideTouched = 0;
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
        Vector2 boxSize = new Vector2(wall_box_size, wall_box_size);

        float x = moveInput.x;
        float facing = Mathf.Abs(x) > 0.01f ? Mathf.Sign(x) : 1f;
        Vector2 origin = (Vector2)wallCheck.position + new Vector2(0.05f * facing, 0f);

        // cast slightly ahead to get hit with a surface normal
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.right * facing, 0.05f, wallLayer);

        // Flags
        isTouchingWall = hit.collider != null;

        // side: -1 = left wall, 1 = right wall
        if (isTouchingWall)
            wallContactDirection = (hit.normal.x > 0f) ? -1 : 1;
        else
            wallContactDirection = 0;

        // refresh side we're touching
        // refresh coyote timers for the side we're touching
        if (isTouchingWall)
        {
            // if we switched wall sides, allow one-use again on the new side
            if (wallContactDirection != 0 && wallContactDirection != lastWallSideTouched)
            {
                // reset one-use gates and timers when switching sides
                wallCoyoteConsumedLeft = wallCoyoteConsumedRight = false;
                wallCoyoteLeft = wallCoyoteRight = 0f;
                lastWallSideTouched = wallContactDirection;
            }

            if (wallContactDirection == -1)
                wallCoyoteLeft = wallCoyoteTime;   // refill while touching left wall
            else if (wallContactDirection == 1)
                wallCoyoteRight = wallCoyoteTime;  // refill while touching right wall
        }


        // "Same wall" suppression after a wall-jump
        if (isTouchingWall && hit.collider != null && !IsGrounded())
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
        if (wallStickCooldownTimer > 0f)
            wallStickCooldownTimer -= Time.fixedDeltaTime;

        bool cooldownActive = wallStickCooldownTimer > 0f;


        if (canWallStick && !cooldownActive && isTouchingWall && !IsGrounded())
        {
            if (!previousWallTouch)
            {
                wallStickCounter = wallStickTime;
                Debug.Log("wall stick Activated");
                Debug.Log($"sticking to wall: {wallContactDirection}");
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
        if(dead) return;
        
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
            Vector2 boxSize = new Vector2(wall_box_size, wall_box_size);
            float facing = 1f;
            Vector2 origin = (Vector2)wallCheck.position + new Vector2(0.05f * facing, 0f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(origin, boxSize);
            Gizmos.DrawLine(origin, origin + Vector2.right * 0.25f); // cast direction preview
        }


        // if (wallCheck != null)
        // {
        //     Vector2 boxSize = new Vector2(wall_box_size, wall_box_size);
        //     float facing = 1f;
        //     Vector2 origin = (Vector2)wallCheck.position + new Vector2(0.05f * facing, 0f);
        //     Gizmos.color = Color.magenta;
        //     Gizmos.DrawWireCube(origin, boxSize);
        // }
        
    }
}
