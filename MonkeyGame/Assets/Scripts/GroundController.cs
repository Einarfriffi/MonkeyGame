using UnityEngine;

public class GroundController : MonoBehaviour
{
    [SerializeField] private float _groundDistanceTolerance = 0.1f;
    [SerializeField] private LayerMask _groundLayerMask;

    private CapsuleCollider2D _capsuleCollider2D;

    public bool IsGrounded { get; private set; }
    public float? DistanceToGround { get; private set; }

    private void Awake()
    {
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        float radius = (_capsuleCollider2D.size.x * 0.5f) - 0.05f;

        Vector2 origin = (Vector2)transform.position -
                         new Vector2(0, _capsuleCollider2D.size.y * 0.5f - radius);

        RaycastHit2D hit = Physics2D.CircleCast(
            origin,
            radius,
            Vector2.down,
            _groundDistanceTolerance + 0.01f,
            _groundLayerMask
        );

        if (hit.collider != null)
        {
            DistanceToGround = hit.distance;
            IsGrounded = hit.distance <= _groundDistanceTolerance;
        }
        else
        {
            DistanceToGround = null;
            IsGrounded = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_capsuleCollider2D == null) return;

        float radius = (_capsuleCollider2D.size.x * 0.5f) - 0.05f;
        Vector2 origin = (Vector2)transform.position -
                         new Vector2(0, _capsuleCollider2D.size.y * 0.5f - radius);

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(origin, radius);
    }
}
