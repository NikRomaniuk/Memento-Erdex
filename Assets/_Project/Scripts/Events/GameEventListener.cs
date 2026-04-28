using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class GameEventListener : MonoBehaviour
{
    [Serializable]
    public class Binding // Binding is one-to-many relationship between GameEvent and UnityEvents
    {
        [SerializeField] private GameEvent _event;
        [SerializeField] private UnityEvent _responce;

        public GameEvent Event => _event;
        public UnityEvent Responce => _responce;
    }

    [Header("Properties")]
    [SerializeField] private Binding[] _bindings = Array.Empty<Binding>();

    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private void OnEnable()
    {
        RegisterAllBindings();
    }

    private void OnDisable()
    {
        UnregisterAllBindings();
    }

    private void OnDestroy()
    {
        UnregisterAllBindings();
    }

    public void OnEventInvoked(GameEvent invokedEvent)
    {
        if (invokedEvent == null) { return; }

        bool hasMatchingBinding = false;
        bool hasAnyInvokedResponse = false;

        for (int i = 0; i < _bindings.Length; i++)
        {
            Binding binding = _bindings[i];
            if (binding == null || binding.Event != invokedEvent) // No match -> Skip
            {
                continue;
            }

            hasMatchingBinding = true;

            UnityEvent response = binding.Responce;
            if (response == null) // No Response -> Skip
            {
                continue;
            }

            response.Invoke();
            hasAnyInvokedResponse = true;
        }

        if (hasMatchingBinding && !hasAnyInvokedResponse)
        {
            D($"Event '{invokedEvent.name}' matched, but no responses were invoked");
            return;
        }

        if (!hasMatchingBinding)
        {
            D($"No bindings matched event '{invokedEvent.name}'");
        }
    }

    private void RegisterAllBindings()
    {
        for (int i = 0; i < _bindings.Length; i++)
        {
            Binding binding = _bindings[i];
            if (binding == null || binding.Event == null)
            {
                continue;
            }

            binding.Event.RegisterListener(this);
        }
    }

    private void UnregisterAllBindings()
    {
        for (int i = 0; i < _bindings.Length; i++)
        {
            Binding binding = _bindings[i];
            if (binding == null || binding.Event == null)
            {
                continue;
            }

            binding.Event.UnregisterListener(this);
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
        Debug.LogWarning($"[GameEventListener:{name}] {message}", this);
    }
}