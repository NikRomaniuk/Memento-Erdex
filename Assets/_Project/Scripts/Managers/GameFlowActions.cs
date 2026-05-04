using UnityEngine;

public class GameFlowActions : MonoBehaviour
{
    [SerializeField] private GameFlowController _gameFlowController;

    public void GoToBootstrap()
    {
        _gameFlowController.GoToBootstrap();
    }

    public void GoToMainMenu()
    {
        _gameFlowController.GoToMainMenu();
    }

    public void StartGameplay() 
    {
        _gameFlowController.StartGameplay();
    }

    public void EnterGameOver()
    {
        _gameFlowController.EnterGameOver();
    }

    public void RestartGameplay()
    {
        _gameFlowController.RestartGameplay();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
