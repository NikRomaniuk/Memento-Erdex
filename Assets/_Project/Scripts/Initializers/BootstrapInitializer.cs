using UnityEngine;

public class BootstrapInitializer : MonoBehaviour
{
    private async void Start()
    {
        await SceneTransitionManager.Instance.TransitionToAsync("MainMenu");
    }
}
