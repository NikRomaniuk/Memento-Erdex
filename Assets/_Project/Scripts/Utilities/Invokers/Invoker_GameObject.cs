using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

/// <summary>
/// Fires UnityEvents on standard GameObject lifecycle events
/// </summary>
[DisallowMultipleComponent]
public class Invoker_GameObject : MonoBehaviour
{
    // ====
    // AWAKE
    // ====

    [FoldoutGroup("Awake")]
    [Tooltip("UnityEvent called on Awake")]
    [SerializeField] private UnityEvent _onAwake;

    // ====
    // START
    // ====

    [FoldoutGroup("Start")]
    [Tooltip("UnityEvent called on Start")]
    [SerializeField] private UnityEvent _onStart;

    // ========
    // ON ENABLE
    // ========

    [FoldoutGroup("OnEnable")]
    [Tooltip("UnityEvent called on OnEnable")]
    [SerializeField] private UnityEvent _onEnable;

    // =========
    // ON DISABLE
    // =========

    [FoldoutGroup("OnDisable")]
    [Tooltip("UnityEvent called on OnDisable")]
    [SerializeField] private UnityEvent _onDisable;

    // ========
    // UPDATE
    // ========

    [FoldoutGroup("Update")]
    [Tooltip("UnityEvent called every frame on Update")]
    [SerializeField] private UnityEvent _onUpdate;

    // =========
    // ON DESTROY
    // =========

    [FoldoutGroup("OnDestroy")]
    [Tooltip("UnityEvent called on OnDestroy")]
    [SerializeField] private UnityEvent _onDestroy;

    // ====
    // DEBUG
    // ====

    [FoldoutGroup("Debug")]
    [Tooltip("Turn on Debug Logging")]
    [SerializeField] private bool _debug;

    private string _lastDebug;

    // =====
    // FIRING
    // =====

    private void Awake()
    {
        Fire(_onAwake, nameof(Awake));
    }

    private void Start()
    {
        Fire(_onStart, nameof(Start));
    }

    private void OnEnable()
    {
        Fire(_onEnable, nameof(OnEnable));
    }

    private void OnDisable()
    {
        Fire(_onDisable, nameof(OnDisable));
    }

    private void Update()
    {
        Fire(_onUpdate, nameof(Update));
    }

    private void OnDestroy()
    {
        Fire(_onDestroy, nameof(OnDestroy));
    }

    // =============
    // HELPER METHODS
    // =============

    private void Fire(UnityEvent unityEvent, string eventName)
    {
        D(eventName);

        unityEvent?.Invoke();
    }

    private void D(string eventName)
    {
        if (!_debug) return;
        if (_lastDebug == eventName) return;

        _lastDebug = eventName;
        Debug.Log($"[Invoker (GameObject)]:{gameObject.name}] {eventName}", this);
    }
}
