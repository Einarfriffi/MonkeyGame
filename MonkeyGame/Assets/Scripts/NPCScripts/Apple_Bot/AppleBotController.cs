using Unity.VisualScripting;
using UnityEngine;

public class AppleBotController : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] public float viewDistance; // view distance of bot
    [SerializeField] private float maxAngle = 80f;

    // =========================================
    [Header("Scan Settings")]
    [SerializeField] private float scanSpeed = 40f;
    [SerializeField] private float rotationLerpSpeed = 6f;
    [SerializeField] private float scanResumeDelay = 0.75f;
    [SerializeField] private Transform rayStart;
    [SerializeField] private LayerMask targetLayer;
    // scanner variables
    private float currentScanAngle = 0f;
    private float targetAngle = 0f;
    private int scanDirection = 1; // 1 = right, -1 = left
    private float lostPlayerTime = 0f;
    private bool wasSeeingPlayer = false;

    // =========================================
    [Header("Fix Settings")]
    [SerializeField] private float offSettPlayerTransForm = 1.5f;



    // internal variables
    // player variables
    private Transform player;

    // state variables
    private bool seePlayer = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
    }
    private void FixedUpdate()
    {
        PlayerDetection();
        if (seePlayer) // if bot sees player
        {
            lostPlayerTime = 0f;
            wasSeeingPlayer = true;
        }
        else
        {
            if (wasSeeingPlayer) // if bot had seen player
            {
                lostPlayerTime += Time.fixedDeltaTime;
                if (lostPlayerTime >= scanResumeDelay)
                {
                    wasSeeingPlayer = false;
                }
            }
            else // normal functionality
            {
                Scan();
            }
        }

        // rotate bot
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.Euler(0f, 0f, targetAngle),
            rotationLerpSpeed * Time.fixedDeltaTime
        );
    }
    private void PlayerDetection()
    {
        // if player is in view range of bot
        Vector3 playertemp = GetPlayerPos();

        float dist = Vector3.Distance(transform.position, playertemp);
        if (dist <= viewDistance)
        {
            float angle = AngleBetween();
            if (Mathf.Abs(angle) <= maxAngle)
            {
                if (LineOfSight())
                {
                    seePlayer = true;
                    targetAngle = angle;
                    currentScanAngle = angle; // sync scan start
                    return;
                }
            }
        }
        seePlayer = false;
    }
    // OLD CODE
    /* private void PlayerDetection()
    {
        // if player is in view range of bot
        Vector3 playertemp = GetPlayerPos();

        float dist = Vector3.Distance(transform.position, playertemp);
        if (dist <= viewDist)
        {
            float angle = AngleBetween();
            if (Mathf.Abs(angle) <= maxAngle)
            {
                seePlayer = true;
                float yRot = transform.localEulerAngles.y;
                transform.localRotation = Quaternion.Euler(0f, yRot, angle);
                return;
            }
        }
        seePlayer = false;
    } */

    private void Scan()
    {
        currentScanAngle += scanDirection * scanSpeed * Time.fixedDeltaTime;

        if (currentScanAngle >= maxAngle)
        {
            currentScanAngle = maxAngle;
            scanDirection = -1;
        }
        else if (currentScanAngle <= -maxAngle)
        {
            currentScanAngle = -maxAngle;
            scanDirection = 1;
        }
        targetAngle = currentScanAngle;
        //transform.localRotation = Quaternion.Euler(0f, 0f, currentScanAngle);
    }

    private float AngleBetween()
    {
        float angle;
        Vector2 direction_temp = player.position - transform.position;
        angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;
        //angle *= -1;
        return angle + 90f;
    }

    private bool LineOfSight()
    {
        //see if there is a line of sight between bot and player
        Debug.Log("meow");
        Vector2 origin = rayStart.position;
        Vector2 target = player.position;

        Vector2 direction = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewDistance, targetLayer);
        Debug.DrawLine(origin, target, Color.red);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }
    
    private Vector3 GetPlayerPos()
    {
        Vector3 playertemp = new Vector3(player.position.x, player.position.y + offSettPlayerTransForm, player.position.y);
        return playertemp;
    }

}
