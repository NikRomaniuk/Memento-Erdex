using UnityEngine;

/// <summary>
/// Anchor representing the HIGHEST point of the jump arc in the minigame
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class JumpAnchor : MonoBehaviour
{
    // --- References ---
    [SerializeField] private Rigidbody2D _rigidbody;

    // ===========
    // APPLY FORCES
    // ===========

    /// <summary>
    /// Applies an impulse force to the Anchor in the given direction
    /// </summary>
    /// <param name="direction">Normalized direction vector</param>
    /// <param name="force">Force magnitude to apply</param>
    public void AddForce(Vector2 direction, float force)
    {
        _rigidbody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Shifts the Anchor to the left
    /// </summary>
    public void AddLeftForce(float force)
    {
        AddForce(Vector2.left, force);
    }

    /// <summary>
    /// Shifts the Anchor to the right
    /// </summary>
    public void AddRightForce(float force)
    {
        AddForce(Vector2.right, force);
    }

    /// <summary>
    /// Shifts the Anchor upward
    /// </summary>
    public void AddUpForce(float force)
    {
        AddForce(Vector2.up, force);
    }

    /// <summary>
    /// Shifts the Anchor downward
    /// </summary>
    public void AddDownForce(float force)
    {
        AddForce(Vector2.down, force);
    }
}
