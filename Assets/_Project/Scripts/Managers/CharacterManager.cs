using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] private GameEvent _characterFellEvent;
    [SerializeField] private float _fallBelowYThreshold = -25f;

    [Header("Debug")]
    [SerializeField] private bool _debug = true;
    private string _lastDebug;

    private bool _hasCharacterFellEventBeenSent;

    /// <summary>
    /// Checks every frame if Character fell below threshold
    /// </summary>
    private void Update()
    {
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
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[CharacterManager] {message}", this);
    }
}
