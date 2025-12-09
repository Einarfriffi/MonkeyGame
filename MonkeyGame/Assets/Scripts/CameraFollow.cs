using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Horizontal limits")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Header("Vertical settings")]
    [SerializeField] private float verticalDeadZone = 1f;   // how far the monkey can move vertically before camera reacts

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

        // Smooth movement to the desired position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref _velocity,
            smoothTime
        );
    }
}
