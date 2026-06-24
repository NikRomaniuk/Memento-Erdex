using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class SaveSystem
{
	private static string SettingsSavePath => Path.Combine(Application.persistentDataPath, "Settings.json");
	private static string GameplaySavePath => Path.Combine(Application.persistentDataPath, "Gameplay.json");
	private static string TreeGenerationSavePath => Path.Combine(Application.persistentDataPath, "TreeGeneration.json");

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
	/// Tries to load TreeGeneration from a JSON file
	/// </summary>
	public static bool TryLoadTreeGeneration(out TreeGenerationSave treeGeneration)
	{
		return TryLoadFromFile(TreeGenerationSavePath, "treeGeneration", out treeGeneration);
	}

	// =============
	// ASYNC VARIANTS
	// =============

	/// <summary>
	/// Loads Settings from a JSON file (Async)
	/// </summary>
	public static async UniTask<SettingsSave> LoadSettingsAsync()
	{
		var (loaded, data) = await TryLoadFromFileAsync<SettingsSave>(SettingsSavePath, "settings");
		return data;
	}

	/// <summary>
	/// Loads Gameplay from a JSON file (Async)
	/// </summary>
	public static async UniTask<GameplaySave> LoadGameplayAsync()
	{
		var (loaded, data) = await TryLoadFromFileAsync<GameplaySave>(GameplaySavePath, "gameplay");
		return data;
	}

	/// <summary>
	/// Loads TreeGeneration from a JSON file (Async)
	/// </summary>
	public static async UniTask<TreeGenerationSave> LoadTreeGenerationAsync()
	{
		var (loaded, data) = await TryLoadFromFileAsync<TreeGenerationSave>(TreeGenerationSavePath, "treeGeneration");
		return data;
	}

	/// <summary>
	/// Tries to load Settings from a JSON file (Async)
	/// </summary>
	public static async UniTask<(bool Success, SettingsSave Data)> TryLoadSettingsAsync()
	{
		return await TryLoadFromFileAsync<SettingsSave>(SettingsSavePath, "settings");
	}

	/// <summary>
	/// Tries to load Gameplay from a JSON file (Async)
	/// </summary>
	public static async UniTask<(bool Success, GameplaySave Data)> TryLoadGameplayAsync()
	{
		return await TryLoadFromFileAsync<GameplaySave>(GameplaySavePath, "gameplay");
	}

	/// <summary>
	/// Tries to load TreeGeneration from a JSON file (Async)
	/// </summary>
	public static async UniTask<(bool Success, TreeGenerationSave Data)> TryLoadTreeGenerationAsync()
	{
		return await TryLoadFromFileAsync<TreeGenerationSave>(TreeGenerationSavePath, "treeGeneration");
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

	/// <summary>
	/// Saves provided TreeGeneration to a JSON file
	/// </summary>
	public static void SaveTreeGeneration(TreeGenerationSave treeGeneration)
	{
		SaveToFile(TreeGenerationSavePath, treeGeneration, "TreeGeneration");
	}

	// =============
	// ASYNC VARIANTS
	// =============

	/// <summary>
	/// Saves provided Settings to a JSON file (Async)
	/// </summary>
	public static async UniTask SaveSettingsAsync(SettingsSave settings)
	{
		await SaveToFileAsync(SettingsSavePath, settings, "Settings");
	}

	/// <summary>
	/// Saves provided Gameplay to a JSON file (Async)
	/// </summary>
	public static async UniTask SaveGameplayAsync(GameplaySave gameplay)
	{
		await SaveToFileAsync(GameplaySavePath, gameplay, "Gameplay");
	}

	/// <summary>
	/// Saves provided TreeGeneration to a JSON file (Async)
	/// </summary>
	public static async UniTask SaveTreeGenerationAsync(TreeGenerationSave treeGeneration)
	{
		await SaveToFileAsync(TreeGenerationSavePath, treeGeneration, "TreeGeneration");
	}

	// =============
	// HELPER METHODS
	// =============

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

	private static async UniTask<(bool Success, T Data)> TryLoadFromFileAsync<T>(string savePath, string saveLabel) where T : new()
	{
		try
		{
			// Read file on ThreadPool to avoid blocking main thread
			var (exists, json) = await UniTask.RunOnThreadPool(() =>
			{
				if (!File.Exists(savePath))
					return (false, null);

				string text = File.ReadAllText(savePath);
				return (true, text);
			});

			if (!exists || string.IsNullOrWhiteSpace(json))
				return (false, new T());

			// JsonUtility must run on main thread
			T data = JsonUtility.FromJson<T>(json) ?? new T();
			return (true, data);
		}
		catch (System.Exception exception)
		{
			Debug.LogWarning($"[SaveSystem] Failed to load {saveLabel} from '{savePath}'. {exception.Message}");
			return (false, new T());
		}
	}

	private static void SaveToFile<T>(string savePath, T saveData, string saveLabel) where T : new()
	{
		saveData ??= new T();

		string json = JsonUtility.ToJson(saveData, true);
		File.WriteAllText(savePath, json);
		Debug.Log($"{saveLabel} saved to: {savePath}");
	}

	private static async UniTask SaveToFileAsync<T>(string savePath, T saveData, string saveLabel) where T : new()
	{
		saveData ??= new T();

		string json = JsonUtility.ToJson(saveData, true);

		await UniTask.RunOnThreadPool(() =>
		{
			File.WriteAllText(savePath, json);
		});

		Debug.Log($"{saveLabel} saved to: {savePath}");
	}
}
