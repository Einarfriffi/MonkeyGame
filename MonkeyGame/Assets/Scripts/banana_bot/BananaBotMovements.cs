using UnityEngine;

public class BananaBotMovements : MonoBehaviour
{
    // Variable for path control and speed control
    [Header("Basic Settings")]
    [SerializeField] public float viewDistance = 5f;
    [SerializeField] private float idle_speed;

    [Header("Travel distance idle")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private Vector2 stopTimeRandomRange;
    [SerializeField] private Vector2 driveTimeRandomRange;
    [SerializeField] private LayerMask targetLayer;

    [Header("Laser Settings")]
    [SerializeField] public float endSizeLaser = 0.025f;
    [SerializeField] public float baseSizeLaser = 0.025f;
    [SerializeField] public Color laserColor;

    [Header("Core Components")]
    [SerializeField] private GameObject laser;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D stun_collider;
    [SerializeField] private Collider2D weak_spot;



    // iniate direction, body and renderer
    private int direction = -1;

    // stop stuff
    private float pauseTimer = 0f;
    private float driveTimer = 0f;
    private bool isPaused = true;

    private Vector3 startPos;
    private float pre_y_pos;

    // components 
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Fetch Componenets
        rb = GetComponent<Rigidbody2D>();
        // lock body rotation
        rb.freezeRotation = true;
        sr = GetComponentInChildren<SpriteRenderer>();

        startPos = transform.position;
        minX = startPos.x - minX;
        maxX = startPos.x + maxX;
        // checks if there are any walls in travel distance
        wall_check();
        //Debug.Log("min " + minX);
        //Debug.Log("max " + maxX);

        laser.SetActive(true);

        pauseTimer = Random.Range(stopTimeRandomRange.x, stopTimeRandomRange.y);
        driveTimer = Random.Range(stopTimeRandomRange.x, stopTimeRandomRange.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        // if robot is driving
        if (isPaused)
        {
            // stops and looks around
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer = Random.Range(stopTimeRandomRange.x, stopTimeRandomRange.y);
                driveTimer = Random.Range(driveTimeRandomRange.x, driveTimeRandomRange.y);
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


    void wall_check()
    {
        // Cast a ray downwards from the object's position
        RaycastHit2D hit_min = Physics2D.Raycast(transform.position, Vector2.right, minX, targetLayer);
        RaycastHit2D hit_max = Physics2D.Raycast(transform.position, Vector2.left, maxX, targetLayer);

        // Check if the ray hit something
        if (hit_min.collider != null)
        {
            //Debug.Log("Hit min" + hit_min.collider.name);
            //Debug.Log("Hit min distance" + hit_min.distance);
            minX += hit_min.distance - 0.7f;
        }
        if (hit_max.collider != null)
        {
            //Debug.Log("Hit max" + hit_max.collider.name);
            //Debug.Log("Hit max distance" + hit_max.distance);
            maxX -= hit_max.distance - 0.7f;
        }

    }
}