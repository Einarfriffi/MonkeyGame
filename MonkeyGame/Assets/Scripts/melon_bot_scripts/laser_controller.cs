using UnityEngine;

public class laser_controller : MonoBehaviour
{
    private float laserSize;
    private Color laserColor = Color.red;
    public LayerMask targetLayer;
    private float maxLength;
    private LineRenderer lr;
    private melon_missile_launcher parentScript;

    void Awake()
    {
        parentScript = GetComponentInParent<melon_missile_launcher>();

        if (parentScript != null)
        {
            maxLength = parentScript.view_dist;
            laserSize = parentScript.laserSize;
            laserColor = parentScript.laserColor;

        }
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (!lr) lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = laserSize;
        lr.endWidth = laserSize;
        lr.useWorldSpace = true;

        // Make the laser red
        lr.startColor = laserColor;
        lr.endColor = laserColor;

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
    }
}
