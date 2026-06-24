using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.Events;

/// <summary>
/// A single Input Block. Holds InputActionReference and
/// fires certain UnityEvents
/// on specific input phases (Pressed, Released, Held)
/// </summary>
[Serializable]
public class GlobalInputBlock
{
    [HorizontalGroup("Header")]
    [Tooltip("Numeric ID used by BlockInput() / UnblockInput()")]
    public int Id;

    [HorizontalGroup("Header")]
    [Tooltip("Name for debug identification")]
    [LabelText("Name")]
    public string BlockName;

    [FoldoutGroup("Data")]
    [Tooltip("Input Action to listen to")]
    public InputActionReference InputAction;

    [FoldoutGroup("Events")]
    [Tooltip("Fired once when input is pressed this frame")]
    public UnityEvent OnPressed;

    [FoldoutGroup("Events")]
    [Tooltip("Fired once when input is released this frame")]
    public UnityEvent OnReleased;

    [FoldoutGroup("Events")]
    [Tooltip("Fired every frame while input is held")]
    public UnityEvent OnHeld;
}

/// <summary>
/// Global Input Manager. Contains an array of Input Blocks
/// </summary>
[DisallowMultipleComponent]
public class GlobalInput : MonoBehaviour
{
    // ===========
    // INPUT BLOCKS
    // ===========

    [ListDrawerSettings(ShowFoldout = true)]
    [Tooltip("All Input Blocks")]
    [SerializeField] private List<GlobalInputBlock> _blocks = new List<GlobalInputBlock>();

    // ===========
    // BLOCKED ID's
    // ===========

    private readonly HashSet<int> _blockedIds = new HashSet<int>();

    // ====
    // DEBUG
    // ====

    [FoldoutGroup("Debug")]
    [Tooltip("Turn on Debug Logging")]
    [SerializeField] private bool _debug;

    private string _lastDebug;

    // ==============
    // UNITY LIFECYCLE
    // ==============

    private void OnEnable()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            GlobalInputBlock block = _blocks[i];
            if (block?.InputAction?.action == null) continue;

            block.InputAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            GlobalInputBlock block = _blocks[i];
            if (block?.InputAction?.action == null) continue;

            block.InputAction.action.Disable();
        }
    }

    private void Update()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            GlobalInputBlock block = _blocks[i];
            if (block == null) continue;
            if (block.InputAction?.action == null) continue;
            if (_blockedIds.Contains(block.Id)) continue;

            ProcessBlock(block, i);
        }
    }

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Blocks a specific Input Block by <paramref name="id"/>
    /// </summary>
    public void BlockInput(int id)
    {
        if (_blockedIds.Add(id))
            D($"BlockInput: id={id} blocked");
    }

    /// <summary>
    /// Unblocks a specific Input Block by <paramref name="id"/>
    /// </summary>
    public void UnblockInput(int id)
    {
        if (_blockedIds.Remove(id))
            D($"UnblockInput: id={id} unblocked");
    }

    /// <summary>
    /// Block all Input Blocks at once
    /// </summary>
    public void BlockAllInputs()
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            GlobalInputBlock block = _blocks[i];
            if (block != null)
                _blockedIds.Add(block.Id);
        }

        D("BlockAllInputs: all inputs blocked");
    }

    /// <summary>
    /// Unblock all Input Blocks at once
    /// </summary>
    public void UnblockAllInputs()
    {
        _blockedIds.Clear();
        D("UnblockAllInputs: all inputs unblocked");
    }

    public bool IsBlocked(int id) => _blockedIds.Contains(id);

    // =============
    // HELPER METHODS
    // =============

    private static void ProcessBlock(GlobalInputBlock block, int index)
    {
        InputAction action = block.InputAction?.action;

        // Read all phases in one frame

        if (action.WasPressedThisFrame())
        {
            block.OnPressed?.Invoke();
        }

        if (action.WasReleasedThisFrame())
        {
            block.OnReleased?.Invoke();
        }

        if (action.IsPressed())
        {
            block.OnHeld?.Invoke();
        }
    }

    private void D(string message)
    {
        if (!_debug) return;
        if (_lastDebug == message) return;

        _lastDebug = message;
        Debug.Log($"[GlobalInput:{gameObject.name}] {message}", this);
    }
}
