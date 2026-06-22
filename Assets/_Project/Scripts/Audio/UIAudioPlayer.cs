using UnityEngine;

/// <summary>
/// Allows UI elements to play sounds through the AudioManager
/// by raising events on the UIAudioEventChannel.
/// UI sounds are always 2D (no position or parent attachment)
/// </summary>
[DisallowMultipleComponent]
public class UIAudioPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("UI event channel used to request audio playback")]
    private UIAudioEventChannel _eventChannel;

    // --- Debug ---
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Plays a UI sound (always 2D, no position tracking).
    /// </summary>
    public void Play(SoundData soundData)
    {
        if (!ValidateCall(soundData)) { return; }

        D($"Play UI: '{soundData.name}'");
        _eventChannel.RaiseRequested(soundData);
    }

    // =========
    // VALIDATION
    // =========

    private bool ValidateCall(SoundData soundData)
    {
        if (_eventChannel == null)
        {
            Debug.LogWarning($"[UIAudioPlayer:{name}] EventChannel is not assigned", this);
            return false;
        }

        if (soundData == null)
        {
            Debug.LogWarning($"[UIAudioPlayer:{name}] SoundData is null", this);
            return false;
        }

        return true;
    }

    // ====
    // DEBUG
    // ====

    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[UIAudioPlayer:{name}] {message}", this);
    }
}
