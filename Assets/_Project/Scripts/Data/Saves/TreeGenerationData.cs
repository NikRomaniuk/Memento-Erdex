using UnityEngine;

[CreateAssetMenu(fileName = "NewTreeGeneration", menuName = "Saves/TreeGenerationProfile")]
public class TreeGenerationData : ScriptableObject, ISaveProfile<TreeGenerationSave>
{
	[Header("Data")]
	[SerializeField] private Reference_Bool _useRandomSeed = new Reference_Bool();
	[SerializeField] private Reference_Int _fixedSeed = new Reference_Int();
	[SerializeField] private Reference_Int _treeHeight = new Reference_Int();

	[Header("Events")]
	[SerializeField] private GameEvent _onDataChanged;

	public Reference_Bool UseRandomSeed => _useRandomSeed;
	public Reference_Int FixedSeed => _fixedSeed;
	public Reference_Int TreeHeight => _treeHeight;

	private bool _ignoreNotifications;

	private void OnEnable()
	{
		_useRandomSeed.SubscribeToSource(OnReferenceValueChangedBool);
		_fixedSeed.SubscribeToSource(OnReferenceValueChangedInt);
		_treeHeight.SubscribeToSource(OnReferenceValueChangedInt);
	}

	private void OnReferenceValueChangedBool(bool _)
	{
		if (!_ignoreNotifications)
			InvokeDataChanged();
	}

	private void OnReferenceValueChangedInt(int _)
	{
		if (!_ignoreNotifications)
			InvokeDataChanged();
	}

	// ================
	// Getters & Setters
	// ================

	// --- Use Random Seed ---
	public bool GetUseRandomSeed() => _useRandomSeed.Value;
	public void SetUseRandomSeed(bool value)
	{
		if (_useRandomSeed.Value == value) { return; }

		_useRandomSeed.SetValue(value); // OnValueChanged → InvokeDataChanged
	}

	// --- Fixed Seed ---
	public int GetFixedSeed() => _fixedSeed.Value;
	public void SetFixedSeed(int value)
	{
		if (_fixedSeed.Value == value) { return; }

		_fixedSeed.SetValue(value); // OnValueChanged → InvokeDataChanged
	}

	// --- Tree Height ---
	public int GetTreeHeight() => _treeHeight.Value;
	public void SetTreeHeight(int value)
	{
		if (_treeHeight.Value == value) { return; }

		_treeHeight.SetValue(value); // OnValueChanged → InvokeDataChanged
	}

	// ================
	// Utility Functions
	// ================

	public TreeGenerationSave ExtractSaveData()
	{
		return new TreeGenerationSave
		{
			UseRandomSeed = _useRandomSeed.Value,
			FixedSeed = _fixedSeed.Value,
			TreeHeight = _treeHeight.Value,
		};
	}

	public void ApplySaveData(TreeGenerationSave source, bool notifyChange = true)
	{
		if (source == null) { return; }

		bool hasChanges = false;
		_ignoreNotifications = true; // Prevent OnValueChanged from firing during copying

		// --- Use Random Seed ---
		if (_useRandomSeed.Value != source.UseRandomSeed)
		{
			_useRandomSeed.SetValue(source.UseRandomSeed);
			hasChanges = true;
		}

		// --- Fixed Seed ---
		if (_fixedSeed.Value != source.FixedSeed)
		{
			_fixedSeed.SetValue(source.FixedSeed);
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
