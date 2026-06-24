using Cysharp.Threading.Tasks;
using UnityEngine;

//[DefaultExecutionOrder(-900)]
[DisallowMultipleComponent]
public class SaveController : MonoBehaviour
{
    // --- Singleton ---
    public static SaveController Instance { get; private set; }

    // --- Data References ---
    [SerializeField] private SettingsData _activeSettings;
    [SerializeField] private SettingsData _defaultSettings;
    [SerializeField] private GameplayData _activeGameplay;
    [SerializeField] private GameplayData _defaultGameplay;
    [SerializeField] private TreeGenerationData _activeTreeGeneration;
    [SerializeField] private TreeGenerationData _defaultTreeGeneration;

    // --- Config ---
    [SerializeField] private bool _autosaveEnabled = false;
    [SerializeField, Min(0f)] private float _autosaveIntervalSeconds = 180f;

    // --- Events ---
    [SerializeField] private GameEvent _onSavesLoaded;
    [SerializeField] private GameEvent _onDataChanged;
    [SerializeField] private GameEvent _onSaveTriggered;
    private bool _isDataChanged;
    private IGameEventListener_Void _dataChangedHandler;
    private IGameEventListener_Void _saveTriggeredHandler;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private float _autosaveTimer;

    // ========
    // LIFECYCLE
    // ========

    private void Awake()
    {
        // Force Singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SaveController] Duplicate instance destroyed.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to GameEvents
        _dataChangedHandler = new GameEventCallback(() => _isDataChanged = true);
        _saveTriggeredHandler = new GameEventCallback(OnSaveTriggered);
        _onDataChanged?.RegisterListener(_dataChangedHandler);
        _onSaveTriggered?.RegisterListener(_saveTriggeredHandler);
    }

    private void OnDestroy()
    {
        // Unsubscribe from GameEvents
        if (_onDataChanged != null && _dataChangedHandler != null)
            _onDataChanged.UnregisterListener(_dataChangedHandler);
        if (_onSaveTriggered != null && _saveTriggeredHandler != null)
            _onSaveTriggered.UnregisterListener(_saveTriggeredHandler);

        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        TickAutosave();
    }

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Synchronous load (backward compatibility).
    /// Prefer <see cref="LoadAsync"/> for new code
    /// </summary>
    public void Load()
    {
        bool anyLoaded = false;

        if (TryLoadProfile(_activeSettings, _defaultSettings,
            () => SaveSystem.TryLoadSettings(out var s) ? s : null,
            "Settings"))
            anyLoaded = true;

        if (TryLoadProfile(_activeGameplay, _defaultGameplay,
            () => SaveSystem.TryLoadGameplay(out var g) ? g : null,
            "Gameplay"))
            anyLoaded = true;

        if (TryLoadProfile(_activeTreeGeneration, _defaultTreeGeneration,
            () => SaveSystem.TryLoadTreeGeneration(out var t) ? t : null,
            "TreeGeneration"))
            anyLoaded = true;

        if (!anyLoaded) { return; }

        Save();
        _onSavesLoaded?.Invoke();
    }

    /// <summary>
    /// Asynchronously loads all save profiles from disk, applies them,
    /// saves a fresh copy, and invokes the SavesLoaded event
    /// </summary>
    public async UniTask LoadAsync()
    {
        bool anyLoaded = false;

        var (settingsOk, settingsData) = await SaveSystem.TryLoadSettingsAsync();
        if (TryApplyLoadedData(_activeSettings, _defaultSettings, settingsData, settingsOk, "Settings"))
            anyLoaded = true;

        var (gameplayOk, gameplayData) = await SaveSystem.TryLoadGameplayAsync();
        if (TryApplyLoadedData(_activeGameplay, _defaultGameplay, gameplayData, gameplayOk, "Gameplay"))
            anyLoaded = true;

        var (treeOk, treeData) = await SaveSystem.TryLoadTreeGenerationAsync();
        if (TryApplyLoadedData(_activeTreeGeneration, _defaultTreeGeneration, treeData, treeOk, "TreeGeneration"))
            anyLoaded = true;

        if (!anyLoaded) { return; }

        await SaveAsync();
        _onSavesLoaded?.Invoke();
    }

    public void Save()
    {
        bool anySaved = false;

        if (SaveProfile(_activeSettings, s => SaveSystem.SaveSettings(s), "Settings"))
            anySaved = true;

        if (SaveProfile(_activeGameplay, g => SaveSystem.SaveGameplay(g), "Gameplay"))
            anySaved = true;

        if (SaveProfile(_activeTreeGeneration, t => SaveSystem.SaveTreeGeneration(t), "TreeGeneration"))
            anySaved = true;

        if (!anySaved) { return; }

        D("All profiles saved");
    }

    /// <summary>
    /// Asynchronously saves all active profiles to disk immediately
    /// </summary>
    public async UniTask SaveAsync()
    {
        bool anySaved = false;

        if (_activeSettings != null)
        {
            await SaveSystem.SaveSettingsAsync(_activeSettings.ExtractSaveData());
            anySaved = true;
            D("Settings saved (async)");
        }
        else
        {
            D("Active Settings is missing");
        }

        if (_activeGameplay != null)
        {
            await SaveSystem.SaveGameplayAsync(_activeGameplay.ExtractSaveData());
            anySaved = true;
            D("Gameplay saved (async)");
        }
        else
        {
            D("Active Gameplay is missing");
        }

        if (_activeTreeGeneration != null)
        {
            await SaveSystem.SaveTreeGenerationAsync(_activeTreeGeneration.ExtractSaveData());
            anySaved = true;
            D("TreeGeneration saved (async)");
        }
        else
        {
            D("Active TreeGeneration is missing");
        }

        if (!anySaved) { return; }

        D("All profiles saved (async)");
    }

    // ================
    // Utility Functions
    // ================

    // --- Ticking ---

    private void TickAutosave()
    {
        if (!_autosaveEnabled)
        {
            _autosaveTimer = 0f;
            return;
        }

        _autosaveTimer += Time.unscaledDeltaTime;
        if (_autosaveTimer < _autosaveIntervalSeconds)
        {
            return;
        }

        _autosaveTimer = 0f;
        D("Autosave interval elapsed");
        SaveAsync().Forget();
    }

    // --- Generic Load / Save Helpers ---

    private bool TryLoadProfile<TSave>(
        ISaveProfile<TSave> active,
        ISaveProfile<TSave> defaultProfile,
        System.Func<TSave> tryLoad,
        string label) where TSave : class, new()
    {
        if (active == null)
        {
            D($"Active {label} is missing");
            return false;
        }

        TSave loaded = tryLoad();
        if (loaded != null)
        {
            active.ApplySaveData(loaded, false);
            D($"{label} loaded");
            return true;
        }

        if (defaultProfile != null)
        {
            active.ApplySaveData(defaultProfile.ExtractSaveData(), false);
            D($"{label} fallback loaded");
            return true;
        }

        D($"Default {label} Profile is missing");
        return false;
    }

    /// <summary>
    /// Applies loaded async data or falls back to defaults
    /// </summary>
    private bool TryApplyLoadedData<TSave>(
        ISaveProfile<TSave> active,
        ISaveProfile<TSave> defaultProfile,
        TSave loadedData,
        bool loadSuccess,
        string label) where TSave : class, new()
    {
        if (active == null)
        {
            D($"Active {label} is missing");
            return false;
        }

        if (loadSuccess)
        {
            active.ApplySaveData(loadedData, false);
            D($"{label} loaded (async)");
            return true;
        }

        if (defaultProfile != null)
        {
            active.ApplySaveData(defaultProfile.ExtractSaveData(), false);
            D($"{label} fallback loaded (async)");
            return true;
        }

        D($"Default {label} Profile is missing");
        return false;
    }

    private bool SaveProfile<TSave>(
        ISaveProfile<TSave> active,
        System.Action<TSave> saveFunc,
        string label) where TSave : new()
    {
        if (active == null)
        {
            D($"Active {label} is missing");
            return false;
        }

        saveFunc(active.ExtractSaveData());
        D($"{label} saved");
        return true;
    }

    // ========
    // CALLBACKS
    // ========

    private void OnSaveTriggered()
    {
        if (!_isDataChanged) { return; }

        Save();
        _isDataChanged = false;
        D("Save triggered via GameEvent");
    }

    /// <summary>
    /// Wrapper to subscribe <see cref="System.Action"/> to <see cref="GameEvent"/>
    /// </summary>
    private sealed class GameEventCallback : IGameEventListener_Void
    {
        private readonly System.Action _action;
        public GameEventCallback(System.Action action) => _action = action;
        public void OnEventInvoked() => _action?.Invoke();
    }

    // ====
    // Debug
    // ====

    /// <summary>
    /// Prints Debug messages
    /// </summary>
    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; } // Same message -> Skip (Used to avoid spamming)

        _lastDebug = message;
        Debug.Log($"[SaveController] {message}", this);
    }
}
