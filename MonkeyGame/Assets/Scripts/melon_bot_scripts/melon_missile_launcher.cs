using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class melon_missile_launcher : MonoBehaviour
{
    //public GameObject target;
    public Transform target;

    public float fix_angle;

    // how far the bot can see
    public float view_dist;

    public GameObject laser;

    public float rotation_speed;

    public float random_missile_angle = 3f;

    private Quaternion originalRotation;

    // missile shit
    public GameObject missile_prefab;
    public Transform firePoint;
    public Transform firePoint_2;
    public float between_missile_time;
    private float missile_time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalRotation = transform.rotation;
        missile_time = between_missile_time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        if (dist <= view_dist)
        {
            // make laser appear
            laser.SetActive(true);
            // find the direction between them
            Vector2 direction = target.position - transform.position;
            // find the angle between the objects
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // fix angle so it points towards the player
            angle += fix_angle;
            // apply the rotation
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // missile shit logic
            missile_time -= Time.deltaTime;
            if (missile_time <= 0)
            {
                // Random rotation in degrees
                float randomAngle1 = UnityEngine.Random.Range(-random_missile_angle, random_missile_angle);
                float randomAngle2 = UnityEngine.Random.Range(-random_missile_angle, random_missile_angle);

                // Apply random rotation around Z-axis (for 2D)
                Quaternion rot1 = firePoint.rotation * Quaternion.Euler(0f, 0f, randomAngle1);
                Quaternion rot2 = firePoint.rotation * Quaternion.Euler(0f, 0f, randomAngle2);

                Instantiate(missile_prefab, firePoint.position, rot1);
                Instantiate(missile_prefab, firePoint_2.position, rot2);

                missile_time = between_missile_time;
            }

        }
        else
        {
            // make laser disappear
            laser.SetActive(false);
            missile_time = between_missile_time;
            //if(transform.rotation != quaternion())
            if (transform.rotation != originalRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotation_speed);
                // transform.rotation = Quaternion.Euler(0f, 0f, rotation_speed);
            }
        }
    }
}
