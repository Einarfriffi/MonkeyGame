using UnityEngine;

public class BananaBotMovements : MonoBehaviour
{
    // Variable for path control and speed control
    [Header("Basic Settings")]
    [SerializeField] public float viewDistance = 5f;
    [SerializeField] private float idle_speed;
    [SerializeField] private LayerMask Targets;
    [SerializeField] private bool Stationery = false;
    //[SerializeField] private bool facingLeft = true;

    [Header("Attack Settings")]
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float timeBeforeAttack = 0.5f;

    [Header("Travel distance idle")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private Vector2 stopTimeRandomRange;
    [SerializeField] private Vector2 driveTimeRandomRange;
    [SerializeField] private LayerMask targetLayerMove;

    [Header("Scanner Settings")]
    [SerializeField] public float endSizeLaser = 0.025f;
    [SerializeField] public float baseSizeLaser = 0.025f;
    [SerializeField] public Color laserColor;
    [SerializeField] public Color laserColorAngry;
    [SerializeField] public Color laserColorDetecting;
    [SerializeField] public float blinkSpeed = 20f; // higher = faster blinking
    [SerializeField] public float laserReSize = 0.04f;

    [Header("Core Components")]
    [SerializeField] private GameObject laser;
    [SerializeField] private Animator animator;
    //[SerializeField] private Collider2D stun_collider;
    [SerializeField] public Transform eyePos;
    [SerializeField] private Animator shieldAnimator;
    [SerializeField] private GameObject ExplosionPreFab;

    // TODO maybe remove?
    //[SerializeField] private Collider2D weak_spot;
    //[SerializeField] private Collider2D shield;
    //[SerializeField] public Transform Player;
    private scanner_script scannerScript;

    [Header("Fixy Settings")]
    [SerializeField] private float belowThresHoldY = 0.5f;
    [SerializeField] private float belowThresHoldAngle = 4;

    [Header("Debug Stuff")]
    [SerializeField] bool targetLaser = false;
    public float detection_angle;
    public int direction = -1;
    public bool SeePlayer = false;

    // stop stuff
    private float pauseTimer = 0f;
    private float driveTimer = 0f;
    private bool isPaused = true;
    private Vector3 startPos;
    // components 
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Transform Player;

    // attack things and such
    private float lastSeen = 0;
    private float curLaserSize;
    private float growRateLaser;

    [Header("Sound Effects")]
    public AudioClip scanningSoundClip;
    [SerializeField, Range(0f, 1f)] private float scanningSoundVolume = 1f;
    public AudioClip dyingSoundClip;
    [SerializeField, Range(0f, 1f)] private float dyingSoundVolume = 1f;
    public AudioClip shieldtHitClip;
    [SerializeField, Range(0f, 1f)] private float shieldHitVolume = 1f;

    private AudioSource scanningSource;
    private bool wasSeeingPlayer = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /* if (!facingLeft)
        {
            direction = 1;
        } */
        float yRot = transform.eulerAngles.y;
        if (Mathf.Abs(yRot - 180f) < 1f)
            direction = 1;   // facing right
        else
            direction = -1;  // facing left


        // Fetch Componenets
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // get player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Player = playerObj.transform;

        // get the scanner script
        scannerScript = GetComponentInChildren<scanner_script>();

        // lock body rotation
        rb.freezeRotation = true;

        startPos = transform.position;
        minX = startPos.x - minX;
        maxX = startPos.x + maxX;

        // laser grow/shrink rate
        growRateLaser = timeBeforeAttack / 100;

        // checks if there are any walls in travel distance, to fix maxX and minX
        //wall_check();

        laser.SetActive(true);

        pauseTimer = UnityEngine.Random.Range(stopTimeRandomRange.x, stopTimeRandomRange.y);
        driveTimer = UnityEngine.Random.Range(stopTimeRandomRange.x, stopTimeRandomRange.y);

        // find the angle that the bot can see the player
        FindDetectionAngle();

        scanningSource = gameObject.AddComponent<AudioSource>();
        scanningSource.clip = scanningSoundClip;
        scanningSource.loop = true;
        scanningSource.playOnAwake = false;
        scanningSource.volume = scanningSoundVolume * (SFXManager.instance != null ? SFXManager.instance.MasterVolume : 1f);
        scanningSource.spatialBlend = 0f; // 2D sound
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // working on detecting player
        PlayerDetection();

        UpdateScanningSound();

        // ============================
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));

        // if the bot has a visual on the player
        AttackPlayer();

        // if the robot is meant to move around
        if (!Stationery)
        {
            NotStationery();
        }

    }

    void AttackPlayer()
    {
        if (SeePlayer)
        {
            if (lastSeen > 0)
            {
                bool blinkState = Mathf.FloorToInt(Time.time * blinkSpeed) % 2 == 0;
                if (blinkState)
                {
                    scannerScript.SetLaserColor(laserColor);
                }
                else
                {
                    scannerScript.SetLaserColor(laserColorDetecting);
                }

                lastSeen -= Time.deltaTime;

                if (curLaserSize > endSizeLaser / 2)
                {
                    scannerScript.SetLaserSize(curLaserSize -= laserReSize);
                }
            }
            else
            {
                scannerScript.SetLaserColor(laserColorAngry);
                rb.linearVelocity = new Vector2(direction * attackSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            //resize laser back to normal sizes
            if (curLaserSize < endSizeLaser)
            {
                scannerScript.SetLaserSize(curLaserSize += laserReSize);
            }


            scannerScript.SetLaserColor(laserColor);
        }
    }

    void NotStationery()
    {
        // if robot is driving
        if (isPaused)
        {
            // stops and looks around
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer = UnityEngine.Random.Range(stopTimeRandomRange.x, stopTimeRandomRange.y);
                driveTimer = UnityEngine.Random.Range(driveTimeRandomRange.x, driveTimeRandomRange.y);
            }
            return;
        }

        //rb.linearVelocity += idle_speed * Time.fixedDeltaTime * (Vector2)transform.right;
        rb.linearVelocity = new Vector2(direction * idle_speed, rb.linearVelocity.y);

        // Flip at edges
        if (transform.position.x <= minX)
        {
            direction = 1;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // face right
        }
        else if (transform.position.x >= maxX)
        {
            direction = -1;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // face left
        }

        // pause stuff
        driveTimer -= Time.deltaTime;
        if (driveTimer <= 0f)
            isPaused = true;
    }


    void PlayerDetection()
    {
        float dist = Vector3.Distance(Player.position, eyePos.position);
        if (dist <= viewDistance)
        {
            float angle = AngleBetween();
            //if (angle < detection_angle)
            if (angle < detection_angle && angle > -detection_angle * belowThresHoldAngle)
            {
                // check if player is below bot
                float temp_y = Player.transform.position.y - transform.position.y;
                if (temp_y > -belowThresHoldY)
                {
                    //Debug.Log(temp_y);
                    // debug laser
                    if (targetLaser)
                        Debug.DrawLine(eyePos.position, Player.position, Color.red);
                    if (PlayerInView())
                    {
                        if (!SeePlayer)
                            animator.SetBool("SeePlayer", true);
                        SeePlayer = true;
                        return;
                    }

                }

            }
        }
        animator.SetBool("SeePlayer", false);
        SeePlayer = false;
        lastSeen = timeBeforeAttack;
    }

    private float AngleBetween()
    {
        float angle;
        if (direction == -1)
        {
            Vector2 dir = new Vector2(-(Player.position.x - eyePos.position.x),
                           Player.position.y - eyePos.position.y);

            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        else
        {
            //Vector2 direction_temp = Player.position - transform.position;
            Vector2 direction_temp = Player.position - eyePos.position;
            angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;
        }
        //Debug.Log(angle);
        return angle;

        /* // find the direction between them
        Vector2 direction_temp = Player.position - eyePos.position;
        // find the angle between the objects
        float angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;

        angle = Mathf.Abs(angle);
        if (direction == -1)
        {
            angle -= 180f;
            angle = Mathf.Abs(angle);
        }

        return angle; */
    }
    void FindDetectionAngle()
    {
        Vector3 temp = new Vector3(eyePos.position.x + viewDistance, eyePos.position.y + endSizeLaser / 2, 0);
        Vector2 direction_temp = temp - eyePos.position;
        float angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;
        detection_angle = angle;
    }
    void wall_check()
    {
        RaycastHit2D hit_min = Physics2D.Raycast(transform.position, Vector2.right, minX, targetLayerMove);
        RaycastHit2D hit_max = Physics2D.Raycast(transform.position, Vector2.left, maxX, targetLayerMove);

        if (hit_min.collider != null)
        {
            minX += hit_min.distance - 0.7f;
        }
        if (hit_max.collider != null)
        {
            maxX -= hit_max.distance - 0.7f;
        }

    }

    private bool PlayerInView()
    {
        //see if there is a line of sight between bot and player

        Vector2 origin = eyePos.position;
        Vector2 target = Player.position;

        Vector2 direction = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewDistance, Targets);
        //Debug.DrawLine(origin, target, Color.red);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
        /* if (hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false; */
    }

    public void WeakSpotHit(Collision2D other)
    {
        if (scanningSource != null)
        scanningSource.Stop();

        // play death sound
        SFXManager.instance.PlaySoundEffect(dyingSoundClip, transform, dyingSoundVolume);

        // explosion VFX
        Instantiate(ExplosionPreFab, transform.position, Quaternion.identity);

        // destroy bot
        Destroy(gameObject);
    }
    public void ShieldtHit(Collision2D other)
    {
        shieldAnimator.SetTrigger("HitShield");

        // play shield sound
        SFXManager.instance.PlaySoundEffect(shieldtHitClip, transform, shieldHitVolume);
    }

    private void UpdateScanningSound()
    {
        float master = (SFXManager.instance != null) ? SFXManager.instance.MasterVolume : 1f;

        // ALWAYS keep loop volume synced to slider + inspector volume
        if (scanningSource != null)
            scanningSource.volume = scanningSoundVolume * master;

        if (SeePlayer && !wasSeeingPlayer)
        {
            // just detected player
            if (scanningSoundClip != null && scanningSource != null)
            {
                scanningSource.time = 0f;
                scanningSource.Play();
            }
        }
        else if (!SeePlayer && wasSeeingPlayer)
        {
            // lost player
            if (scanningSource != null)
                scanningSource.Stop();
        }

        wasSeeingPlayer = SeePlayer;
    }

}