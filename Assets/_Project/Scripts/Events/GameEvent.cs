using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Events/Game Event")]
public class GameEvent : ScriptableObject
{
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;
    private readonly List<IGameEventListener_Void> _listeners = new List<IGameEventListener_Void>();

    public void RegisterListener(IGameEventListener_Void listener)
    {
        if (listener == null) { return; }

        if (_listeners.Contains(listener))
        {
            D("Register ignored: Listener is already subscribed");
            return;
        }

        _listeners.Add(listener);
        D($"Registered listener. Total listeners: {_listeners.Count}");
    }

    public void UnregisterListener(IGameEventListener_Void listener)
    {
        if (listener == null) { return; }

        if (!_listeners.Remove(listener))
        {
            D("Unregister ignored: Listener is not subscribed");
            return;
        }

        D($"Unregistered listener. Total listeners: {_listeners.Count}");
    }

    public void Invoke()
    {
        if (_listeners.Count == 0)
        {
            D("Invoke called, but no listeners are registered");
            return;
        }

        D($"Invoking event for {_listeners.Count} listener(s)");

        // Snapshot protects iteration if listeners unsubscribe while handling this event
        List<IGameEventListener_Void> listenersSnapshot = new List<IGameEventListener_Void>(_listeners);
        for (int i = 0; i < listenersSnapshot.Count; i++)
        {
            IGameEventListener_Void listener = listenersSnapshot[i];
            if (listener == null) { continue; } // Unregistered during invocation -> Skip

            listener.OnEventInvoked(); // Notify listener
        }
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[GameEvent:{name}] {message}", this);
    }
}