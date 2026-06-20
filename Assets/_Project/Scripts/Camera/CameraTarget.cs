using UnityEngine;
using Sirenix.OdinInspector;

public class CameraTarget : MonoBehaviour
{
    // --- References ---
    [AssetSelector]
    [SerializeField] private GameEvent_GameObject _changeCameraTargetEvent;

    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    /// <summary>
    /// Invokes the "Change Camera Target Event", passing this GameObject as the argument
    /// </summary>
    public void ChangeCameraTargetOnMe()
    {
        if (_changeCameraTargetEvent == null)
        {
            D("ChangeCameraTarget called with null event");
            return;
        }

        _changeCameraTargetEvent.Invoke(gameObject);
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[CameraTarget:{name}] {message}", this);
    }
}
