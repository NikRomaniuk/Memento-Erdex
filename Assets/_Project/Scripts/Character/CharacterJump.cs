using UnityEngine;

[DisallowMultipleComponent]
public class CharacterJump : MonoBehaviour
{
    // --- References ---
    [SerializeField] private CharacterManager _characterManager;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // --- Cache ---
    private Rigidbody2D _rb;

    private void Awake()
    {
        if (_characterManager == null)
        {
            Debug.LogError("[CharacterJump] CharacterManager reference is not assigned!", this);
            return;
        }

        _rb = _characterManager.RB;
    }

    // =============
    // JUMP METHODS
    // =============

    /// <summary>
    /// Checks if the Character is able to jump
    /// </summary>
    public bool CanJump()
    {
        return _characterManager.GroundCheck.HasSurfaceBelow();
    }

    /// <summary>
    /// Performs a Jump using JumpForce from CharacterData
    /// </summary>
    public void Jump()
    {
        Jump(_characterManager.Data.JumpForce);
    }

    /// <summary>
    /// Performs a Jump with the specified force
    /// </summary>
    public void Jump(float force)
    {
        if (!CanJump()) { return; }

        // Reset vertical velocity for consistent jump height
        Vector2 velocity = _rb.linearVelocity;
        velocity.y = 0f;
        _rb.linearVelocity = velocity;

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Cuts upward velocity mid-air
    /// Used for variable jump height
    /// </summary>
    public void CutJump()
    {
        if (_rb.linearVelocity.y <= 0f) { return; }

        Vector2 velocity = _rb.linearVelocity;
        velocity.y *= 0.9f;
        _rb.linearVelocity = velocity;

        D("Jump cut");
    }

    // =============
    // HELPER METHODS
    // =============

    /// <summary>
    /// Prints debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[CharacterJump] {message}", this);
    }
}
