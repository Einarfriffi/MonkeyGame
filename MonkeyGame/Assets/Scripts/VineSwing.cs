using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VineSwing : MonoBehaviour
{
    [SerializeField] private float idleTorque = 0.05f;     
    [SerializeField] private float hitTorque = 1.5f;
    [SerializeField] private string monkeyTag = "Player";

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        float direction = Random.value > 0.5f ? 1f : -1f;
        _rb.AddTorque(idleTorque * direction, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        _rb.AddTorque(idleTorque * 0.01f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(monkeyTag)) return;

        float direction = collision.transform.position.x < transform.position.x ? 1f : -1f;

        _rb.AddTorque(hitTorque * direction, ForceMode2D.Impulse);
    }
}
