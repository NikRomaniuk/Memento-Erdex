using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSFXAudioEventChannel", menuName = "Audio/Event Channel/SFX")]
public class SFXAudioEventChannel : ScriptableObject
{
    // --- Debug ---
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // --- Events ---

    /// <summary> Called when an SFX sound playback is requested </summary>
    public event Action<SoundData, Vector3, Transform> OnAudioRequested;

    /// <summary> Called when sound playback is completed </summary>
    public event Action<SoundData> OnAudioCompleted;

    // =================
    // ON AUDIO REQUESTED
    // =================

    public void Subscribe_Requested(Action<SoundData, Vector3, Transform> handler)
    {
        if (handler == null) { return; }

        OnAudioRequested += handler;
        D($"Subscribed to OnAudioRequested. Handlers: {OnAudioRequested?.GetInvocationList().Length}");
    }

    public void Unsubscribe_Requested(Action<SoundData, Vector3, Transform> handler)
    {
        if (handler == null) { return; }

        OnAudioRequested -= handler;
        D($"Unsubscribed from OnAudioRequested. Handlers: {OnAudioRequested?.GetInvocationList().Length ?? 0}");
    }

    public void RaiseRequested(SoundData data, Vector3 position, Transform parent = null)
    {
        if (data == null)
        {
            Debug.LogWarning($"[SFXAudioEventChannel:{name}] RaiseRequested called with null SoundData");
            return;
        }

        D($"RaiseRequested: '{data.name}' at {position}, parent={parent?.name ?? "null"}");
        OnAudioRequested?.Invoke(data, position, parent);
    }

    // =================
    // ON AUDIO COMPLETED
    // =================

    public void Subscribe_Completed(Action<SoundData> handler)
    {
        if (handler == null) { return; }

        OnAudioCompleted += handler;
        D($"Subscribed to OnAudioCompleted. Handlers: {OnAudioCompleted?.GetInvocationList().Length}");
    }

    public void Unsubscribe_Completed(Action<SoundData> handler)
    {
        if (handler == null) { return; }

        OnAudioCompleted -= handler;
        D($"Unsubscribed from OnAudioCompleted. Handlers: {OnAudioCompleted?.GetInvocationList().Length ?? 0}");
    }

    public void RaiseCompleted(SoundData data)
    {
        if (data == null) { return; }

        D($"RaiseCompleted: '{data.name}'");
        OnAudioCompleted?.Invoke(data);
    }

    // ====
    // DEBUG
    // ====

    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[SFXAudioEventChannel:{name}] {message}", this);
    }
}
