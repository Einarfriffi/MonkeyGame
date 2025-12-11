using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [Header("Hit Settings")]
    [Tooltip("Layers that destroy bullet on contact")]
    public LayerMask hitMask;

    [Tooltip("If true any collider ends the projectile")]
    public bool destroyOnAnyCollider = false;

    [Tooltip("Optional impact effect")]
    public GameObject impactVFX;
    public float impactVFXLifetime = 1f;

    [Header("Safety")]
    [Tooltip("Ignore collision right after spawn")]
    public float ignoreForSeconds = 0.03f;
    //public float shieldIgnoreForSeconds = 0.03f;



    private Rigidbody2D rb;
    private bool dead;
    private float spawnTime;
    //private bool lastHitEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /* if (lastHit > 0)
        {
            lastHit -= Time.deltaTime;
        } */
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        dead = false;
        spawnTime = Time.time;
    }

    /* void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        //if (!enabled || dead) return;
        //if (Time.time - spawnTime < ignoreForSeconds) return;

        if (destroyOnAnyCollider || LayerMatches(collision.gameObject.layer))
        {
            Vector2 contactPoint = collision.ClosestPoint(transform.position);

            Die(contactPoint);
        }
    } */

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!enabled || dead) return;
        if (Time.time - spawnTime < ignoreForSeconds) return;

        if (destroyOnAnyCollider || LayerMatches(col.collider.gameObject.layer))
        {
            var contact = col.GetContact(0);
            Vector2 hitPoint = contact.point;
            Vector2 hitNormal = contact.normal;

            // TODO FIX LATER IF TIME REFLECT FOR BULLET IF IT HITS SHIELD
            /* if (col.collider.CompareTag("Shield"))
            {
                Vector2 incoming = rb.linearVelocity;
                Vector2 normal = col.contacts[0].normal;

                // Reflect the bullet's velocity
                Vector2 reflected = Vector2.Reflect(incoming * 3, normal);
                rb.linearVelocity = reflected;
            }
            else
            {
                Die(hitPoint);
            } */
            Die(hitPoint);
        }
    }

    private bool LayerMatches(int layer)
    {
        int bit = 1 << layer;
        return (hitMask.value & bit) != 0;
    }

    private void Die(Vector2 at)
    {
        dead = true;
        if (impactVFX)
        {
            var v = Instantiate(impactVFX, at, Quaternion.identity);
            if (impactVFXLifetime > 0f) Destroy(v, impactVFXLifetime);
        }
        Destroy(gameObject);
    }
}
