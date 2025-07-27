using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private const string BallHitDebugMessage = "Ball hit triggered!";

    // --- Movement Variables ---
    public float moveSpeed = 5f;
    public float deceleration = 20f;
    public float jumpForce = 8f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    // --- Ball Hit Variables ---
    public float hitForce = 10f;
    private bool hitKeyPressed = false;

    // --- Smart Bump Variables ---
    [Header("Ball Bump Setup")]
    public BallController ballController;
    public Transform ballTransform;
    public Transform netTransform;

    [Header("Bump Override Settings")]
    public float bumpOverrideAngle = 100f;

    private Rigidbody rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveInput = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 currentVelocity = rb.linearVelocity;

        if (moveInput != Vector3.zero)
        {
            Vector3 targetVelocity = moveInput * moveSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);
        }
        else
        {
            Vector3 flatVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

            if (flatVelocity.magnitude > 0.01f)
            {
                Vector3 decel = flatVelocity.normalized * deceleration * Time.deltaTime;

                if (decel.sqrMagnitude > flatVelocity.sqrMagnitude)
                    flatVelocity = Vector3.zero;
                else
                    flatVelocity -= decel;

                rb.linearVelocity = new Vector3(flatVelocity.x, currentVelocity.y, flatVelocity.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (!isGrounded)
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1), ForceMode.Acceleration);
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1), ForceMode.Acceleration);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            hitKeyPressed = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball") && hitKeyPressed)
        {
            Debug.Log(BallHitDebugMessage);

            Vector3 toBall = (ballTransform.position - transform.position).normalized;
            Vector3 toNet = (netTransform.position - transform.position).normalized;
            Vector3 playerForward = transform.forward;

            float angleToBall = Vector3.Angle(playerForward, toBall);
            float angleAwayFromNet = Vector3.Angle(playerForward, -toNet);

            Vector3 bumpDirection = toBall;

            if (angleToBall < 45f && angleAwayFromNet > bumpOverrideAngle)
            {
                bumpDirection = toNet;
            }

            ballController.Bump(bumpDirection);
            hitKeyPressed = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    public void HandleHitTrigger(Collider other)
    {
        if (other.CompareTag("Ball") && hitKeyPressed)
        {
            Debug.Log(BallHitDebugMessage);

            Vector3 toBall = (ballTransform.position - transform.position).normalized;
            Vector3 toNet = (netTransform.position - transform.position).normalized;
            Vector3 playerForward = transform.forward;

            float angleToBall = Vector3.Angle(playerForward, toBall);
            float angleAwayFromNet = Vector3.Angle(playerForward, -toNet);

            Vector3 bumpDirection = toBall;

            if (angleToBall < 45f && angleAwayFromNet > bumpOverrideAngle)
            {
                bumpDirection = toNet;
            }

            ballController.Bump(bumpDirection);
            hitKeyPressed = false;
        }
    }
}