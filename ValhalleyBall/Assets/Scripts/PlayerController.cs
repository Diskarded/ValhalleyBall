using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private const string BallHitDebugMessage = "Ball hit triggered!";

    // --- Movement Variables ---
    public float moveSpeed = 5f;          // Player movement speed
    public float deceleration = 20f;      // Rate at which player slows down when no input is given
    public float jumpForce = 8f;          // Force applied when the player jumps
    public float fallMultiplier = 3f;     // Gravity multiplier for faster falling
    public float lowJumpMultiplier = 2f;  // Gravity multiplier for shorter jumps


// --- Ball Hit Variables ---
public float hitForce = 10f;          // Force applied to the ball when hit
    private bool hitKeyPressed = false;   // Flag to track if the player has pressed F

    private Rigidbody rb;                 // Cached Rigidbody component
    private bool isGrounded = false;      // Whether the player is on the ground

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- Handle Player Movement ---
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrow
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down arrow

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

        // --- Handle Jumping ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // --- Apply Better Gravity ---
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

        // --- Detect F Key for Ball Hit ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            hitKeyPressed = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Check if the collider is tagged as the Ball and if the player pressed F
        if (other.CompareTag("Ball") && hitKeyPressed)
        {
            Debug.Log("Ball hit triggered!");

            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 hitDirection = ((other.transform.position - transform.position).normalized + Vector3.up * 0.5f).normalized;
                ballRb.AddForce(hitDirection * hitForce, ForceMode.Impulse);
            }

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

            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 hitDirection = ((other.transform.position - transform.position).normalized + Vector3.up * 0.5f).normalized;
                ballRb.AddForce(hitDirection * hitForce, ForceMode.Impulse);
            }

            hitKeyPressed = false;
        }
    }

}
