using UnityEngine;

/// <summary>
/// Allows an object to play SFX sounds through the AudioManager
/// by raising events on the SFXAudioEventChannel
/// </summary>
[DisallowMultipleComponent]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("SFX event channel used to request audio playback")]
    private SFXAudioEventChannel _eventChannel;

    // --- Debug ---
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Plays a sound at the position of this object
    /// </summary>
    public void Play(SoundData soundData)
    {
        if (!ValidateCall(soundData)) { return; }

        D($"Play: '{soundData.name}' at self position");
        _eventChannel.RaiseRequested(soundData, transform.position, null);
    }

    /// <summary>
    /// Plays a sound at the specified world position
    /// </summary>
    public void Play(SoundData soundData, Vector3 worldPosition)
    {
        if (!ValidateCall(soundData)) { return; }

        D($"Play: '{soundData.name}' at {worldPosition}");
        _eventChannel.RaiseRequested(soundData, worldPosition, null);
    }

    /// <summary>
    /// Plays a sound attached to this object.
    /// The sound will follow the object
    /// </summary>
    public void PlayAttached(SoundData soundData)
    {
        if (!ValidateCall(soundData)) { return; }

        D($"PlayAttached: '{soundData.name}' attached to {name}");
        _eventChannel.RaiseRequested(soundData, transform.position, transform);
    }

    // =========
    // VALIDATION
    // =========

    private bool ValidateCall(SoundData soundData)
    {
        if (_eventChannel == null)
        {
            Debug.LogWarning($"[AudioPlayer:{name}] EventChannel is not assigned", this);
            return false;
        }

        if (soundData == null)
        {
            Debug.LogWarning($"[AudioPlayer:{name}] SoundData is null", this);
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
        Debug.Log($"[AudioPlayer:{name}] {message}", this);
    }
}
