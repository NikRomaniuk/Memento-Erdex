using UnityEngine;

[DisallowMultipleComponent]
public class SpectatorMovement : MonoBehaviour
{
    // --- References ---
    [SerializeField] private SpectatorManager _spectatorManager;

    // --- Debug ---
    [SerializeField] private bool _debug = false;

    // --- Cache ---
    private string _lastDebug;
    private Rigidbody2D _rb;

    /// <summary>
    /// Caches required components
    /// </summary>
    private void Awake()
    {
        if (_spectatorManager == null)
        {
            Debug.LogError("[SpectatorMovement] SpectatorManager reference is not assigned!", this);
            return;
        }

        _rb = _spectatorManager.RB;
    }

    /// <summary>
    /// Sets full velocity directly
    /// </summary>
    public void SetMovement(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Returns current velocity
    /// </summary>
    public Vector2 GetMovement()
    {
        return _rb.linearVelocity;
    }

    /// <summary>
    /// Returns true if velocity magnitude is above threshold
    /// </summary>
    public bool IsMoving()
    {
        return _rb.linearVelocity.sqrMagnitude > 0.0001f;
    }

    /// <summary>
    /// Stops all movement instantly
    /// </summary>
    public void StopMovement()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Accelerates movement towards target velocity gradually
    /// </summary>
    public void AccelerateMovement(float acceleration, Vector2 targetVelocity)
    {
        Vector2 current = _rb.linearVelocity;
        Vector2 newVelocity = Vector2.MoveTowards(current, targetVelocity, acceleration * Time.fixedDeltaTime);
        _rb.linearVelocity = newVelocity;
    }

    /// <summary>
    /// Decelerates movement towards zero gradually
    /// </summary>
    public void DecelerateMovement(float deceleration)
    {
        Vector2 current = _rb.linearVelocity;
        Vector2 newVelocity = Vector2.MoveTowards(current, Vector2.zero, deceleration * Time.fixedDeltaTime);
        _rb.linearVelocity = newVelocity;
    }

    // =============
    // HELPER METHODS
    // =============

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[SpectatorMovement] {message}", this);
    }
}
