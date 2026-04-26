using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameFlowController : MonoBehaviour
{
    public enum GameState
    {
        Bootstrap,
        MainMenu,
        Gameplay,
        Paused,
        GameOver
    }

    [Header("Scenes")]
    [SerializeField] private string _bootstrapSceneName = "Bootstrap";
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    [SerializeField] private string _defaultGameplaySceneName = "TreeFootStage";

    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private static GameFlowController _instance;
    private static bool _isInstanceAutoCreated;

    private GameState _currentState = GameState.Bootstrap;
    private string _currentGameplaySceneName = string.Empty;

    private bool _isSceneTransitionInProgress;
    private GameState? _stateAfterSceneLoad;

    public static GameFlowController Instance => _instance;
    public GameState CurrentState => _currentState;
    public string CurrentGameplaySceneName => _currentGameplaySceneName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstance()
    {
        if (_instance != null)
        {
            return;
        }

        var root = new GameObject(nameof(GameFlowController));
        _instance = root.AddComponent<GameFlowController>();
        _isInstanceAutoCreated = true;
        DontDestroyOnLoad(root);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            if (_isInstanceAutoCreated)
            {
                Destroy(_instance.gameObject);
                _instance = this;
                _isInstanceAutoCreated = false;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _isInstanceAutoCreated = false;
        DontDestroyOnLoad(gameObject);

        BootstrapStateFromActiveScene();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void GoToBootstrap()
    {
        if (!CanLoadSceneForTargetState(GameState.Bootstrap, _bootstrapSceneName))
        {
            return;
        }

        RequestSceneTransition(_bootstrapSceneName, GameState.Bootstrap);
    }

    public void GoToMainMenu()
    {
        if (!CanLoadSceneForTargetState(GameState.MainMenu, _mainMenuSceneName))
        {
            return;
        }

        RequestSceneTransition(_mainMenuSceneName, GameState.MainMenu);
    }

    public void StartGameplay()
    {
        StartGameplay(_defaultGameplaySceneName);
    }

    public void StartGameplay(string sceneName)
    {
        string targetSceneName = string.IsNullOrWhiteSpace(sceneName) ? _defaultGameplaySceneName : sceneName;
        if (!CanLoadSceneForTargetState(GameState.Gameplay, targetSceneName))
        {
            return;
        }

        _currentGameplaySceneName = targetSceneName;
        RequestSceneTransition(targetSceneName, GameState.Gameplay);
    }

    public void EnterGameOver()
    {
        if (_currentState != GameState.Gameplay && _currentState != GameState.Paused)
        {
            D($"GameOver rejected. Current state: {_currentState}");
            return;
        }

        SetState(GameState.GameOver);
    }

    public void RestartCurrentGameplay()
    {
        string sceneName = string.IsNullOrWhiteSpace(_currentGameplaySceneName)
            ? _defaultGameplaySceneName
            : _currentGameplaySceneName;

        if (!CanLoadSceneForTargetState(GameState.Gameplay, sceneName))
        {
            return;
        }

        _currentGameplaySceneName = sceneName;
        RequestSceneTransition(sceneName, GameState.Gameplay);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isSceneTransitionInProgress = false;

        if (_stateAfterSceneLoad.HasValue)
        {
            GameState loadedTargetState = _stateAfterSceneLoad.Value;
            _stateAfterSceneLoad = null;

            FinalizeTransitionAfterSceneLoad(loadedTargetState, scene.name);
            return;
        }

        BootstrapStateFromSceneName(scene.name);
    }

    private void FinalizeTransitionAfterSceneLoad(GameState targetState, string loadedSceneName)
    {
        switch (targetState)
        {
            case GameState.Bootstrap:
                SetState(GameState.Bootstrap);
                HandleBootstrapEntered();
                return;

            case GameState.MainMenu:
                SetState(GameState.MainMenu);
                return;

            case GameState.Gameplay:
                _currentGameplaySceneName = loadedSceneName;
                SetState(GameState.Gameplay);
                return;

            default:
                SetState(targetState);
                return;
        }
    }

    private void HandleBootstrapEntered()
    {
        // Reserved for bootstrap service initialization.
        if (string.IsNullOrWhiteSpace(_mainMenuSceneName))
        {
            Debug.LogError("[GameFlowController] MainMenu scene name is empty. Cannot continue from Bootstrap.");
            return;
        }

        GoToMainMenu();
        //StartGameplay();
    }

    private void RequestSceneTransition(string sceneName, GameState targetState)
    {
        if (_isSceneTransitionInProgress)
        {
            D($"Scene transition ignored. Already loading another scene. Target: {sceneName}");
            return;
        }

        _isSceneTransitionInProgress = true;
        _stateAfterSceneLoad = targetState;

        D($"Loading scene '{sceneName}' for target state '{targetState}'");

        try
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        catch (Exception exception)
        {
            _isSceneTransitionInProgress = false;
            _stateAfterSceneLoad = null;
            Debug.LogError($"[GameFlowController] Failed to load scene '{sceneName}'. {exception.Message}");
        }
    }

    private bool CanLoadSceneForTargetState(GameState targetState, string sceneName)
    {
        if (_isSceneTransitionInProgress)
        {
            D($"Transition to {targetState} rejected. Scene transition is already in progress.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"[GameFlowController] Transition to {targetState} rejected. Scene name is empty.");
            return false;
        }

        return true;
    }

    private void BootstrapStateFromActiveScene()
    {
        BootstrapStateFromSceneName(SceneManager.GetActiveScene().name);
    }

    private void BootstrapStateFromSceneName(string sceneName)
    {
        if (IsScene(sceneName, _bootstrapSceneName))
        {
            SetState(GameState.Bootstrap);
            HandleBootstrapEntered();
            return;
        }

        if (IsScene(sceneName, _mainMenuSceneName))
        {
            SetState(GameState.MainMenu);
            return;
        }

        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            _currentGameplaySceneName = sceneName;
            SetState(GameState.Gameplay);
        }
    }

    private bool IsScene(string left, string right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    private void SetState(GameState nextState)
    {
        if (_currentState == nextState) { return; }

        D($"State: {_currentState} -> {nextState}");
        _currentState = nextState;
    }

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[GameFlowController] {message}");
    }

}
