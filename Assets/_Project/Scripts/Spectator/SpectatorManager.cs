using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Cinemachine;

[DisallowMultipleComponent]
public partial class SpectatorManager : MonoBehaviour
{
    // --- References ---
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpectatorInput _input;
    [SerializeField] private SpectatorMovement _movement;
    [SerializeField] private SpectatorData _data;
    [SerializeField] private Observable_Vector3 _characterPosition;

    // --- Camera Offset ---
    [SerializeField] private Transform _cinemachineCamera;
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private float _maxCameraOffset = 0.5f;
    [SerializeField] private float _returnSpeedScale = 0.05f;
    [SerializeField] private float _returnCurvePower = 1.25f;

    // --- Zoom ---
    [SerializeField] private CameraConfig _cameraConfig;
    [SerializeField] private Observable_Float _currentZoomOffset;
    [SerializeField] private float _zoomSpeedScale = 0.5f;

    // --- Debug ---
    [ShowInInspector] private string _currentState;
    [SerializeField] private bool _debug = true;
    private string _lastDebug;

    private StateMachine<SpectatorManager> _stateMachine;

    // States
    private IdleState _idleState;
    private MoveState _moveState;

    // --- Public Accessors ---
    public Rigidbody2D RB => _rb;
    public SpectatorInput Input => _input;
    public SpectatorMovement Movement => _movement;
    public SpectatorData Data => _data;

    private void Awake()
    {
        _stateMachine = new StateMachine<SpectatorManager>();

        _idleState = new IdleState(this, _stateMachine);
        _moveState = new MoveState(this, _stateMachine);

        _stateMachine.Initialize(_idleState);
    }

    /// <summary>
    /// Updates the current state
    /// </summary>
    private void Update()
    {
        _stateMachine.CurrentState?.LogicUpdate();
        _currentState = _stateMachine.CurrentState?.Name;
    }

    /// <summary>
    /// <para> Returns per-axis speed multiplier based on Cameras offset (Main Camera and Cinemachine) </para>
    /// Moving away from "return direction" -> ▼ Speed <br/>
    /// Moving toward "return direction" -> ▲ Speed (Scales upon exceeding <see cref="_maxCameraOffset"/>)
    /// </summary>
    public Vector2 GetCameraOffsetMultiplier(Vector2 moveDirection)
    {
        if (_cinemachineCamera == null || _mainCamera == null)
            return Vector2.one;

        Vector3 offset = _cinemachineCamera.position - _mainCamera.position;

        float multX = GetAxisMultiplier(offset.x, moveDirection.x);
        float multY = GetAxisMultiplier(offset.y, moveDirection.y);

        return new Vector2(multX, multY);
    }

    private float GetAxisMultiplier(float offset, float input)
    {
        if (input == 0f) return 1f;

        // Moving toward return direction
        if (Mathf.Sign(offset) != Mathf.Sign(input))
        {
            float absOffset = Mathf.Abs(offset);
            float excess = Mathf.Max(0f, (absOffset - _maxCameraOffset) / _maxCameraOffset);
            float curvedExcess = Mathf.Pow(excess, _returnCurvePower);
            return 1f + _returnSpeedScale * curvedExcess;
        }

        // Moving away from center -> slow down as before
        return 1f - Mathf.Clamp01(Mathf.Abs(offset) / _maxCameraOffset);
    }

    /// <summary>
    /// Returns speed multiplier based on current zoom.
    /// At MinZoomOffset → 1x (base speed). Further away → faster.
    /// </summary>
    public float GetZoomSpeedMultiplier()
    {
        if (_cameraConfig == null || _currentZoomOffset == null)
            return 1f;

        float minZ = _cameraConfig.MinZoomOffset;
        float currentZ = _currentZoomOffset.Value;

        if (Mathf.Approximately(currentZ, 0f) || Mathf.Approximately(minZ, 0f))
            return 1f;

        float ratio = currentZ / minZ;
        return 1f + _zoomSpeedScale * (ratio - 1f);
    }

    /// <summary>
    /// Teleports Spectator to Character position
    /// </summary>
    public void TeleportToCharacter()
    {
        transform.position = _characterPosition.Value;
    }

    /// <summary>
    /// Fixed Updates the current state
    /// </summary>
    private void FixedUpdate()
    {
        _stateMachine.CurrentState?.PhysicsUpdate();
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[SpectatorManager] {message}", this);
    }
}
