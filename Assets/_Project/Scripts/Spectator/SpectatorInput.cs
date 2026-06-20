using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorInput : MonoBehaviour
{
    // --- Input Actions ---
    [SerializeField] private InputActionReference _moveAction;

    // --- Public Properties ---
    public Vector2 MoveDirection { get; private set; }
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    // --- Input Block ---
    private bool _inputBlocked;

    private void OnEnable() => ToggleAction(true);
    private void OnDisable() => ToggleAction(false);

    void Update()
    {
        if (_inputBlocked)
        {
            ClearInputValues();
            return;
        }

        Vector2 rawMove = _moveAction.action.ReadValue<Vector2>();
        MoveDirection = Vector2.ClampMagnitude(rawMove, 1f);

        Horizontal = MoveDirection.x;
        Vertical = MoveDirection.y;
    }

    /// <summary>
    /// Enables or disables input reading
    /// </summary>
    public void SetInputBlock(bool shouldBlock)
    {
        _inputBlocked = shouldBlock;
        if (_inputBlocked) ClearInputValues();
    }

    private void ClearInputValues()
    {
        MoveDirection = Vector2.zero;
        Horizontal = 0f;
        Vertical = 0f;
    }

    private void ToggleAction(bool enable)
    {
        if (enable)
            _moveAction.action?.Enable();
        else
            _moveAction.action?.Disable();
    }
}
