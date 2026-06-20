using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

/// <summary>
/// A single Condition Block. Holds a Reference_Bool and fires
/// certain UnityEvents (GameEvent) when evaluated
/// </summary>
[Serializable]
public class BoolConditionBlock
{
    [HorizontalGroup("Header")]
    [Tooltip("Numeric ID used by EvaluateById()")]
    public int Id;

    [HorizontalGroup("Header")]
    [Tooltip("Name used by EvaluateByName()")]
    [LabelText("Name")]
    public string BlockName;

    [FoldoutGroup("Data")]
    [Tooltip("Bool Source")]
    public Reference_Bool Operand;

    [FoldoutGroup("Events")]
    [Tooltip("Called when Operand.Value is True")]
    public UnityEvent OnTrue;

    [FoldoutGroup("Events")]
    [Tooltip("Called when Operand.Value is False")]
    public UnityEvent OnFalse;

    [FoldoutGroup("Events")]
    [Tooltip("Optional GameEvent fired on True")]
    public GameEvent OnTrueEvent;

    [FoldoutGroup("Events")]
    [Tooltip("Optional GameEvent fired on False")]
    public GameEvent OnFalseEvent;
}

/// <summary>
/// Custom Bool Comparator. Contains an array of Bool Condition Blocks.
/// Called from a UnityEvent
/// </summary>
[DisallowMultipleComponent]
public class Comparator_Bool : MonoBehaviour
{
    // ===============
    // CONDITION BLOCKS
    // ===============

    [ListDrawerSettings(ShowFoldout = true)]
    [Tooltip("All Condition Blocks")]
    [SerializeField] private List<BoolConditionBlock> _blocks = new List<BoolConditionBlock>();

    // ====
    // DEBUG
    // ====

    [FoldoutGroup("Debug")]
    [Tooltip("Turn on Debug Logging")]
    [SerializeField] private bool _debug;

    private string _lastDebug;

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Evaluate Condition Block at <paramref name="index"/>
    /// </summary>
    public void EvaluateByIndex(int index)
    {
        if (!TryGetBlock(index, out BoolConditionBlock block)) return;

        bool value = block.Operand.Value;
        D($"EvaluateByIndex [{index}] \"{block.BlockName}\": value={value}");

        FireBlockEvents(block, value);
    }

    /// <summary>
    /// Evaluate first matching Condition Block by <paramref name="blockName"/>
    /// </summary>
    public void EvaluateByName(string blockName)
    {
        if (string.IsNullOrEmpty(blockName))
        {
            D("EvaluateByName skipped: name is null or empty");
            return;
        }

        for (int i = 0; i < _blocks.Count; i++)
        {
            BoolConditionBlock block = _blocks[i];
            if (block == null) continue;

            if (string.Equals(block.BlockName, blockName, StringComparison.Ordinal))
            {
                bool value = block.Operand.Value;
                D($"EvaluateByName \"{blockName}\" at [{i}]: value={value}");

                FireBlockEvents(block, value);
                return;
            }
        }

        D($"EvaluateByName \"{blockName}\": no block found");
    }

    /// <summary>
    /// Evaluate first matching Condition Block by <paramref name="id"/>
    /// </summary>
    public void EvaluateById(int id)
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            BoolConditionBlock block = _blocks[i];
            if (block == null) continue;

            if (block.Id == id)
            {
                bool value = block.Operand.Value;
                D($"EvaluateById {id} at [{i}] \"{block.BlockName}\": value={value}");

                FireBlockEvents(block, value);
                return;
            }
        }

        D($"EvaluateById {id}: no block found");
    }

    // =============
    // HELPER METHODS
    // =============

    private bool TryGetBlock(int index, out BoolConditionBlock block)
    {
        block = null;

        if (_blocks == null || index < 0 || index >= _blocks.Count)
        {
            D($"EvaluateByIndex [{index}] out of range (count={(_blocks != null ? _blocks.Count : 0)})");
            return false;
        }

        block = _blocks[index];
        if (block == null)
        {
            D($"EvaluateByIndex [{index}]: block is null");
            return false;
        }

        return true;
    }

    private static void FireBlockEvents(BoolConditionBlock block, bool value)
    {
        if (value)
        {
            block.OnTrue?.Invoke();
            if (block.OnTrueEvent != null) block.OnTrueEvent.Invoke();
        }
        else
        {
            block.OnFalse?.Invoke();
            if (block.OnFalseEvent != null) block.OnFalseEvent.Invoke();
        }
    }

    private void D(string message)
    {
        if (!_debug) return;
        if (_lastDebug == message) return;

        _lastDebug = message;
        Debug.Log($"[Comparator_Bool:{gameObject.name}] {message}", this);
    }
}
