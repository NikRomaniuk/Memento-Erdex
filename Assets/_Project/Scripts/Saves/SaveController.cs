using UnityEngine;

//[DefaultExecutionOrder(-900)]
[DisallowMultipleComponent]
public class SaveController : MonoBehaviour
{
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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        TickDirtySave();
        TickAutosave();
    }

    // =============
    // Main Functions
    // =============

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

    public void Save()
    {
        MarkDirty();
        D("Save marked Dirty");
    }

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
        SaveNow();
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
        SaveNow();
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
