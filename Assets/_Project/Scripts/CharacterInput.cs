using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    // --- Input Actions ---
    [SerializeField] private InputActionProperty moveAction;
    [SerializeField] private InputActionProperty jumpAction;
    [SerializeField] private InputActionProperty focusAction;

    // --- Public Properties ---
    public float Horizontal { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool JumpHeld { get; private set; }

    // --- Input Block ---
    private bool isInputBlocked;

    private void OnEnable() => ToggleActions(true);
    private void OnDisable() => ToggleActions(false);

    void Update()
    {
        if (isInputBlocked)
        {
            ClearInputValues();
            return;
        }

        Horizontal = moveAction.action.ReadValue<float>();
        JumpTriggered = jumpAction.action.WasPressedThisFrame();
        JumpHeld = jumpAction.action.IsPressed();
    }

    /// <summary>
    /// Disables/Enables Input reading
    /// </summary>
    public void SetInputBlock(bool shouldBlock)
    {
        isInputBlocked = shouldBlock;
        if (isInputBlocked) ClearInputValues();
    }

    private void ClearInputValues()
    {
        Horizontal = 0f;
        JumpTriggered = false;
        JumpHeld = false;
    }

    private void ToggleActions(bool enable)
    {
        if (enable)
        {
            moveAction.action?.Enable();
            jumpAction.action?.Enable();
            focusAction.action?.Enable();
        }
        else
        {
            moveAction.action?.Disable();
            jumpAction.action?.Disable();
            focusAction.action?.Disable();
        }
    }
}
