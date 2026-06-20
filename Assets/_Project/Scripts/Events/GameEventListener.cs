using UnityEngine;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class GameEventListener : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeReference]
    [ListDrawerSettings(ShowIndexLabels = false)]
    private BaseBinding[] _bindings = System.Array.Empty<BaseBinding>();

    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private void OnEnable()
    {
        for (int i = 0; i < _bindings.Length; i++)
        {
            _bindings[i]?.Register();
        }

        D($"Enabled — {_bindings.Length} binding(s) registered");
    }

    private void OnDisable()
    {
        for (int i = 0; i < _bindings.Length; i++)
        {
            _bindings[i]?.Unregister();
        }

        D($"Disabled — {_bindings.Length} binding(s) unregistered");
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