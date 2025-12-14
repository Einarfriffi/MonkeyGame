using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D targetRb;

    [Header("Limits")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY = 8f;

    [Header("Vertical Dead Zone")]
    [SerializeField] private float verticalDeadZone = 1f;

    [Header("Y Smoothing")]
    [Tooltip("Slower = smoother upward movement")]
    [SerializeField] private float upwardSmoothTime = 0.2f;

    [Tooltip("Faster = camera catches falling quickly")]
    [SerializeField] private float downwardSmoothTime = 0.05f;

    private Vector3 _offset;
    private float _yVelocity;

    void Start()
    {
        _offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 desired = target.position + _offset;

        // Snap X
        desired.x = Mathf.Clamp(desired.x, minX, maxX);

        // Vertical dead zone
        float deltaY = desired.y - transform.position.y;
        if (Mathf.Abs(deltaY) < verticalDeadZone)
        {
            desired.y = transform.position.y;
            _yVelocity = 0f;
        }

        desired.y = Mathf.Max(desired.y, minY);

        // Choose smooth time based on vertical velocity
        float smoothTime =
            targetRb.linearVelocity.y < 0f ? downwardSmoothTime : upwardSmoothTime;

        float smoothY = Mathf.SmoothDamp(
            transform.position.y,
            desired.y,
            ref _yVelocity,
            smoothTime
        );

        transform.position = new Vector3(
            desired.x,
            smoothY,
            transform.position.z
        );
    }
}



// OLD Camera code
/* using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Horizontal limits")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Vertical settings")]
    [SerializeField] private float verticalDeadZone = 1f;   // how far the monkey can move vertically before camera reacts

    [Header("Vertical limits")]
    [SerializeField] private float minY = 8f;


    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.2f;       // higher = slower camera

    private Vector3 _offset;
    private Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        if (target != null)
        {
            _offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Where the camera *would* like to be, based purely on offset
        Vector3 desired = target.position + _offset;

        // Clamp X
        desired.x = Mathf.Clamp(desired.x, minX, maxX);

        // --- Vertical dead zone logic ---
        float currentY = transform.position.y;
        float desiredY = desired.y;
        float deltaY = desiredY - currentY;

        // If the target is still within the dead zone vertically, don't move the camera on Y
        if (Mathf.Abs(deltaY) < verticalDeadZone)
        {
            desired.y = currentY; // keep current camera height
        }

        desired.y = Mathf.Max(desired.y, minY);

        // Smooth movement to the desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref _velocity,
            smoothTime
        );
    }
} */
