using UnityEngine;

public class scanner_script : MonoBehaviour
{
    private float endSizeLaser;
    private float baseSizeLaser;
    public Color laserColor = Color.red;
    public LayerMask targetLayer;
    private float maxLength;
    private LineRenderer lr;
    private BananaBotMovements parentScript;

    void Awake()
    {
        parentScript = GetComponentInParent<BananaBotMovements>();

        if (parentScript != null)
        {
            maxLength = parentScript.viewDistance;
            endSizeLaser = parentScript.endSizeLaser;
            baseSizeLaser = parentScript.baseSizeLaser;
            laserColor = parentScript.laserColor;

        }
    }

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (!lr) lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.startWidth = baseSizeLaser;
        lr.endWidth = endSizeLaser;
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
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxLength, targetLayer);
        Vector2 endPoint = hit.collider != null ? hit.point : origin + direction * maxLength;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, endPoint);

        // Optional: visualize the ray in Scene view
        //Debug.DrawRay(origin, direction * maxLength, Color.red);
    }
    public void SetLaserColor(Color c)
    {
        lr.startColor = c;
        lr.endColor = c;
    }
    public void SetLaserSize(float s)
    {
        lr.endWidth = s;
    }
    public void ResetLaserSize()
    {
        lr.startWidth = baseSizeLaser;
        lr.endWidth = endSizeLaser;
    }
}
