using UnityEngine;

[CreateAssetMenu(fileName = "NewGameplay", menuName = "Saves/GameplayProfile")]
public class GameplayData : ScriptableObject
{
	[Header("Data")]
	[SerializeField] private GameplaySave _gameplay = new GameplaySave();

	[Header("Events")]
	[SerializeField] private GameEvent _onDataChanged;

	// ================
	// Getters & Setters
	// ================

	// --- Seed ---
	public int GetSeed()
	{
		EnsureGameplayCache();
		return _gameplay.Seed;
	}

	public void SetSeed(int value)
	{
		EnsureGameplayCache();

		if (_gameplay.Seed == value) { return; }

		_gameplay.Seed = value;
		InvokeDataChanged();
	}

	// ================
	// Utility Functions
	// ================

	public GameplaySave GetGameplaySave()
	{
		EnsureGameplayCache();

		return new GameplaySave
		{
			Seed = _gameplay.Seed,
		};
	}

	public void CopyFrom(GameplaySave source, bool notifyChange = true)
	{
		EnsureGameplayCache();
		if (source == null) { return; }

		bool hasChanges = false;

		// --- Per Property ---
		if (_gameplay.Seed != source.Seed)
		{
			_gameplay.Seed = source.Seed;
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

	private void EnsureGameplayCache()
	{
		_gameplay ??= new GameplaySave();
	}
}