using UnityEngine;
using UnityEngine.InputSystem;

public class BootstrapInitializer : MonoBehaviour
{
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    [SerializeField] private InputActionAsset _inputActions;

    private async void Start()
    {
        ApplyInputSchemeByPlatform();
        await SceneTransitionManager.Instance.TransitionToAsync(_mainMenuSceneName);
    }

    private void ApplyInputSchemeByPlatform()
    {
        if (_inputActions == null)
        {
            Debug.LogWarning("[BootstrapInitializer] InputActionAsset is not assigned. Skipping Scheme setup");
            return;
        }

        string schemeName;

#if UNITY_WEBGL && !UNITY_EDITOR
        schemeName = "Web";
#else
        schemeName = "Desktop";
#endif

        _inputActions.bindingMask = new InputBinding { groups = schemeName };
        Debug.Log($"[BootstrapInitializer] Input scheme set to '{schemeName}'");
    }
}
