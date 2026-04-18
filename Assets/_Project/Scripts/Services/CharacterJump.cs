using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterMovement))]
public class CharacterJump : MonoBehaviour
{
    public enum State
    {
        None,
        Focus,
        Jumping
    }

    [Header("References")]
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private JumpTargetManager _jumpTargetManager;
    [SerializeField] private Camera _worldCamera;

    [Header("Surface Detection")]
    [SerializeField] private string _surfaceTag = "Surface";
    [SerializeField] private LayerMask _surfaceMask = Physics2D.DefaultRaycastLayers;
    [SerializeField, Min(0f)] private float _cursorRayStartYOffset = 0.1f;
    [SerializeField, Min(0.01f)] private float _cursorRayDistance = 1.5f;

    [Header("Focus")]
    [SerializeField, Min(0f)] private float _focusEnterWhileMovingWindow = 0.5f;
    [SerializeField, Min(0f)] private float _focusInputLockDuration = 0.5f;
    [SerializeField, Min(0f)] private float _focusBreakDistance = 0.05f;
    [SerializeField, Min(0f)] private float _focusPhysicsGraceDuration = 0.5f;
    [SerializeField, Min(0.01f)] private float _focusRadius = 5f;
    [SerializeField] private float _focusCenterOffsetY = 0f;
    [SerializeField, Min(0f)] private float _targetTopYOffset = 1f;
    [SerializeField, Min(0f)] private float _lostHitDistance = 1.5f;
    [SerializeField] private State _currentState = State.None;

    [Header("Debug")]
    [SerializeField] private bool _debug = false;

    // --- State ---
    private Vector2 _focusAnchorPosition;
    private float _focusEnterRequestEndTime;
    private float _focusInputLockEndTime;
    private float _focusPhysicsGraceEndTime;
    // --- Flags ---
    private bool _isInputLocked;
    private bool _isFocusEnterRequestPending;
    private bool _isJumpInProgress;
    private bool _hasLastValidPoint;
    // --- Cache ---
    private Vector2 _lastValidTargetPosition;
    private string _lastDebug;

    public State CurrentState => _currentState;

    /// <summary>
    /// Prepares to function
    /// Caches required components
    /// </summary>
    private void Awake()
    {
        _characterMovement ??= GetComponent<CharacterMovement>();
    }

    /// <summary>
    /// Handles focus toggle input and target updates while focused
    /// </summary>
    private void Update()
    {
        if (_currentState == State.Focus && _isInputLocked && Time.time >= _focusInputLockEndTime)
        {
            LockInput(false);
            _characterMovement?.LockInput(false);
        }

        HandlePendingFocusEnterRequest();

        HandleFocusToggleInput();

        if (_currentState != State.Focus)
        {
            return;
        }

        HandleJumpInput();

        if (_currentState != State.Focus)
        {
            return;
        }

        if (ShouldBreakFocus())
        {
            ExitFocus();
            return;
        }

        UpdateFocusTarget();
    }

    /// <summary>
    /// Placeholder
    /// </summary>
    public void PerformJump()
    {
        if (_isJumpInProgress) { return; }
        if (_currentState != State.Focus) { return; }
        if (_jumpTargetManager == null || !_jumpTargetManager.IsTargetValid()) { return; }

        StartJump(_jumpTargetManager.GetPosition());
        CompleteJump();
    }

    /// <summary>
    /// Handles jump click
    /// </summary>
    private void HandleJumpInput()
    {
        //if (_isInputLocked) { return; } It doesn't care about locked input rn
        if (_isJumpInProgress) { return; }
        if (Mouse.current == null) { return; }
        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }
        if (_jumpTargetManager == null || !_jumpTargetManager.IsTargetValid()) { return; }

        PerformJump();
    }

    /// <summary>
    /// Starts jump lifecycle and teleports character to target
    /// </summary>
    private void StartJump(Vector3 targetPosition)
    {
        _isJumpInProgress = true;
        _currentState = State.Jumping;
        LockInput(true);
        _characterMovement?.LockInput(true);
        _characterMovement?.EnterJumpingState();

        Vector3 current = transform.position;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, current.z);

        if (_jumpTargetManager != null)
        {
            _jumpTargetManager.Show(false);
            _jumpTargetManager.ClearTargetPosition();
        }

        _hasLastValidPoint = false;
        _lastValidTargetPosition = Vector2.zero;

        D("State: Jumping");
    }

    /// <summary>
    /// Finishes jump lifecycle and returns to None state
    /// </summary>
    public void CompleteJump()
    {
        if (!_isJumpInProgress) { return; } // Not Jumping -> Skip

        _isJumpInProgress = false;
        _currentState = State.None;
        LockInput(false);
        _characterMovement?.LockInput(false);
        _characterMovement?.ExitJumpingState();
        D("State: None");
    }

    /// <summary>
    /// Locks/Unlocks jump inputs
    /// </summary>
    public void LockInput(bool value)
    {
        _isInputLocked = value;

        // Clear request buffer
        if (_isInputLocked)
        {
            _isFocusEnterRequestPending = false;
        }
    }

    /// <summary>
    /// Processes delayed focus enter request made while character is moving
    /// </summary>
    private void HandlePendingFocusEnterRequest()
    {
        if (!_isFocusEnterRequestPending) { return; } // No Request -> Skip

        if (_currentState != State.None) // In some State -> Skip
        {
            _isFocusEnterRequestPending = false;
            return;
        }

        if (Time.time >= _focusEnterRequestEndTime) // Request expired -> Skip
        {
            _isFocusEnterRequestPending = false;
            D("Focus Request expired");
            return;
        }

        if (_characterMovement.CurrentState == CharacterMovement.MovementState.Idle) // Idle -> Enter Focus
        {
            _isFocusEnterRequestPending = false;
            EnterFocus();
        }
    }

    /// <summary>
    /// Toggles focus mode on RM Click
    /// </summary>
    private void HandleFocusToggleInput()
    {
        if (_isInputLocked) { return; }
        if (Mouse.current == null) { return; }
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        ToggleFocus();
    }

    /// <summary>
    /// Switches between Focus and None
    /// </summary>
    private void ToggleFocus()
    {
        if (_currentState == State.Jumping) { return;} // Jumping -> Skip

        if (_currentState == State.Focus) // Focus -> None
        {
            ExitFocus();
            return;
        }

        if (_characterMovement.CurrentState == CharacterMovement.MovementState.Moving) // Moving -> Set pending Focus Request
        {
            _isFocusEnterRequestPending = true;
            _focusEnterRequestEndTime = Time.time + _focusEnterWhileMovingWindow;
            D("Focus Request pending");
            return;
        }

        EnterFocus(); // None -> Focus
    }

    /// <summary>
    /// Enters Focus state and gives grace period for instant displacement (sliding)
    /// </summary>
    private void EnterFocus()
    {
        if (_currentState == State.Focus) { return;}

        _isFocusEnterRequestPending = false;

        LockInput(true);
        _focusInputLockEndTime = Time.time + _focusInputLockDuration;

        _focusAnchorPosition = transform.position;
        _focusPhysicsGraceEndTime = Time.time + _focusPhysicsGraceDuration;
        _currentState = State.Focus;

        _characterMovement?.ForceIdleState();
        _characterMovement?.LockInput(true);

        if (_jumpTargetManager != null)
        {
            _jumpTargetManager.Show(false);
            _jumpTargetManager.ClearTargetPosition();
        }

        _hasLastValidPoint = false;
        _lastValidTargetPosition = Vector2.zero;

        D("State: Focus");
    }

    /// <summary>
    /// Exits Focus state and restores movement
    /// </summary>
    private void ExitFocus()
    {
        if (_currentState != State.Focus)
        {
            return;
        }

        _isFocusEnterRequestPending = false;
        LockInput(false);
        _characterMovement?.LockInput(false);
        _currentState = State.None;

        if (_jumpTargetManager != null)
        {
            _jumpTargetManager.Show(false);
            _jumpTargetManager.ClearTargetPosition();
        }

        _hasLastValidPoint = false;
        _lastValidTargetPosition = Vector2.zero;

        D("State: None");
    }

    /// <summary>
    /// Validates whether Focus state must be cancelled
    /// </summary>
    private bool ShouldBreakFocus()
    {
        if (_isInputLocked)
        {
            _focusAnchorPosition = transform.position;
            return false;
        }

        if (_characterMovement != null && _characterMovement.HasMovementInput()) // Any movement input -> Break Focus
        {
            return true;
        }

        if (Time.time < _focusPhysicsGraceEndTime) // During grace period, ignore displacement
        {
            _focusAnchorPosition = transform.position;
            return false;
        }

        if (_focusBreakDistance > 0f)
        {
            float displacement = Vector2.Distance(transform.position, _focusAnchorPosition);
            if (displacement >= _focusBreakDistance) // Displacement over threshold -> Break Focus
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates Jump Target visibility and position in Focus state
    /// </summary>
    private void UpdateFocusTarget()
    {
        if (_jumpTargetManager == null)
        {
            return;
        }

        Camera targetCamera = _worldCamera != null ? _worldCamera : Camera.main;
        if (targetCamera == null || Mouse.current == null)
        {
            HideAndClearTarget();
            return;
        }

        Vector2 cursorWorld = ScreenToWorldPoint(targetCamera, Mouse.current.position.ReadValue());
        Vector2 focusCenter = (Vector2)transform.position + Vector2.up * _focusCenterOffsetY;

        if (Vector2.Distance(cursorWorld, focusCenter) > _focusRadius)
        {
            HandleNoValidHit(cursorWorld);
            return;
        }

        if (TryGetSurfaceHit(cursorWorld, out RaycastHit2D hit))
        {
            Vector2 targetPosition = new Vector2(cursorWorld.x, hit.collider.bounds.max.y + _targetTopYOffset);
            _jumpTargetManager.SetPosition(targetPosition);
            _jumpTargetManager.Show(true);

            _lastValidTargetPosition = targetPosition;
            _hasLastValidPoint = true;
            D($"Valid hit: {hit.collider.name}");
            return;
        }

        HandleNoValidHit(cursorWorld);
    }

    /// <summary>
    /// Finds first valid Surface hit under cursor ray
    /// </summary>
    private bool TryGetSurfaceHit(Vector2 cursorWorld, out RaycastHit2D validHit)
    {
        Vector2 rayOrigin = cursorWorld + Vector2.up * _cursorRayStartYOffset;
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, _cursorRayDistance, _surfaceMask);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null)
            {
                continue;
            }

            Transform hitTransform = hitCollider.transform;
            if (hitTransform == transform || hitTransform.IsChildOf(transform))
            {
                continue;
            }

            if (!hitCollider.CompareTag(_surfaceTag))
            {
                continue;
            }

            validHit = hits[i];
            return true;
        }

        validHit = default;
        return false;
    }

    /// <summary>
    /// Keeps "sticky" Jump Target or hides it
    /// </summary>
    private void HandleNoValidHit(Vector2 cursorWorld)
    {
        if (!_hasLastValidPoint) // No valid point -> Hide
        {
            HideAndClearTarget();
            return;
        }

        float distance = Vector2.Distance(cursorWorld, _lastValidTargetPosition);
        if (distance <= _lostHitDistance) // Within lost hit distance -> Keeps "sticky" Jump Target
        {
            _jumpTargetManager.SetPosition(_lastValidTargetPosition);
            _jumpTargetManager.Show(true);
            D("Sticky target");
            return;
        }

        HideAndClearTarget(); // Outside lost hit distance -> Hide
    }

    /// <summary>
    /// Hides Jump Target and clears cached Jump Target position
    /// </summary>
    private void HideAndClearTarget()
    {
        if (_jumpTargetManager != null)
        {
            _jumpTargetManager.Show(false);
            _jumpTargetManager.ClearTargetPosition();
        }

        _hasLastValidPoint = false;
        _lastValidTargetPosition = Vector2.zero;
        D("Hidden");
    }

    /// <summary>
    /// Converts cursor Screen coordinates to World coordinates
    /// </summary>
    private static Vector2 ScreenToWorldPoint(Camera cam, Vector2 screenPosition)
    {
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.nearClipPlane));
        return new Vector2(world.x, world.y);
    }

    /// <summary>
    /// Prints debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[CharacterJump] {message}", this);
    }
}
