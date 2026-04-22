using System.Collections.Generic;
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
    [SerializeField, Min(0.01f)] private float _cursorBoxWidth = 0.9f;
    [SerializeField, Min(0.01f)] private float _cursorBoxHeight = 0.2f;
    [SerializeField, Min(0f)] private float _surfaceCoverageEpsilon = 0.01f;

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
    private readonly List<Vector2> _coverageIntervals = new();
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
        if (_characterMovement.CurrentState == CharacterMovement.MovementState.Falling) { return; }
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
        if (_characterMovement.CurrentState == CharacterMovement.MovementState.Falling) { return; }
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
        if (_characterMovement.CurrentState == CharacterMovement.MovementState.Falling) { return; } // Falling -> Skip

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
        if (_characterMovement.CurrentState == CharacterMovement.MovementState.Falling) { return true; } // Falling -> Break Focus

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
        if (_jumpTargetManager == null) { return; }

        Camera targetCamera = _worldCamera != null ? _worldCamera : Camera.main;
        if (targetCamera == null || Mouse.current == null)
        {
            HideAndClearTarget();
            return;
        }

        Vector2 cursorWorld = ScreenToWorldPoint(targetCamera, Mouse.current.position.ReadValue(), transform.position.z); // Cursor World pos
        Vector2 focusCenter = (Vector2)transform.position + Vector2.up * _focusCenterOffsetY; // Focus area center pos

        if (Vector2.Distance(cursorWorld, focusCenter) > _focusRadius) // Cursor out of Focus area
        {
            HandleNoValidHit(cursorWorld); // Enable "Sticky" Target or hide
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

        HandleNoValidHit(cursorWorld); // Enable "Sticky" Target or hide
    }

    /// <summary>
    /// Finds valid Surface hit under cursor BoxCast (validates full-width coverage)
    /// </summary>
    private bool TryGetSurfaceHit(Vector2 cursorWorld, out RaycastHit2D validHit)
    {
        validHit = default;

        Vector2 boxOrigin = cursorWorld + Vector2.up * _cursorRayStartYOffset; // Box World pos
        Vector2 boxSize = new Vector2(_cursorBoxWidth, _cursorBoxHeight);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(boxOrigin, boxSize, 0f, Vector2.down, _cursorRayDistance, _surfaceMask);

        if (hits.Length == 0) // No hits -> Return False
        {
            validHit = default;
            D("No BoxCast hits");
            return false;
        }

        // Get coverage edges
        float requiredLeft = cursorWorld.x - _cursorBoxWidth * 0.5f;
        float requiredRight = cursorWorld.x + _cursorBoxWidth * 0.5f;

        _coverageIntervals.Clear();
        bool hasValidSurfaceHit = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null) { continue; }

            Transform hitTransform = hitCollider.transform;
            if (hitTransform == transform || hitTransform.IsChildOf(transform)) { continue; } // Character Collider -> Skip
            if (!hitCollider.CompareTag(_surfaceTag)) { continue; } // Not Surface -> Skip

            if (!hasValidSurfaceHit) // First valid hit -> Cache it
            {
                hasValidSurfaceHit = true;
                validHit = hits[i];
            }

            float intervalStart = Mathf.Max(requiredLeft, hitCollider.bounds.min.x);
            float intervalEnd = Mathf.Min(requiredRight, hitCollider.bounds.max.x);

            if (intervalEnd - intervalStart <= _surfaceCoverageEpsilon) { continue; } // No meaningful coverage -> Skip

            _coverageIntervals.Add(new Vector2(intervalStart, intervalEnd)); // Add coverage interval
        }

        if (!hasValidSurfaceHit) // No valid Surface hits -> Return False
        {
            validHit = default;
            D("No valid Surface in BoxCast");
            return false;
        }

        if (!HasFullSurfaceCoverage(requiredLeft, requiredRight)) // No full-width coverage -> Return False
        {
            validHit = default;
            D("Surface coverage gap");
            return false;
        }

        D("Full-width BoxCast");
        return true;
    }

    /// <summary>
    /// Validates that Surface intervals merged cover the required range
    /// </summary>
    private bool HasFullSurfaceCoverage(float requiredLeft, float requiredRight)
    {
        if (_coverageIntervals.Count == 0) { return false; } // No intervals -> Return False

        _coverageIntervals.Sort((a, b) => a.x.CompareTo(b.x)); // Sort intervals by start (left) edge

        float mergedEnd = _coverageIntervals[0].y;
        if (_coverageIntervals[0].x > requiredLeft + _surfaceCoverageEpsilon)
        {
            return false; // Leftmost interval starts after left edge -> Return False
        } 
        if (mergedEnd >= requiredRight - _surfaceCoverageEpsilon)
        {
            return true; // First interval alone covers -> Return True
        }

        for (int i = 1; i < _coverageIntervals.Count; i++)
        {
            Vector2 interval = _coverageIntervals[i];
            if (interval.x > mergedEnd + _surfaceCoverageEpsilon)
            {
                return false;
            }

            if (interval.y > mergedEnd)
            {
                mergedEnd = interval.y;
            }

            if (mergedEnd >= requiredRight - _surfaceCoverageEpsilon)
            {
                return true;
            }
        }

        return mergedEnd >= requiredRight - _surfaceCoverageEpsilon;
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
    private static Vector2 ScreenToWorldPoint(Camera cam, Vector2 screenPosition, float worldPlaneZ)
    {
        Ray mouseRay = cam.ScreenPointToRay(screenPosition);
        Plane gameplayPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, worldPlaneZ));

        if (gameplayPlane.Raycast(mouseRay, out float enter))
        {
            Vector3 worldHit = mouseRay.GetPoint(enter);
            return new Vector2(worldHit.x, worldHit.y);
        }

        // Fallback for edge cases where the ray is parallel to the XY gameplay plane
        float fallbackDepth = Mathf.Abs(worldPlaneZ - cam.transform.position.z);
        Vector3 fallback = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, fallbackDepth));
        return new Vector2(fallback.x, fallback.y);
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
