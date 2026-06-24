using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

//[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public class GameFlowController : MonoBehaviour
{
    public enum GameState
    {
        Bootstrap,
        MainMenu,
        Gameplay
    }

    [Header("Scenes")]
    [SerializeField] private string _bootstrapSceneName = "Bootstrap";
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    [SerializeField] private string _defaultGameplaySceneName = "TreeFootStage";

    [Header("Bootstrap")]
    [SerializeField, Min(0f)] private float _bootstrapRetryIntervalSeconds = 0.5f;

    // --- Events ---
    [SerializeField] private GameEvent _onGameBootstrap;
    [SerializeField] private GameEvent _onMenuEntered;

    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private GameState _currentState = GameState.Bootstrap;

    private bool _isSavesLoaded = false;
    private bool _isSceneTransitionInProgress;
    private GameState? _stateAfterSceneLoad;
    private float _bootstrapRetryTimer;

    public GameState CurrentState => _currentState;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Reset Flags
        _isSavesLoaded = false;

        SetState(GameState.Bootstrap); // Set inital State

        //_onGameBootstrap?.Invoke(); // Invoke Bootstrap Event
    }

    private void Start()
    {
        _onGameBootstrap?.Invoke(); // Invoke Bootstrap Event
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (_currentState == GameState.Bootstrap)
        {
            HandleBootstrapEntered();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        TickBootstrapRetry();
    }

    public void GoToBootstrap()
    {
        if (!CanLoadScene(GameState.Bootstrap, _bootstrapSceneName)) { return; }

        RequestSceneTransition(_bootstrapSceneName, GameState.Bootstrap);
    }

    public void GoToMainMenu()
    {
        if (!CanLoadScene(GameState.MainMenu, _mainMenuSceneName)) { return; }

        RequestSceneTransition(_mainMenuSceneName, GameState.MainMenu);
    }

    public void StartGameplay()
    {
        if (!CanLoadScene(GameState.Gameplay, _defaultGameplaySceneName)) { return; }

        RequestSceneTransition(_defaultGameplaySceneName, GameState.Gameplay);
    }

    public void RestartGameplay()
    {
        if (!CanLoadScene(GameState.Gameplay, _defaultGameplaySceneName)) { return; }
        
        RequestSceneTransition(_defaultGameplaySceneName, GameState.Gameplay);
    }

    public void FlagSavesLoaded(bool loaded)
    {
        _isSavesLoaded = loaded;
        D($"Saves Loaded: {loaded}");
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
                _onMenuEntered?.Invoke();
                D("Menu Entered");
                return;

            case GameState.Gameplay:
                SetState(GameState.Gameplay);
                return;

            default:
                SetState(targetState);
                return;
        }
    }

    private void HandleBootstrapEntered()
    {
        if (!_isSavesLoaded)
        {
            D("Bootstrap waiting for Saves to load...");
            _bootstrapRetryTimer = _bootstrapRetryIntervalSeconds;
            return;
        }

        if (string.IsNullOrWhiteSpace(_mainMenuSceneName))
        {
            Debug.LogError("[GameFlowController] MainMenu scene name is empty. Cannot continue from Bootstrap.");
            return;
        }

        GoToMainMenu();
    }

    private void TickBootstrapRetry()
    {
        if (_currentState != GameState.Bootstrap || _isSceneTransitionInProgress)
        {
            return;
        }

        if (_bootstrapRetryIntervalSeconds <= 0f)
        {
            HandleBootstrapEntered();
            return;
        }

        _bootstrapRetryTimer -= Time.unscaledDeltaTime;
        if (_bootstrapRetryTimer > 0f)
        {
            return;
        }

        _bootstrapRetryTimer = _bootstrapRetryIntervalSeconds;
        HandleBootstrapEntered();
    }

    private void RequestSceneTransition(string sceneName, GameState targetState)
    {
        SceneTransitionManager stm = SceneTransitionManager.Instance;

        if (stm != null && stm.IsTransitionInProgress)
        {
            D($"Scene transition ignored. Already loading another scene. Target: {sceneName}");
            return;
        }

        _isSceneTransitionInProgress = true;
        _stateAfterSceneLoad = targetState;

        D($"Loading scene '{sceneName}' for target state '{targetState}'");

        if (stm == null)
        {
            Debug.LogError("[GameFlowController] SceneTransitionManager.Instance is null. Fallback to sync load");
            FallbackLoadScene(sceneName);
            return;
        }

        stm.TransitionToAsync(sceneName).Forget();
    }

    private void FallbackLoadScene(string sceneName)
    {
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

    private bool CanLoadScene(GameState targetState, string sceneName)
    {
        if (_isSceneTransitionInProgress)
        {
            D($"Transition to {targetState} rejected. Scene transition is already in progress");
            return false;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"[GameFlowController] Transition to {targetState} rejected. Scene name is empty");
            return false;
        }

        return true;
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
