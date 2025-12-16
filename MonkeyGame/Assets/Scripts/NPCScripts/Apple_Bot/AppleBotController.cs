using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AppleBotController : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] public float viewDistance; // view distance of bot
    [SerializeField] private float maxAngle = 80f;
    [SerializeField] private float scanAngle = 45;
    [SerializeField] private float attackTime = 1.5f;
    [SerializeField] private float timeBeforeAttack = 0.5f;
    [SerializeField] private float graceSpotTime = 0.1f;
    private float curAttackTime = 0;
    private float curSpotTime;
    private bool isAttacking = false;
    private bool isSpotted = false;


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
    private float max_cur;
    private float min_cur;

    // =========================================
    [Header("Light Stuff")]
    [SerializeField] Light2D scannerLight;
    [SerializeField] Color baseColor;
    [SerializeField] Color spotColor;
    [SerializeField] Color angryColor;
    [SerializeField] float beamMaxSize;
    [SerializeField] float beamMinSize;
    private float lightGrowRate;
    [SerializeField] private float fallOffGrowRate = 0.02f;
    [SerializeField] private float intensityGrowRate = 0.02f;

    // =========================================
    [Header("Fix Settings")]
    [SerializeField] private float offSettPlayerTransForm = 1.5f;



    // internal variables
    // player variables
    private Transform player;

    // state variables
    private bool seePlayer = false;

    private Vector3 curPlayerPos;


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;

        // Light Pre sets
        // set laser size to scan angle
        scannerLight.pointLightOuterAngle = scanAngle * 1.5f;
        scannerLight.pointLightInnerAngle = scanAngle / 2;
        lightGrowRate = timeBeforeAttack - graceSpotTime / 100;
    }
    private void FixedUpdate()
    {
        GetPlayerPos();
        PlayerDetection();
        if (isSpotted)
        {
            curSpotTime += Time.fixedDeltaTime;
            scannerLight.color = spotColor;
            if(curSpotTime <= graceSpotTime && !seePlayer)
            {
                isSpotted = false;
                curSpotTime = 0;
                //return;
            }
            else
            {
                if(scannerLight.pointLightOuterAngle >= beamMinSize)
                {
                    scannerLight.pointLightOuterAngle -= lightGrowRate;
                    scannerLight.pointLightInnerAngle -= lightGrowRate;
                }
                if(scannerLight.falloffIntensity < 1)
                {
                    scannerLight.falloffIntensity += fallOffGrowRate;
                }
                scannerLight.intensity += intensityGrowRate;
            }
            if (curSpotTime >= timeBeforeAttack)
            {
                isAttacking = true;
                isSpotted = false;
                curSpotTime = 0;
                //return;
            }
        }
        else if (isAttacking)
        {
            scannerLight.color = angryColor;
            curAttackTime += Time.deltaTime;
            if (curAttackTime >= attackTime)
            {
                curAttackTime = 0f;
                isAttacking = false;
                scannerLight.intensity = 1;
            }
            //return;
        }
        else if (seePlayer) // if bot sees player
        {
            // new shit
            //curAttackTime = timeBeforeAttack;
            isSpotted = true;
            lostPlayerTime = 0f;
            wasSeeingPlayer = true;
        }
        else
        {
            // light stuff
            if(scannerLight.pointLightOuterAngle <= beamMaxSize)
            {
                scannerLight.pointLightOuterAngle += lightGrowRate;
                scannerLight.pointLightInnerAngle += lightGrowRate;
            }
            if(scannerLight.falloffIntensity > 0.5f)
            {
                scannerLight.falloffIntensity -= fallOffGrowRate;
            }
        
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
                scannerLight.color = baseColor;
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
        Vector3 playertemp = curPlayerPos;

        float dist = Vector3.Distance(transform.position, playertemp);
        if (dist <= viewDistance)
        {
            float angle = AngleBetween();
            //max_cur = scanAngle + Mathf.Abs(currentScanAngle);
            //Debug.Log(angle);
            //if (Mathf.Abs(angle) <= scanAngle)
            min_cur = currentScanAngle - scanAngle/2;
            max_cur = currentScanAngle + scanAngle/2;
            if(angle <= currentScanAngle + scanAngle/2 && angle >= currentScanAngle - scanAngle/2)
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
        scannerLight.color = baseColor;
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
        Vector2 direction_temp = curPlayerPos - transform.position;
        angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;
        //angle *= -1;
        return angle + 90f;
    }

    private bool LineOfSight()
    {
        //see if there is a line of sight between bot and player
        Vector2 origin = rayStart.position;

        Vector2 target = curPlayerPos;

        Vector2 direction = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewDistance, targetLayer);
        Debug.DrawLine(origin, target, Color.red);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }
    
    private void GetPlayerPos()
    {
        // raises the transform of the player so the bot does not aim at its feet
        Vector3 playertemp = new Vector3(player.position.x, player.position.y + offSettPlayerTransForm, player.position.z);
        //Debug.DrawLine(player.position ,playertemp, Color.pink, 0.1f);
        curPlayerPos = playertemp;
    }

}
