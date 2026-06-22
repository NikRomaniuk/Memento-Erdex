using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUIAudioEventChannel", menuName = "Audio/Event Channel/UI")]
public class UIAudioEventChannel : ScriptableObject
{
    // --- Debug ---
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // --- Events ---

    /// <summary> Called when a UI sound playback is requested </summary>
    public event Action<SoundData> OnAudioUIRrequested;

    /// <summary> Called when sound playback is completed </summary>
    public event Action<SoundData> OnAudioCompleted;

    // =================
    // ON AUDIO REQUESTED
    // =================

    public void Subscribe_Requested(Action<SoundData> handler)
    {
        if (handler == null) { return; }

        OnAudioUIRrequested += handler;
        D($"Subscribed to OnAudioUIRrequested. Handlers: {OnAudioUIRrequested?.GetInvocationList().Length}");
    }

    public void Unsubscribe_Requested(Action<SoundData> handler)
    {
        if (handler == null) { return; }

        OnAudioUIRrequested -= handler;
        D($"Unsubscribed from OnAudioUIRrequested. Handlers: {OnAudioUIRrequested?.GetInvocationList().Length ?? 0}");
    }

    public void RaiseRequested(SoundData data)
    {
        if (data == null)
        {
            Debug.LogWarning($"[UIAudioEventChannel:{name}] RaiseRequested called with null SoundData");
            return;
        }

        D($"RaiseRequested: '{data.name}'");
        OnAudioUIRrequested?.Invoke(data);
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
        Debug.Log($"[UIAudioEventChannel:{name}] {message}", this);
    }
}
