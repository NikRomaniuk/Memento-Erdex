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
    [SerializeField, Min(0f)] private float _saveDelaySeconds = 2f;
    [SerializeField] private bool _autosaveEnabled = false;
    [SerializeField, Min(0f)] private float _autosaveIntervalSeconds = 180f;

    // --- Events ---
    [SerializeField] private GameEvent _onSavesLoaded;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    private bool _isDirty;
    private float _saveDelayTimer;
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
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        TickDirtySave();
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

        SaveNow();
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

        await SaveNowAsync();
        _onSavesLoaded?.Invoke();
    }

    public void Save()
    {
        MarkDirty();
        D("Save marked Dirty");
    }

    /// <summary>
    /// Synchronous immediate save (backward compatibility).
    /// Prefer <see cref="SaveNowAsync"/> for new code
    /// </summary>
    public void SaveNow()
    {
        bool anySaved = false;

        if (SaveProfile(_activeSettings, s => SaveSystem.SaveSettings(s), "Settings"))
            anySaved = true;

        if (SaveProfile(_activeGameplay, g => SaveSystem.SaveGameplay(g), "Gameplay"))
            anySaved = true;

        if (SaveProfile(_activeTreeGeneration, t => SaveSystem.SaveTreeGeneration(t), "TreeGeneration"))
            anySaved = true;

        if (!anySaved) { return; }

        ClearDirtyState();
    }

    /// <summary>
    /// Asynchronously saves all active profiles to disk immediately
    /// </summary>
    public async UniTask SaveNowAsync()
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

        ClearDirtyState();
    }

    // ================
    // Utility Functions
    // ================

    // --- Ticking ---
    private void TickDirtySave()
    {
        if (!_isDirty) { return; }

        _saveDelayTimer -= Time.unscaledDeltaTime;
        if (_saveDelayTimer > 0f) { return; }

        D("Dirty Save delay elapsed");
        SaveNowAsync().Forget();
    }

    private void TickAutosave()
    {
        if (!_autosaveEnabled || !_isDirty) 
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
        SaveNowAsync().Forget();
    }

    // --- Dirty State ---

    private void MarkDirty()
    {
        _isDirty = true;
        _saveDelayTimer = _saveDelaySeconds;
        _autosaveTimer = 0f;
    }

    private void ClearDirtyState()
    {
        _isDirty = false;
        _saveDelayTimer = 0f;
        _autosaveTimer = 0f;
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
