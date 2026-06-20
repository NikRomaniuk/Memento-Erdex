using UnityEngine;

[DefaultExecutionOrder(-500)]
public class StageInitializer : MonoBehaviour
{
    [SerializeField] private GameplayData _activeGameplay;
    [SerializeField] private GameplayData _defaultGameplay;

    void Awake()
    {
        // Reset FreeCamMode configuration to default
        _activeGameplay.IsFreeCamMode.SetValue(_defaultGameplay.IsFreeCamMode.Value);
    }
}
