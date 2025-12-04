using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] float minX;
    [SerializeField] float maxX;

    private Vector3 _offset;

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

        Vector3 desired = target.position + _offset;

        transform.position = target.position + _offset;
        desired.x = Mathf.Clamp(desired.x, minX, maxX);
        transform.position = desired;
    }
}
