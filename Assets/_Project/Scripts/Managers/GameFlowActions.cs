using UnityEngine;

public class GameFlowActions : MonoBehaviour
{
    [SerializeField] private string _gameplaySceneName = "TreeFootStage";

    public void GoToBootstrap()
    {
        if (GameFlowController.Instance == null) { return; }

        GameFlowController.Instance.GoToBootstrap();
    }

    public void GoToMainMenu()
    {
        if (GameFlowController.Instance == null) { return; }

        GameFlowController.Instance.GoToMainMenu();
    }

    public void StartDefaultGameplay()
    {
        if (GameFlowController.Instance == null) { return; }

        GameFlowController.Instance.StartGameplay();
    }

    public void StartConfiguredGameplay() 
    {
        if (GameFlowController.Instance == null) { return; }

        GameFlowController.Instance.StartGameplay(_gameplaySceneName);
    }

    public void EnterGameOver()
    {
        if (GameFlowController.Instance == null) { return; }

        GameFlowController.Instance.EnterGameOver();
    }

    public void RestartCurrentGameplay()
    {
        if (GameFlowController.Instance == null) { return; }

        GameFlowController.Instance.RestartCurrentGameplay();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
