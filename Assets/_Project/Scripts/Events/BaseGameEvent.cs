using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base for parameterized GameEvents
/// </summary>
public abstract class BaseGameEvent<T> : ScriptableObject
{
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;
    private readonly List<IGameEventListener<T>> _listeners = new List<IGameEventListener<T>>();

    public void RegisterListener(IGameEventListener<T> listener)
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

    public void UnregisterListener(IGameEventListener<T> listener)
    {
        if (listener == null) { return; }

        if (!_listeners.Remove(listener))
        {
            D("Unregister ignored: Listener is not subscribed");
            return;
        }

        D($"Unregistered listener. Total listeners: {_listeners.Count}");
    }

    public void Invoke(T value)
    {
        if (_listeners.Count == 0)
        {
            D("Invoke called, but no listeners are registered");
            return;
        }

        D($"Invoking event for {_listeners.Count} listener(s)");

        // Snapshot protects iteration if listeners unsubscribe while handling this event
        List<IGameEventListener<T>> listenersSnapshot = new List<IGameEventListener<T>>(_listeners);
        for (int i = 0; i < listenersSnapshot.Count; i++)
        {
            IGameEventListener<T> listener = listenersSnapshot[i];
            if (listener == null) { continue; } // Unregistered during invocation -> Skip

            listener.OnEventInvoked(value);
        }
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[{GetType().Name}:{name}] {message}", this);
    }
}
