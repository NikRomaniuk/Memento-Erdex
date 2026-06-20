using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class CharacterMovement : MonoBehaviour
{
    // --- References ---
    [SerializeField] private CharacterManager _characterManager;

    // --- Debug ---
    [SerializeField] private bool _drawDebug = true;
    [SerializeField] private bool _debug = false;

    // --- Cache ---
    private string _lastDebug;
    private Rigidbody2D _rb;

    // --- Public Accessors ---

    /// <summary>
    /// Caches required components
    /// </summary>
    private void Awake()
    {
        if (_characterManager == null)
        {
            Debug.LogError("[CharacterMovement] CharacterManager reference is not assigned!", this);
            return;
        }

        _rb = _characterManager.RB;
    }

    /// <summary>
    /// Sets horizontal velocity
    /// </summary>
    public void SetHorizontalSpeed(float speed)
    {
        Vector2 velocity = _rb.linearVelocity;
        velocity.x = speed;
        _rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Returns current horizontal velocity
    /// </summary>
    public float GetHorizontalSpeed()
    {
        return _rb.linearVelocity.x;
    }

    public bool IsMovingHorizontally()
    {
        return Mathf.Abs(_rb.linearVelocity.x) > 0.01f;
    }

    /// <summary>
    /// Stops horizontal movement instantly
    /// </summary>
    public void StopHorizontalMovement()
    {
        SetHorizontalSpeed(0f);
    }

    /// <summary>
    /// Accelerates horizontal movement gradually
    /// </summary>
    public void AccelerateHorizontalMovement(float accelerationSpeed, float targetSpeed)
    {
        float currentSpeed = GetHorizontalSpeed();
        
        float newX = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationSpeed * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(newX, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Decelerates horizontal movement gradually
    /// </summary>
    public void DecelerateHorizontalMovement(float decelerationSpeed)
    {
        float currentSpeed = GetHorizontalSpeed();
        
        float newX = Mathf.MoveTowards(currentSpeed, 0f, decelerationSpeed * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector2(newX, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Clamps maximum downward velocity (Falling speed)
    /// </summary>
    public void ClampFallSpeed(float maxFallSpeed)
    {
        if (maxFallSpeed <= 0f) { return; }

        Vector2 velocity = _rb.linearVelocity;
        float clampedY = Mathf.Max(velocity.y, -maxFallSpeed);
        if (Mathf.Approximately(clampedY, velocity.y)) { return; }

        velocity.y = clampedY;
        _rb.linearVelocity = velocity;
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
        Debug.Log($"[CharacterMovement] {message}", this);
    }
}
