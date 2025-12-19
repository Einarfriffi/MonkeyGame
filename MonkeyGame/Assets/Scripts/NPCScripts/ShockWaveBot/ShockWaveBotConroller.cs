using LootLocker.Extension.DataTypes;
using TreeEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShockWaveBotConroller : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] private float viewDistance;
    [SerializeField] private float scanAngle = 45;
    [SerializeField] private float attackTime = 1.5f;
    [SerializeField] private float timeBeforeAttack = 0.5f;
    [SerializeField] private float graceSpotTime = 0.1f;


    // =========================================
    [Header("Scan Settings")]
    [SerializeField] private Transform rayStart; // pretty much the eyes
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float scanOffset = 7.5f;

    // =========================================
    [Header("Light Settings")]
    [SerializeField] Light2D scannerLight;
    [SerializeField] Color baseColor;
    [SerializeField] Color spotColor;
    [SerializeField] Color angryColor;

    // =========================================
    [Header("Shock Wave Settings")]
    [SerializeField] private float WaveSpeed;
    [SerializeField] private float acceleration = 5f;   // how fast it speeds up
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float maxRange = 10f;
    [SerializeField] private float coolDownShock = 1f;
    [SerializeField] private float launchForce;
    [SerializeField] private GameObject PrefabShockwave;
    [SerializeField] private Transform LaunchPoint;

    // =========================================
    [Header("Fix Settings")]
    [SerializeField] private float offSettPlayerTransForm = 1.5f;


    // =========================================
    // internal variables
    private Transform player;
    private Vector3 curPlayerPos;
    private bool seePlayer = false;
    public int direction = -1;
    private float shockCoolDownTimer = 0;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;

        // find the bots rotation
        float yRot = transform.eulerAngles.y;
        if (Mathf.Abs(yRot - 180f) < 1f)
            direction = 1;   // facing right
        else
            direction = -1;  // facing left
    }

    private void FixedUpdate()
    {
        GetPlayerPos();
        PlayerDetection();
        if (seePlayer)
        {
            if (shockCoolDownTimer >= coolDownShock)
            {
                shockCoolDownTimer = 0;
                ShockWaveAttack();
            }
            else
            {
                shockCoolDownTimer += Time.deltaTime;
            }

        }
        else
        {
            shockCoolDownTimer = 0;
        }

    }

    private void PlayerDetection()
    {
        // if player is in view range of bot
        Vector3 playertemp = curPlayerPos;

        float dist = Vector3.Distance(rayStart.position, playertemp);
        if (dist <= viewDistance)
        {
            float angle = AngleBetween();
            //max_cur = scanAngle + Mathf.Abs(currentScanAngle);
            if (angle <= scanOffset && angle >= -scanAngle * 2)
            {
                //Debug.Log(angle);
                if (LineOfSight())
                {
                    seePlayer = true;
                    return;
                }
            }
        }
        seePlayer = false;
    }

    private float AngleBetween()
    {
        float angle;
        if (direction == -1)
        {
            Vector2 dir = new Vector2(-(curPlayerPos.x - rayStart.position.x),
                           curPlayerPos.y - rayStart.position.y);

            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        else
        {
            //Vector2 direction_temp = Player.position - transform.position;
            Vector2 direction_temp = curPlayerPos - rayStart.position;
            angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;
        }
        //Debug.Log(angle);
        return angle;
    }

    private bool LineOfSight()
    {
        //see if there is a line of sight between bot and player
        Vector2 origin = rayStart.position;

        Vector2 target1 = curPlayerPos;

        Vector2 directionTop = (target1 - origin).normalized;

        Debug.DrawLine(origin, target1, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(origin, directionTop, viewDistance, targetLayer);
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

    private void ShockWaveAttack()
    {
        // create shock wave and the init new attributes for missile
        Quaternion rot1 = LaunchPoint.rotation * Quaternion.Euler(0f, 0f, 0f);
        GameObject m1 = Instantiate(PrefabShockwave, LaunchPoint.position, rot1);
        m1.GetComponent<ShcokWaveController>().Init(acceleration, maxSpeed, maxRange, direction, launchForce);
    }
}
