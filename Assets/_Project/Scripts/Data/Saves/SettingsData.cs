using UnityEngine;

[CreateAssetMenu(fileName = "NewSettings", menuName = "Saves/SettingsProfile")]
public class SettingsData : ScriptableObject, ISaveProfile<SettingsSave>
{
	[Header("Data")]
	[SerializeField] private Reference_Int _savedSeed = new Reference_Int();
	[SerializeField] private Reference_Int _treeHeight = new Reference_Int();

	[Header("Events")]
	[SerializeField] private GameEvent _onDataChanged;

	public Reference_Int SavedSeed => _savedSeed;
	public Reference_Int TreeHeight => _treeHeight;

	private bool _ignoreNotifications;

	private void OnEnable()
	{
		_savedSeed.SubscribeToSource(OnReferenceValueChanged);
		_treeHeight.SubscribeToSource(OnReferenceValueChanged);
	}

	private void OnReferenceValueChanged(int _)
	{
		if (!_ignoreNotifications)
			InvokeDataChanged();
	}

	// ================
	// Getters & Setters
	// ================

	// --- Seed ---
	public int GetSeed() => _savedSeed.Value;
	public void SetSeed(int value)
	{
		if (_savedSeed.Value == value) { return; }

		_savedSeed.SetValue(value); // OnValueChanged -> InvokeDataChanged
	}

	// --- Tree Height ---
	public int GetTreeHeight() => _treeHeight.Value;
	public void SetTreeHeight(int value)
	{
		if (_treeHeight.Value == value) { return; }

		_treeHeight.SetValue(value);
	}

	// ================
	// Utility Functions
	// ================

	public SettingsSave ExtractSaveData()
	{
		return new SettingsSave
		{
			SavedSeed = _savedSeed.Value,
			TreeHeight = _treeHeight.Value,
		};
	}

	public void ApplySaveData(SettingsSave source, bool notifyChange = true)
	{
		if (source == null) { return; }

		bool hasChanges = false;
		_ignoreNotifications = true; // Prevent OnValueChanged from firing during copying

		// --- Seed ---
		if (_savedSeed.Value != source.SavedSeed)
		{
			_savedSeed.SetValue(source.SavedSeed);
			hasChanges = true;
		}
		// --- Tree Height ---
		if (_treeHeight.Value != source.TreeHeight)
		{
			_treeHeight.SetValue(source.TreeHeight);
			hasChanges = true;
		}

		_ignoreNotifications = false;
		
		// Notify change if ANY data was changed
		if (hasChanges && notifyChange)
		{
			InvokeDataChanged();
		}
	}

	private void InvokeDataChanged()
	{
		_onDataChanged?.Invoke();
	}
}
