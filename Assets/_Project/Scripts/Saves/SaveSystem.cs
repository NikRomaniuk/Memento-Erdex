using System.IO;
using UnityEngine;

public static class SaveSystem
{
	private static string SavePath => Path.Combine(Application.persistentDataPath, "Settings.json");

    /// <summary>
    /// Loads Settings from a JSON file without check
    /// </summary>
	public static SettingsSave LoadSettings()
	{
		TryLoadSettings(out SettingsSave settings);
		return settings;
	}

	/// <summary>
	/// Tries to load Settings from a JSON file
	/// </summary>
	public static bool TryLoadSettings(out SettingsSave settings)
	{
		try
		{
			if (!File.Exists(SavePath))
			{
				settings = new SettingsSave();
				return false;
			}

			string json = File.ReadAllText(SavePath);
			if (string.IsNullOrWhiteSpace(json))
			{
				settings = new SettingsSave();
				return false;
			}

			settings = JsonUtility.FromJson<SettingsSave>(json) ?? new SettingsSave();
			return true;
		}
		catch (System.Exception exception)
		{
			Debug.LogWarning($"[SaveSystem] Failed to load settings from '{SavePath}'. {exception.Message}");
			settings = new SettingsSave();
			return false;
		}
	}

    /// <summary>
    /// Saves provided Settings to a JSON file
    /// </summary>
	public static void SaveSettings(SettingsSave settings)
	{
		settings ??= new SettingsSave();

		string json = JsonUtility.ToJson(settings, true);
		File.WriteAllText(SavePath, json);
		Debug.Log($"Settings saved to: {SavePath}");
	}


}
