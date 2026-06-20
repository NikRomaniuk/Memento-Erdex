using UnityEngine;
using Sirenix.OdinInspector;

public class CameraBounds : MonoBehaviour
{
    // --- References ---
    [AssetSelector]
    [SerializeField] private GameEvent_Collider2D _setCameraBoundsEvent;
    [LabelText("Collider 2D")]
    [SerializeField] private Collider2D _col;

    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // --- Cache ---
    private Vector3 _originalPosition;
    
    private void Awake()
    {
        if (_col == null)
        {
            D("Awake called with null collider");
            return;
        }

        _originalPosition = _col.transform.localPosition;
    }

    /// <summary>
    /// Sets Camera Bounds.
    /// Invokes "Set Camera Bounds Event" with "Collider 2D" as the argument
    /// </summary>
    public void SetCameraBounds()
    {
        if (_setCameraBoundsEvent == null)
        {
            D("SetCameraBounds called with null event");
            return;
        }

        if (_col == null)
        {
            D("SetCameraBounds called with null collider");
            return;
        }

        _setCameraBoundsEvent.Invoke(_col);
    }
    
    /// <summary>
    /// Sets Collider based on Height
    /// </summary>
    public void SetColliderOnHeight(float height)
    {
        if (_col == null)
        {
            D("SetColliderOnHeight called with null collider");
            return;
        }

        Vector3 newSize = _col.transform.localScale;
        Vector3 newPos = _originalPosition;

        newSize.y = height;
        newPos.y = _originalPosition.y + height / 2;
        
        _col.transform.localScale = newSize;
        _col.transform.localPosition = newPos;
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[CameraBounds:{name}] {message}", this);
    }
}
