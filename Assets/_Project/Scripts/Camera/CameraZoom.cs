using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CinemachineFollow))]
public class CameraZoom : MonoBehaviour
{
    // --- References ---
    [SerializeField] private CinemachineFollow _follow;
    [SerializeField] private CinemachineConfiner2D _confiner;

    // --- Configuration ---
    [SerializeField] private CameraConfig _cameraConfig;
    [SerializeField] private Observable_Float _currentZoomOffset;
    [SerializeField] private Reference_Float _zoomSpeed;
    [SerializeField, Range(0f, 1f)] private float _smoothing = 0.1f;

    // --- Input ---
    [SerializeField] private InputActionProperty _zoomInputAction;

    // --- Events ---
    [SerializeField] private GameEvent_Float _onZoomConfigChanged;

    // --- Input Block ---
    private bool _inputBlocked;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private float _targetZoom;
    private float _currentZoom;

    // ========
    // LIFECYCLE
    // ========

    private void OnEnable()
    {
        _zoomInputAction.action?.Enable();
        InitializeZoom();
    }

    private void OnDisable()
    {
        _zoomInputAction.action?.Disable();
    }

    private void Update()
    {
        HandleZoomInput();
        ApplyZoom();
    }

    // =========
    // PUBLIC API
    // =========

    public void SetCameraConfig(CameraConfig newConfig)
    {
        _cameraConfig = newConfig;
        InitializeZoom();
    }

    // --- Zoom Logic ---

    /// <summary>
    /// Initializes zoom values.
    /// Clamped within the CameraConfig values
    /// </summary>
    public void InitializeZoom()
    {
        float initialZ = _follow.FollowOffset.z;
        float minZ = _cameraConfig.MinZoomOffset.Value;
        float maxZ = _cameraConfig.MaxZoomOffset.Value;

        if (initialZ < Mathf.Min(minZ, maxZ))
            initialZ = Mathf.Min(minZ, maxZ);
        else if (initialZ > Mathf.Max(minZ, maxZ))
            initialZ = Mathf.Max(minZ, maxZ);

        _currentZoom = initialZ;
        _targetZoom = initialZ;

        ApplyZoom();
    }

    /// <summary>
    /// Initializes zoom values from previous zoom.
    /// Clamped within the CameraConfig values
    /// </summary>
    public void InitializeZoomFromPrevious(Observable_Float previousZoom)
    {
        float initialZ = previousZoom.Value;
        float minZ = _cameraConfig.MinZoomOffset.Value;
        float maxZ = _cameraConfig.MaxZoomOffset.Value;

        if (initialZ < Mathf.Min(minZ, maxZ))
            initialZ = Mathf.Min(minZ, maxZ);
        else if (initialZ > Mathf.Max(minZ, maxZ))
            initialZ = Mathf.Max(minZ, maxZ);

        _currentZoom = initialZ;
        _targetZoom = initialZ;

        ApplyZoom();
    }

    /// <summary>
    /// Jumps to the target zoom instantly, skipping smoothing
    /// </summary>
    public void ApplyZoomInstantly()
    {
        _currentZoom = _targetZoom;
        ApplyZoom();
    }

    /// <summary>
    /// Enables/disables zoom input
    /// </summary>
    public void SetInputBlock(bool shouldBlock)
    {
        _inputBlocked = shouldBlock;
    }

    /// <summary>
    /// Invokes "On Zoom Config Changed" event with the current zoom offset value
    /// </summary>
    public void InvokeZoomChanged()
    {
        _onZoomConfigChanged?.Invoke(_currentZoom);
    }

    /// <summary>
    /// Reads the input (mouse scroll) and
    /// accumulates the target zoom offset
    /// Zoom speed decreases the closer the target gets to _minZoomOffset
    /// </summary>
    private void HandleZoomInput()
    {
        if (_inputBlocked) { return; }

        float scrollDelta = _zoomInputAction.action.ReadValue<Vector2>().y;

        if (Mathf.Approximately(scrollDelta, 0f)) { return; } // No scroll input

        float minZ = _cameraConfig.MinZoomOffset.Value;
        float maxZ = _cameraConfig.MaxZoomOffset.Value;

        // Compute speed multiplier: 1.0 at maxZoom (furthest), ~0 at minZoom (closest)
        float normalizedDist = Mathf.InverseLerp(maxZ, minZ, _targetZoom);
        float speedMultiplier = 1f - Mathf.Pow(normalizedDist, 0.8f);
        speedMultiplier = Mathf.Max(speedMultiplier, 0.25f); // Never fully 0

        // Scroll forward (Y > 0) → approach target → Z moves toward 0 (less negative)
        _targetZoom += scrollDelta * _zoomSpeed.Value * speedMultiplier;

        // Clamp: maxZoomOffset is the furthest (less), minZoomOffset is the closest (more)
        _targetZoom = Mathf.Clamp(_targetZoom, Mathf.Min(minZ, maxZ), Mathf.Max(minZ, maxZ));

        D($"Scroll: {scrollDelta:F3} | SpeedMul: {speedMultiplier:F2} | Target Z: {_targetZoom:F2}");
    }

    /// <summary>
    /// Smoothly interpolates the current zoom toward the target and applies it to Cinemachine Follow
    /// </summary>
    private void ApplyZoom()
    {
        float t = 1f - Mathf.Exp(-_smoothing * 60f * Time.deltaTime);
        _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, t);

        // Preserve X and Y, change only Z
        Vector3 offset = _follow.FollowOffset;
        offset.z = _currentZoom;
        _follow.FollowOffset = offset;

        // Write current zoom to the Observable
        if (_currentZoomOffset != null)
        {
            _currentZoomOffset.Value = _currentZoom;
        }

        // Force confiner to recalculate bounds with the new camera position
        _confiner?.InvalidateBoundingShapeCache();
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[CameraZoom:{name}] {message}", this);
    }
}
