using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class melon_missile_launcher : MonoBehaviour
{
    [Header("Core Settings")]
    [SerializeField] public float view_dist; // view distance of bot
    [SerializeField] private float blinkSpeed = 20f; // higher = faster blinking
    [SerializeField] private float rotation_speed; // how fast bot launchers rotate
    //[SerializeField] private float detection_angle_up = 80f; // how far he can detect player up
    //[SerializeField] private float detection_angle_down = 80f; // how far he can detect player down
    [SerializeField] private float viewAngle = 80f; // how far he can detect player down
    [SerializeField] private float bob_range = 0.3f; // smaller for subtle bob
    [SerializeField] private float bob_speed = 1f;   // how fast it bobs

    // missile shit
    [Header("Missile Settings")]
    [SerializeField] private float randAngleMissile = 3f;
    [SerializeField] private float betweenTimeMissile = 2f;
    [SerializeField] private float acceleration = 5f; // how fast it speeds up
    [SerializeField] private float maxSpeed = 20f; // max speed of missile
    [SerializeField] private float maxRange = 10f; // max range of missile

    [Header("Laser Settings")]
    [SerializeField] public float laserSize = 0.025f;
    [SerializeField] public Color laserColor;
    [SerializeField] private float laserTimeOff = 1f;

    [Header("Core Components")]
    [SerializeField] private GameObject laser;
    //[SerializeField] private Transform player;
    [SerializeField] private Transform melon_whole_trans;

    [Header("Missile Components")]
    [SerializeField] private GameObject missile_prefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform firePoint_2;
    //[SerializeField] private Transform trans_launch_1;
    //[SerializeField] private Transform trans_launch_2;

    [Header("Launcher Animators")]
    [SerializeField] private Animator front_launcher_animator;
    [SerializeField] private Animator back_launcher_animator;

    private float missile_time;
    //private float fix_angle = 180f;
    private Quaternion originalRotation;
    private Vector3 melonStartLocalPos;
    private float laser_off_time = 0f;
    public int direction = -1;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;

        // have correct direction -1 = left / 1 = right
        float yRot = transform.eulerAngles.y;
        if (Mathf.Abs(yRot - 180f) < 1f)
            direction = 1;   // facing right
        else
            direction = -1;  // facing left

        originalRotation = melon_whole_trans.rotation;

        missile_time = betweenTimeMissile;
        if (melon_whole_trans != null)
            melonStartLocalPos = melon_whole_trans.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (melon_whole_trans != null)
        {
            // Smooth sine wave bobbing
            float newY = melonStartLocalPos.y + Mathf.Sin(Time.time * bob_speed) * bob_range;
            melon_whole_trans.localPosition = new Vector3(
                melonStartLocalPos.x,
                newY,
                melonStartLocalPos.z
            );
        }
        LauncherLogic();
    }

    private void LauncherLogic()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist > view_dist)
        {
            RotateLauncherback();
            laser.SetActive(false);
            missile_time = betweenTimeMissile;
            laser_off_time = 0;
            return;
        }
        float angle = AngleBetween();
        if (Mathf.Abs(angle) > viewAngle)
        {
            laser_off_time -= Time.deltaTime;
            return;
        }
        if (PlayerInView())
        {
            //Debug.DrawLine(laser.transform.position, player.position, Color.red);
            laser_blink();
            // apply the rotation
            float yRot = transform.localEulerAngles.y;
            transform.localRotation = Quaternion.Euler(0f, yRot, angle);

            // missile logic
            missile_time -= Time.deltaTime;
            if (missile_time <= 0)
            {
                launch_missiles();
            }
        }
        else
        {
            laser_off_time = 0;
        }


        // OLD
        /*
        float dist = Vector3.Distance(player.position, transform.position);
        //float angle = AngleBetween();
        if (dist <= view_dist)
        {
            float angle = AngleBetween();
            //if (angle < detection_angle_up && angle > detection_angle_down)
            if (Mathf.Abs(angle) < viewAngle)
            {
                if (PlayerInView())
                {
                    //Debug.DrawLine(laser.transform.position, player.position, Color.red);
                    laser_blink();
                    // apply the rotation
                    float yRot = transform.localEulerAngles.y;
                    transform.localRotation = Quaternion.Euler(0f, yRot, angle);

                    // missile logic
                    missile_time -= Time.deltaTime;
                    if (missile_time <= 0)
                    {
                        launch_missiles();
                    }
                }
            }
            laser_off_time -= Time.deltaTime;
            RotateLauncherback();
            return;
        }
        // make laser disappear
        laser.SetActive(false);
        missile_time = betweenTimeMissile;
        RotateLauncherback();
        laser_off_time = 0; */
    }

    private void launch_missiles()
    {
        laser_off_time = laserTimeOff;

        // Random rotation in degrees
        float randomAngle1 = UnityEngine.Random.Range(-randAngleMissile, randAngleMissile);
        float randomAngle2 = UnityEngine.Random.Range(-randAngleMissile, randAngleMissile);

        // Apply random rotation around Z-axis (for 2D)
        Quaternion rot1 = firePoint.rotation * Quaternion.Euler(0f, 0f, randomAngle1);
        Quaternion rot2 = firePoint.rotation * Quaternion.Euler(0f, 0f, randomAngle2);

        // play the launch animation
        front_launcher_animator.SetTrigger("shoot");
        front_launcher_animator.SetTrigger("shoot");

        // create missile and the init new attributes for missile
        GameObject m1 = Instantiate(missile_prefab, firePoint.position, rot1);
        m1.GetComponent<missile_controller>().Init(acceleration, maxSpeed, maxRange);

        GameObject m2 = Instantiate(missile_prefab, firePoint_2.position, rot2);
        m2.GetComponent<missile_controller>().Init(acceleration, maxSpeed, maxRange);

        // reset missile timer
        missile_time = betweenTimeMissile;
    }
    private void laser_blink()
    {
        if (laser_off_time <= 0)
        {
            // Laser BLINKING when close to firing
            if (missile_time <= 0.5f)
            {
                bool blinkState = Mathf.FloorToInt(Time.time * blinkSpeed) % 2 == 0;
                laser.SetActive(blinkState);
            }
            else
            {
                // Laser stays ON when aiming but not yet blinking
                laser.SetActive(true);
            }
        }
    }
    private float AngleBetween()
    {
        float angle = 0;
        if (direction == -1)
        {
            Vector2 dir = new Vector2(-(player.position.x - transform.position.x),
                           player.position.y - transform.position.y);

            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle *= -1;
        }
        else
        {
            Vector2 direction_temp = player.position - transform.position;
            angle = Mathf.Atan2(direction_temp.y, direction_temp.x) * Mathf.Rad2Deg;
            angle *= -1;
        }
        return angle;

        /* // find the direction between them
        Vector2 direction = player.position - transform.position;
        // find the angle between the objects
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // fix angle so it points towards the player
        angle += fix_angle;

        return angle; */
    }
    private bool PlayerInView()
    {
        //see if there is a line of sight between bot and player

        Vector2 origin = laser.transform.position;
        Vector2 target = player.position;

        Vector2 direction = (target - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, view_dist);
        //Debug.DrawLine(origin, target, Color.red);
        if (hit.collider.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }

    void RotateLauncherback()
    {
        if (transform.rotation != originalRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotation_speed);
        }
    }

}
