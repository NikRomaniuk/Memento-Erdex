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

    // TODO: Make Generic
    public void Load()
    {
        bool hasLoadedAnySave = false;

        if (SaveSystem.TryLoadSettings(out SettingsSave loadedSettings))
        {
            if (HasActiveSettings())
            {
                _activeSettings.CopyFrom(loadedSettings, false);
                hasLoadedAnySave = true;
            }

            D("Settings loaded");
        }
        else if (HasDefaultSettings())
        {
            if (HasActiveSettings())
            {
                _activeSettings.CopyFrom(_defaultSettings.GetSettingsSave(), false);
                hasLoadedAnySave = true;
            }

            D("Settings fallback loaded");
        }

        if (SaveSystem.TryLoadGameplay(out GameplaySave loadedGameplay))
        {
            if (HasActiveGameplay())
            {
                _activeGameplay.CopyFrom(loadedGameplay, false);
                hasLoadedAnySave = true;
            }

            D("Gameplay loaded");
        }
        else if (HasDefaultGameplay())
        {
            if (HasActiveGameplay())
            {
                _activeGameplay.CopyFrom(_defaultGameplay.GetGameplaySave(), false);
                hasLoadedAnySave = true;
            }

            D("Gameplay fallback loaded");
        }

        if (!hasLoadedAnySave) { return; }

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
        bool hasSavedAny = false;

        if (HasActiveSettings())
        {
            SaveSystem.SaveSettings(_activeSettings.GetSettingsSave());
            hasSavedAny = true;
            D("Settings saved");
        }

        if (HasActiveGameplay())
        {
            SaveSystem.SaveGameplay(_activeGameplay.GetGameplaySave());
            hasSavedAny = true;
            D("Gameplay saved");
        }

        if (!hasSavedAny) { return; }

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

    // --- Checks ---
    private bool HasActiveSettings()
    {
        if (_activeSettings != null) { return true; }

        D("Active Settings is missing");
        return false;
    }

    private bool HasDefaultSettings()
    {
        if (_defaultSettings != null) { return true; }

        D("Default Settings Profile is missing");
        return false;
    }

    private bool HasActiveGameplay()
    {
        if (_activeGameplay != null) { return true; }

        D("Active Gameplay is missing");
        return false;
    }

    private bool HasDefaultGameplay()
    {
        if (_defaultGameplay != null) { return true; }

        D("Default Gameplay Profile is missing");
        return false;
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
