using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Player")]
    public float moveSpeed = 10f;
    public float ascendSpeed = 6f;
    public float rotationSpeed = 80f;   // New rotation speed
    private Rigidbody rb;

    public GameObject missilePrefab;
    public Transform firePoint;
    public float shootForce = 500f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");   // A (-1), D (+1)
        float vertical = Input.GetAxis("Vertical");       // W (+1), S (-1)

        // ✅ ROTATE instead of sliding left/right
        if (horizontal != 0)
        {
            transform.Rotate(0f, horizontal * rotationSpeed * Time.deltaTime, 0f);
        }

        // ✅ Move only forward/backward (no more strafe)
        if (vertical != 0)
        {
            Vector3 moveDir = transform.forward * vertical;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
        }

        // Ascend / Descend
        if (Input.GetKey(KeyCode.Q))
        {
            rb.MovePosition(rb.position + Vector3.up * ascendSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            rb.MovePosition(rb.position + Vector3.down * ascendSpeed * Time.deltaTime);
        }

        // Shooting
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            ShootMissile();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void ShootMissile()
    {
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody mrb = missile.GetComponent<Rigidbody>();
        mrb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);

        Destroy(missile, 5f);
    }
}
