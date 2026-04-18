using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Moving,
        Jumping
    }

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _moveSpeed = 5f;
    [SerializeField] private Key _moveLeftKey = Key.A;
    [SerializeField] private Key _moveRightKey = Key.D;

    [Header("Surface Detection")]
    [SerializeField] private string _surfaceTag = "Surface";
    [SerializeField] private LayerMask _surfaceMask = Physics2D.DefaultRaycastLayers;
    [SerializeField, Min(0.01f)] private float _rayDistance = 0.6f;
    [SerializeField, Min(0f)] private float _rayStartYOffset = 0.05f;
    [SerializeField, Min(0f)] private float _sideRayOffset = 0.35f;

    [Header("Debug")]
    [SerializeField] private bool _drawDebug = true;
    [SerializeField] private bool _debug = false;

    // --- State ---
    [SerializeField] private MovementState _currentState = MovementState.Idle;
    private float _moveInput;
    // --- Flags ---
    private bool _isInputLocked;
    // --- Cache ---
    private string _lastDebug;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _characterCollider;

    public MovementState CurrentState => _currentState;

    /// <summary>
    /// Returns true when left or right movement keys are currently pressed
    /// </summary>
    public bool HasMovementInput()
    {
        if (_isInputLocked) { return false; }
        if (Keyboard.current == null) { return false;}

        return Keyboard.current[_moveLeftKey].isPressed || Keyboard.current[_moveRightKey].isPressed;
    }

    /// <summary>
    /// Locks/Unlocks movement input
    /// </summary>
    public void LockInput(bool value)
    {
        _isInputLocked = value;

        if (_isInputLocked)
        {
            _moveInput = 0f;
            SetState(MovementState.Idle);
            StopHorizontalMovement();
        }
    }

    /// <summary>
    /// Caches required components and prepares movement service
    /// </summary>
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _characterCollider = GetComponent<Collider2D>();
        if (_characterCollider == null)
        {
            _characterCollider = GetComponentInChildren<Collider2D>();
        }
    }

    /// <summary>
    /// Reads horizontal input from Input System and updates grounded states
    /// </summary>
    private void Update()
    {
        if (_isInputLocked)
        {
            _moveInput = 0f;
            SetState(MovementState.Idle);
            return;
        }

        if (_currentState == MovementState.Jumping)
        {
            return;
        }

        _moveInput = ReadHorizontalInput();
        UpdateGroundedState();
    }

    /// <summary>
    /// Returns horizontal input value in range [-1, 1] using keyboard keys
    /// </summary>
    private float ReadHorizontalInput()
    {
        if (Keyboard.current == null)
        {
            return 0f;
        }

        float left = Keyboard.current[_moveLeftKey].isPressed ? 1f : 0f;
        float right = Keyboard.current[_moveRightKey].isPressed ? 1f : 0f;
        return right - left;
    }

    /// <summary>
    /// Applies horizontal velocity based on current movement state
    /// </summary>
    private void FixedUpdate()
    {
        if (_currentState == MovementState.Jumping)
        {
            StopHorizontalMovement();
            return;
        }

        if (_currentState != MovementState.Moving)
        {
            StopHorizontalMovement();
            return;
        }

        float direction = Mathf.Sign(_moveInput);
        if (!CanMoveInDirection(direction))
        {
            SetState(MovementState.Idle);
            StopHorizontalMovement();
            return;
        }

        SetHorizontalSpeed(direction * _moveSpeed);
    }

    /// <summary>
    /// Forces character into Jumping state and disables horizontal control
    /// </summary>
    public void EnterJumpingState()
    {
        _moveInput = 0f;
        SetState(MovementState.Jumping);
        StopHorizontalMovement();
    }

    /// <summary>
    /// Returns character from Jumping state back to grounded logic
    /// </summary>
    public void ExitJumpingState()
    {
        _moveInput = 0f;
        SetState(MovementState.Idle);
    }

    /// <summary>
    /// Switches Jumping state
    /// </summary>
    /// <param name="isJumping">True to lock movement, false to unlock it</param>
    public void SetJumpingState(bool isJumping)
    {
        if (isJumping)
        {
            EnterJumpingState();
            return;
        }

        ExitJumpingState();
    }

    /// <summary>
    /// Immediately sets movement state to Idle and stops horizontal velocity
    /// </summary>
    public void ForceIdleState()
    {
        _moveInput = 0f;
        SetState(MovementState.Idle);
        StopHorizontalMovement();
    }

    /// <summary>
    /// Updates Idle or Moving state using horizontal input and edge checks
    /// </summary>
    private void UpdateGroundedState()
    {
        if (Mathf.Abs(_moveInput) < 0.01f)
        {
            D($"Idle: input={_moveInput:0.##}");
            SetState(MovementState.Idle);
            return;
        }

        float direction = Mathf.Sign(_moveInput);
        SetState(CanMoveInDirection(direction) ? MovementState.Moving : MovementState.Idle);
    }

    /// <summary>
    /// Validates that current and forward raycasts hit Surface-tagged colliders
    /// </summary>
    /// <param name="direction">Horizontal direction: -1 left, +1 right</param>
    private bool CanMoveInDirection(float direction)
    {
        if (!HasSurfaceBelow(GetCenterRayOrigin()))
        {
            D("Blocked: center ray miss");
            return false;
        }

        if (direction > 0f)
        {
            bool ok = HasSurfaceBelow(GetRightRayOrigin());
            if (!ok) { D("Blocked: right ray miss"); }
            return ok;
        }

        if (direction < 0f)
        {
            bool ok = HasSurfaceBelow(GetLeftRayOrigin());
            if (!ok) { D("Blocked: left ray miss"); }
            return ok;
        }

        return false;
    }

    /// <summary>
    /// Casts one ray down from the given origin and validates Surface tag
    /// </summary>
    private bool HasSurfaceBelow(Vector2 origin)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, _rayDistance, _surfaceMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null) { continue; }

            // Ignore character colliders on this object and children.
            if (hitCollider.transform == transform || hitCollider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hitCollider.CompareTag(_surfaceTag))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns origin for center down ray at character feet level
    /// </summary>
    private Vector2 GetCenterRayOrigin()
    {
        if (_characterCollider == null)
        {
            Vector2 fallbackPosition = transform.position;
            return fallbackPosition + Vector2.up * _rayStartYOffset;
        }

        Bounds bounds = _characterCollider.bounds;
        return new Vector2(bounds.center.x, bounds.min.y + _rayStartYOffset);
    }

    /// <summary>
    /// Returns origin for left down ray
    /// </summary>
    private Vector2 GetLeftRayOrigin()
    {
        return GetCenterRayOrigin() + Vector2.left * _sideRayOffset;
    }

    /// <summary>
    /// Returns origin for right down ray
    /// </summary>
    private Vector2 GetRightRayOrigin()
    {
        return GetCenterRayOrigin() + Vector2.right * _sideRayOffset;
    }

    /// <summary>
    /// Sets horizontal velocity while preserving current vertical velocity
    /// </summary>
    private void SetHorizontalSpeed(float speed)
    {
        Vector2 velocity = _rigidbody2D.linearVelocity;
        velocity.x = speed;
        _rigidbody2D.linearVelocity = velocity;
    }

    /// <summary>
    /// Stops horizontal movement instantly
    /// </summary>
    private void StopHorizontalMovement()
    {
        SetHorizontalSpeed(0f);
    }

    /// <summary>
    /// Changes current movement state only when value is different
    /// </summary>
    private void SetState(MovementState newState)
    {
        if (_currentState == newState) { return; }

        D($"State: {_currentState} -> {newState}");

        _currentState = newState;
    }

    /// <summary>
    /// Prints debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[CharacterMovement] {message}", this);
    }

    /// <summary>
    /// Draws Rays used for Edge detection
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!_drawDebug) { return; }

        Vector2 center = GetCenterRayOrigin();
        Vector2 left = GetLeftRayOrigin();
        Vector2 right = GetRightRayOrigin();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center, center + Vector2.down * _rayDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(left, left + Vector2.down * _rayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(right, right + Vector2.down * _rayDistance);
    }
}
