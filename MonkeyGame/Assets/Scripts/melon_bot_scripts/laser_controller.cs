using UnityEngine;

public class laser_controller : MonoBehaviour
{
    public float maxLength = 10f;
    public LayerMask targetLayer;
    private LineRenderer lr;


    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (!lr) lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = 0.025f;
        lr.endWidth = 0.025f;
        lr.useWorldSpace = true;

        // Make the laser red
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        // Assign a simple material (needed for LineRenderer)
        lr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        Vector2 origin = transform.position;
        Vector2 direction = -transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxLength, targetLayer);
        Vector2 endPoint = hit.collider != null ? hit.point : origin + direction * maxLength;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, endPoint);

        // Optional: visualize the ray in Scene view
        //Debug.DrawRay(origin, direction * maxLength, Color.red);
    }
}
