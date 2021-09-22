using UnityEngine;

public class CamMouseOrbit : MonoBehaviour
{
    private float x = 0.0f;
    private float y = 0.0f;
    private float dist;

    public Transform target;
    public float distance = 10.0f;
    public float xSpeed = 5.0f;
    public float ySpeed = 2.5f;
    public float distSpeed = 10.0f;
    public float yMinLimit = -20.0f;
    public float yMaxLimit = 80.0f;
    public float distMinLimit = 5.0f;
    public float distMaxLimit = 50.0f;
    public float orbitDamping = 4.0f;
    public float distDamping = 4.0f;

    private void Awake()
    {
        dist = distance;
    }

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    private void LateUpdate()
    {
        if (!target) return;

        x += Input.GetAxis("Mouse X") * xSpeed;
        y -= Input.GetAxis("Mouse Y") * ySpeed;
        distance -= Input.GetAxis("Mouse ScrollWheel") * distSpeed;

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        distance = Mathf.Clamp(distance, distMinLimit, distMaxLimit);
        dist = Mathf.Lerp(dist, distance, distDamping * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitDamping);
        transform.position = transform.rotation * new Vector3(0.0f, 0.0f, -dist) + target.position;
    }

    private float ClampAngle(float a, float min, float max)
    {
        while (max < min) max += 360.0f;
        while (a > max) a -= 360.0f;
        while (a < min) a += 360.0f;

        if (a > max)
        {
            if (a - (max + min) * 0.5 < 180.0)
                return max;
            else
                return min;
        }
        else
            return a;
    }
}