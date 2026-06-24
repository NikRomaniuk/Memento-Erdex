using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-100)]
public class SceneTransitionManager : MonoBehaviour
{
    // --- Singleton ---
    public static SceneTransitionManager Instance { get; private set; }

    // --- Events ---
    [SerializeField] private GameEvent _onTransitionComplete;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private bool _isTransitionInProgress;
    private readonly List<Func<UniTask>> _preTransitionTasks = new();
    private readonly List<Func<UniTask>> _preActivationTasks = new();

    public bool IsTransitionInProgress => _isTransitionInProgress;

    // ========
    // LIFECYCLE
    // ========

    private void Awake()
    {
        // Force Singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SceneTransitionManager] Duplicate instance destroyed", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Register default pre-transition task:
        // - Save Saves (To Disk)
        RegisterPreTransitionTask(SaveSavesTask);

        // Register default pre-activation task:
        // - Load Saves (From Disk)
        RegisterPreActivationTask(LoadSavesTask);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Performs an async scene transition.
    /// Scene loading begins immediately but activation is deferred until
    /// all registered pre-activation tasks complete
    /// </summary>
    public async UniTask TransitionToAsync(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SceneTransitionManager] Cannot transition: scene name is null or empty");
            return;
        }

        if (_isTransitionInProgress)
        {
            D($"Transition rejected. Already transitioning. Requested: {sceneName}");
            return;
        }

        _isTransitionInProgress = true;
        D($"Starting transition to '{sceneName}'");

        try
        {
            // --- Phase 1: Pre-Transition Tasks ---
            // Current scene is still fully alive

            D($"Running pre-transition tasks ({_preTransitionTasks.Count})...");
            await RunPreTransitionTasksAsync();
            D("Pre-transition tasks complete");

            // --- Phase 2: Async Scene Load ---
            // Start async scene load, but don't activate yet

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            if (asyncLoad == null)
            {
                Debug.LogError($"[SceneTransitionManager] Failed to start loading scene '{sceneName}'. " +
                               "Check that the scene exists and is added to Build Settings.");
                _isTransitionInProgress = false;
                return;
            }

            asyncLoad.allowSceneActivation = false;

            // Wait until scene is fully loaded (progress reaches 0.9)
            while (asyncLoad.progress < 0.9f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            D($"Scene '{sceneName}' loaded. Running pre-activation tasks ({_preActivationTasks.Count})...");

            // --- Phase 3: Pre-Activation Tasks ---
            // Run all pre-activation tasks sequentially

            await RunPreActivationTasksAsync();

            D($"Pre-activation tasks complete. Activating scene '{sceneName}'...");

            // Allow scene activation
            asyncLoad.allowSceneActivation = true;

            // Wait until activation is complete
            while (!asyncLoad.isDone)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            D($"Transition to '{sceneName}' complete");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[SceneTransitionManager] Transition to '{sceneName}' failed. {exception}");
        }
        finally
        {
            _isTransitionInProgress = false;
        }

        // Notify listeners
        _onTransitionComplete?.Invoke();
    }

    /// <summary>
    /// Registers a Task to be executed BEFORE the scene load begins.
    /// The current scene is still fully alive at this point —
    /// useful for saving, cleanup, exit animations, etc.
    /// </summary>
    public void RegisterPreTransitionTask(Func<UniTask> task)
    {
        if (task == null)
        {
            Debug.LogWarning("[SceneTransitionManager] Cannot register null pre-transition Task");
            return;
        }

        if (_preTransitionTasks.Contains(task))
        {
            D("Pre-transition Task already registered. Skipped");
            return;
        }

        _preTransitionTasks.Add(task);
        D($"Pre-transition Task registered. Total: {_preTransitionTasks.Count}");
    }

    /// <summary>
    /// Unregisters a previously registered pre-transition Task
    /// </summary>
    public void UnregisterPreTransitionTask(Func<UniTask> task)
    {
        if (task == null) { return; }

        _preTransitionTasks.Remove(task);
        D($"Pre-transition Task unregistered. Total: {_preTransitionTasks.Count}");
    }

    /// <summary>
    /// Registers a Task to be executed after the scene is loaded
    /// but before it is activated
    /// </summary>
    public void RegisterPreActivationTask(Func<UniTask> task)
    {
        if (task == null)
        {
            Debug.LogWarning("[SceneTransitionManager] Cannot register null Task");
            return;
        }

        if (_preActivationTasks.Contains(task))
        {
            D("Pre-activation Task already registered. Skipped");
            return;
        }

        _preActivationTasks.Add(task);
        D($"Pre-activation Task registered. Total: {_preActivationTasks.Count}");
    }

    /// <summary>
    /// Unregisters a previously registered pre-activation Task
    /// </summary>
    public void UnregisterPreActivationTask(Func<UniTask> task)
    {
        if (task == null) { return; }

        _preActivationTasks.Remove(task);
        D($"Pre-activation Task unregistered. Total: {_preActivationTasks.Count}");
    }

    // =============
    // HELPER METHODS
    // =============

    /// <summary>
    /// Default Pre-Transition Task:
    /// Save Saves (To Disk)
    /// </summary>
    private static async UniTask SaveSavesTask()
    {
        if (SaveController.Instance == null)
        {
            Debug.LogWarning("[SceneTransitionManager] SaveController.Instance is null. Skipping save.");
            return;
        }

        await SaveController.Instance.SaveNowAsync();
    }

    /// <summary>
    /// Default Pre-Activation Task:
    /// Load Saves (From Disk)
    /// </summary>
    private static async UniTask LoadSavesTask()
    {
        if (SaveController.Instance == null)
        {
            Debug.LogWarning("[SceneTransitionManager] SaveController.Instance is null. Skipping save load.");
            return;
        }

        await SaveController.Instance.LoadAsync();
    }

    private async UniTask RunPreTransitionTasksAsync()
    {
        var tasksSnapshot = new List<Func<UniTask>>(_preTransitionTasks);

        for (int i = 0; i < tasksSnapshot.Count; i++)
        {
            try
            {
                D($"Running pre-transition Task {i + 1}/{tasksSnapshot.Count}");
                await tasksSnapshot[i]();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SceneTransitionManager] Pre-transition Task {i + 1} failed. {exception}");
            }
        }
    }

    private async UniTask RunPreActivationTasksAsync()
    {
        // Snapshot to avoid issues if collection is modified during iteration
        var tasksSnapshot = new List<Func<UniTask>>(_preActivationTasks);

        for (int i = 0; i < tasksSnapshot.Count; i++)
        {
            try
            {
                D($"Running pre-activation Task {i + 1}/{tasksSnapshot.Count}");
                await tasksSnapshot[i]();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SceneTransitionManager] Pre-activation Task {i + 1} failed. {exception}");
            }
        }
    }

    // ====
    // Debug
    // ====

    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[SceneTransitionManager] {message}", this);
    }
}
