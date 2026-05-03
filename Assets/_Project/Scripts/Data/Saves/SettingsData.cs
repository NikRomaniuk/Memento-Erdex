using UnityEngine;

[CreateAssetMenu(fileName = "NewSettings", menuName = "Saves/SettingsProfile")]
public class SettingsData : ScriptableObject
{
	[Header("Data")]
	[SerializeField] private SettingsSave _settings = new SettingsSave();

	[Header("Events")]
	[SerializeField] private GameEvent _onDataChanged;

    // ================
    // Getters & Setters
    // ================

    // --- Seed ---
	public int GetSeed()
	{
		EnsureSettingsCache();
		return _settings.Seed;
	}

	public void SetSeed(int value)
	{
		EnsureSettingsCache();

		if (_settings.Seed == value) { return; }

		_settings.Seed = value;
		InvokeDataChanged();
	}

    // ================
    // Utility Functions
    // ================

	public SettingsSave GetSettingsSave()
	{
		EnsureSettingsCache();

		return new SettingsSave
		{
			Seed = _settings.Seed,
		};
	}

	public void CopyFrom(SettingsSave source, bool notifyChange = true)
	{
        EnsureSettingsCache();
		if (source == null) { return; }

		bool hasChanges = false;

        // --- Per Property ---
		if (_settings.Seed != source.Seed)
		{
			_settings.Seed = source.Seed;
			hasChanges = true;
		}

		if (hasChanges && notifyChange)
		{
			InvokeDataChanged();
		}
	}

	private void InvokeDataChanged()
	{
		_onDataChanged?.Invoke();
	}

    // --- Checks ---

	private void EnsureSettingsCache()
	{
		_settings ??= new SettingsSave();
	}
}
