using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public partial class CharacterManager : MonoBehaviour
{
    // --- References ---
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _col;
    [SerializeField] private CharacterData _data;
    [SerializeField] private CharacterInput _input;
    [SerializeField] private CharacterAnimation _animation;
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] private CharacterMovement _movement;
    [SerializeField] private CharacterJump _jump;
    [SerializeField] private Observable_Vector3 _position;

    // --- Game Over ---
    [SerializeField] private GameEvent _characterFellEvent;
    [SerializeField] private float _fallBelowYThreshold = -25f;

    // --- Debug ---
    [ShowInInspector] private string currentState;
    [ShowInInspector] private float linearVelocityY;
    [SerializeField] private bool _debug = true;
    private string _lastDebug;

    private bool _hasCharacterFellEventBeenSent;
    private StateMachine<CharacterManager> _movementStateMachine;

    // Super States
    private GroundState _groundState;
    private AirState _airState;
    private FocusState _focusState;

    // Ground Sub-States
    private GroundIdleState _groundIdleState;
    private GroundMoveState _groundMoveState;

    // Air Sub-States
    private AirFallState _airFallState;
    private AirJumpState _airJumpState;
    private AirLeapState _airLeapState;

    // Focus Sub-States
    private FocusGroundState _focusGroundState;
    private FocusAirState _focusAirState;

    // --- Public Accessors ---
    public Rigidbody2D RB => _rb;
    public Collider2D Collider => _col;
    public CharacterData Data => _data;
    public CharacterInput Input => _input;
    public GroundCheck GroundCheck => _groundCheck;
    public CharacterMovement Movement => _movement;
    public CharacterJump Jump => _jump;
    public CharacterAnimation Animation => _animation;
    // States
    public StateMachine<CharacterManager> MovementState => _movementStateMachine;
    public State<CharacterManager> CharacterGroundState => _groundState;
    public State<CharacterManager> CharacterAirState => _airState;
    public State<CharacterManager> CharacterFocusState => _focusState;
    public State<CharacterManager> CharacterGroundIdleState => _groundIdleState;
    public State<CharacterManager> CharacterGroundMoveState => _groundMoveState;
    public State<CharacterManager> CharacterAirFallState => _airFallState;
    public State<CharacterManager> CharacterAirJumpState => _airJumpState;
    public State<CharacterManager> CharacterAirLeapState => _airLeapState;
    public State<CharacterManager> CharacterFocusGroundState => _focusGroundState;
    public State<CharacterManager> CharacterFocusAirState => _focusAirState;

    // --- Public Variables ---
    [HideInInspector] public int DirectionFacing { get; private set; } = 1;

    private void Awake()
    {
        // Declare State Machine
        _movementStateMachine = new StateMachine<CharacterManager>();

        // Initialize Super States
        _groundState = new GroundState(this, _movementStateMachine);
        _airState = new AirState(this, _movementStateMachine);
        _focusState = new FocusState(this, _movementStateMachine);

        // Initialize Sub-States
        _groundIdleState = new GroundIdleState(this, _movementStateMachine, _groundState);
        _groundMoveState = new GroundMoveState(this, _movementStateMachine, _groundState);
        _airFallState = new AirFallState(this, _movementStateMachine, _airState);
        _airJumpState = new AirJumpState(this, _movementStateMachine, _airState);
        _airLeapState = new AirLeapState(this, _movementStateMachine, _airState);
        _focusGroundState = new FocusGroundState(this, _movementStateMachine, _focusState);
        _focusAirState = new FocusAirState(this, _movementStateMachine, _focusState);

        // Initialize State Machine
        _movementStateMachine.Initialize(_groundIdleState);
    }

    /// <summary>
    /// Updates the current state,
    /// Checks every frame if Character fell below threshold
    /// </summary>
    private void Update()
    {
        _movementStateMachine.CurrentState?.LogicUpdate();
        currentState = _movementStateMachine.CurrentState?.Name;
        linearVelocityY = RB.linearVelocity.y;

        _position.Value = transform.position;

        if (_hasCharacterFellEventBeenSent) { return; }
        if (transform.position.y >= _fallBelowYThreshold) { return; }

        _hasCharacterFellEventBeenSent = true;
        D($"Character fell below threshold {_fallBelowYThreshold} -> Invoking CharacterFell event");

        if (_characterFellEvent == null)
        {
            Debug.LogWarning("[CharacterManager] CharacterFell event is not assigned", this);
            return;
        }

        _characterFellEvent.Invoke();
    }

    /// <summary>
    /// Fixed Updates the current state
    /// </summary>
    private void FixedUpdate()
    {
        _movementStateMachine.CurrentState?.PhysicsUpdate();
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[CharacterManager] {message}", this);
    }

    public void SetDirectionFacing(int direction)
    {
        if (direction == 0) { return; }
        DirectionFacing = direction > 0 ? 1 : -1;
    }
}
