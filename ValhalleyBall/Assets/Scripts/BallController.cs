using UnityEngine;

public class BallController : MonoBehaviour
{
    // Reference to the Rigidbody component for physics interactions
    private Rigidbody rb;

    // These values control how strong the bump is.
    // You can fine-tune them from the Unity Inspector.
    [Header("Bump Settings")]
    public float bumpForce = 8f;         // Forward force
    public float bumpLift = 12f;         // Upward force
    
    [Header("Custom Gravity")]
    public float gravityUp = -2f;     // Gravity when ball is moving up
    public float gravityDown = -30f;  // Gravity when ball is falling
    
    void Awake()
    {
        // This gets the Rigidbody component on the ball
        // It's called once when the object is created or enabled
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // Check current vertical motion
        if (rb.linearVelocity.y > 0)
        {
            // Going up — gentle gravity for hangtime
            rb.AddForce(Vector3.up * gravityUp, ForceMode.Acceleration);
        }
        else
        {
            // Falling — strong gravity for snap fall
            rb.AddForce(Vector3.up * gravityDown, ForceMode.Acceleration);
        }
    }
    /// <summary>
    /// Call this method to apply an anime-style bump to the ball.
    /// 'direction' is usually based on player position or forward vector.
    /// </summary>
    /// <param name="direction">A normalized Vector3 showing where the ball should go</param>
    public void Bump(Vector3 direction)
    {
        // Reset current velocity so the ball doesn't carry old movement
        rb.linearVelocity = Vector3.zero;

        // Create a combined force: forward * bumpForce + upward * bumpLift
        Vector3 bumpVector = direction * bumpForce + Vector3.up * bumpLift;

        // Apply the force as an impulse (instant push)
        rb.AddForce(bumpVector, ForceMode.Impulse);
    }
}
