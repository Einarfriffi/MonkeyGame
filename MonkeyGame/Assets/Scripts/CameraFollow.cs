using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

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

        transform.position = target.position + _offset;
    }
}
