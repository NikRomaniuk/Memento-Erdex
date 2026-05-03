using UnityEngine;

public class GameFlowActions : MonoBehaviour
{
    [SerializeField] private string _gameplaySceneName = "TreeFootStage";
    [SerializeField] private GameFlowController _gameFlowController;

    public void GoToBootstrap()
    {
        _gameFlowController.GoToBootstrap();
    }

    public void GoToMainMenu()
    {
        _gameFlowController.GoToMainMenu();
    }

    public void StartDefaultGameplay()
    {
        _gameFlowController.StartGameplay();
    }

    public void StartConfiguredGameplay() 
    {
        _gameFlowController.StartGameplay(_gameplaySceneName);
    }

    public void EnterGameOver()
    {
        _gameFlowController.EnterGameOver();
    }

    public void RestartCurrentGameplay()
    {
        _gameFlowController.RestartCurrentGameplay();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
