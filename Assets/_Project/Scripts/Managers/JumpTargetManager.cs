using UnityEngine;

[DisallowMultipleComponent]
public class JumpTargetManager : MonoBehaviour
{
    private GameObject _jumpTarget;
    private bool _isVisible;
    private bool _hasTargetPosition;
    private Vector3 _targetPosition;

    /// <summary>
    /// Prepares to function
    /// Caches required components
    /// </summary>
    private void Awake()
    {
        _jumpTarget = gameObject;
    }

    /// <summary>
    /// Applies movement and visibility each frame
    /// </summary>
    private void Update()
    {
        Move();
        ApplyVisibility();
    }

    /// <summary>
    /// Shows or hides object
    /// </summary>
    public void Show(bool isVisible)
    {
        _isVisible = isVisible;
        ApplyVisibility();
    }

    /// <summary>
    /// Returns true when target can be used to Perform Jump
    /// </summary>
    public bool IsTargetValid()
    {
        return _isVisible && _hasTargetPosition;
    }

    /// <summary>
    /// Returns current cached target world position
    /// </summary>
    public Vector3 GetPosition()
    {
        return _targetPosition;
    }

    /// <summary>
    /// Sets object world position for next movement update
    /// </summary>
    public void SetPosition(Vector2 worldPosition)
    {
        if (!EnsureTarget()) { return; }

        Vector3 current = _jumpTarget.transform.position;
        _targetPosition = new Vector3(worldPosition.x, worldPosition.y, current.z);
        _hasTargetPosition = true;
    }

    /// <summary>
    /// Clears cached movement object position
    /// </summary>
    public void ClearTargetPosition()
    {
        _hasTargetPosition = false;
        _targetPosition = Vector3.zero;
    }

    /// <summary>
    /// Moves object and applies visibility state
    /// </summary>
    public void Move()
    {
        if (!EnsureTarget()) { return; }

        if (_hasTargetPosition)
        {
            _jumpTarget.transform.position = _targetPosition;
        }
    }

    /// <summary>
    /// Applies current visibility state to object
    /// </summary>
    private void ApplyVisibility()
    {
        if (!EnsureTarget()) { return; }

        if (_jumpTarget.activeSelf != _isVisible)
        {
            _jumpTarget.SetActive(_isVisible);
        }
    }

    /// <summary>
    /// Ensures object reference is assigned
    /// </summary>
    private bool EnsureTarget()
    {
        _jumpTarget ??= gameObject;
        return _jumpTarget != null;
    }
}
