using System.IO;
using UnityEngine;

public static class SaveSystem
{
	private static string SettingsSavePath => Path.Combine(Application.persistentDataPath, "Settings.json");
	private static string GameplaySavePath => Path.Combine(Application.persistentDataPath, "Gameplay.json");

    /// <summary>
    /// Loads Settings from a JSON file without check
    /// </summary>
	public static SettingsSave LoadSettings()
	{
		TryLoadSettings(out SettingsSave settings);
		return settings;
	}

	/// <summary>
	/// Loads Gameplay from a JSON file without check
	/// </summary>
	public static GameplaySave LoadGameplay()
	{
		TryLoadGameplay(out GameplaySave gameplay);
		return gameplay;
	}

	/// <summary>
	/// Tries to load Settings from a JSON file
	/// </summary>
	public static bool TryLoadSettings(out SettingsSave settings)
	{
		return TryLoadFromFile(SettingsSavePath, "settings", out settings);
	}

	/// <summary>
	/// Tries to load Gameplay from a JSON file
	/// </summary>
	public static bool TryLoadGameplay(out GameplaySave gameplay)
	{
		return TryLoadFromFile(GameplaySavePath, "gameplay", out gameplay);
	}

    /// <summary>
    /// Saves provided Settings to a JSON file
    /// </summary>
	public static void SaveSettings(SettingsSave settings)
	{
		SaveToFile(SettingsSavePath, settings, "Settings");
	}

	/// <summary>
	/// Saves provided Gameplay to a JSON file
	/// </summary>
	public static void SaveGameplay(GameplaySave gameplay)
	{
		SaveToFile(GameplaySavePath, gameplay, "Gameplay");
	}

	private static bool TryLoadFromFile<T>(string savePath, string saveLabel, out T saveData) where T : new()
	{
		try
		{
			if (!File.Exists(savePath))
			{
				saveData = new T();
				return false;
			}

			string json = File.ReadAllText(savePath);
			if (string.IsNullOrWhiteSpace(json))
			{
				saveData = new T();
				return false;
			}

			saveData = JsonUtility.FromJson<T>(json) ?? new T();
			return true;
		}
		catch (System.Exception exception)
		{
			Debug.LogWarning($"[SaveSystem] Failed to load {saveLabel} from '{savePath}'. {exception.Message}");
			saveData = new T();
			return false;
		}
	}

	private static void SaveToFile<T>(string savePath, T saveData, string saveLabel) where T : new()
	{
		saveData ??= new T();

		string json = JsonUtility.ToJson(saveData, true);
		File.WriteAllText(savePath, json);
		Debug.Log($"{saveLabel} saved to: {savePath}");
	}


}
