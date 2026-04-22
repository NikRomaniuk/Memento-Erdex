using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;
    private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

    public void RegisterListener(GameEventListener listener)
    {
        if (listener == null) { return; }

        if (_listeners.Contains(listener))
        {
            D($"Register ignored: Listener '{listener.name}' is already subscribed");
            return;
        }

        _listeners.Add(listener);
        D($"Registered listener '{listener.name}'. Total listeners: {_listeners.Count}");
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (listener == null) { return; }

        if (!_listeners.Remove(listener))
        {
            D($"Unregister ignored: Listener '{listener.name}' is not subscribed");
            return;
        }

        D($"Unregistered listener '{listener.name}'. Total listeners: {_listeners.Count}");
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
        List<GameEventListener> listenersSnapshot = new List<GameEventListener>(_listeners);
        for (int i = 0; i < listenersSnapshot.Count; i++)
        {
            GameEventListener listener = listenersSnapshot[i];
            if (listener == null) { continue; } // Unregistered during invocation -> Skip

            listener.OnEventInvoked(this); // Notify listener
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