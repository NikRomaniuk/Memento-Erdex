using UnityEngine;

/// <summary>
/// Writes a Transform position to an Observable (Vector3)
/// </summary>
public class PositionTranslator : MonoBehaviour
{
    // --- References ---
    [SerializeField] private Transform _target;
    [SerializeField] private Observable_Vector3 _outputPosition;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Writes the target's current position to the output Observable_Vector3
    /// </summary>
    public void Translate()
    {
        if (_target == null)
        {
            D("Translate called but no target assigned");
            return;
        }

        if (_outputPosition == null)
        {
            D("Translate called but no output Observable_Vector3 assigned");
            return;
        }

        _outputPosition.Value = _target.position;
        D($"Translated position: {_target.position}");
    }

    /// <summary>
    /// Writes the target's local position to the output Observable_Vector3
    /// </summary>
    public void TranslateLocal()
    {
        if (_target == null)
        {
            D("TranslateLocal called but no target assigned");
            return;
        }

        if (_outputPosition == null)
        {
            D("TranslateLocal called but no output Observable_Vector3 assigned");
            return;
        }

        _outputPosition.Value = _target.localPosition;
        D($"Translated local position: {_target.localPosition}");
    }

    // --- Debug ---

    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[PositionTranslator:{name}] {message}", this);
    }
}
