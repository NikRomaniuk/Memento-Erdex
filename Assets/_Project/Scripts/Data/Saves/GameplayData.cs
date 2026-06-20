using UnityEngine;

[CreateAssetMenu(fileName = "NewGameplay", menuName = "Saves/GameplayProfile")]
public class GameplayData : ScriptableObject, ISaveProfile<GameplaySave>
{
	[Header("Data")]
	[SerializeField] private Reference_Bool _isFreeCamMode = new Reference_Bool();

	[Header("Events")]
	[SerializeField] private GameEvent _onDataChanged;

	public Reference_Bool IsFreeCamMode => _isFreeCamMode;

	private bool _ignoreNotifications;

	private void OnEnable()
	{
		_isFreeCamMode.SubscribeToSource(OnReferenceValueChanged);
	}

	private void OnReferenceValueChanged(bool _)
	{
		if (!_ignoreNotifications)
			InvokeDataChanged();
	}

	// ================
	// Getters & Setters
	// ================

	// --- Free Cam Mode ---
	public bool GetFreeCamMode() => _isFreeCamMode.Value;
	public void SetFreeCamMode(bool value)
	{
		if (_isFreeCamMode.Value == value) { return; }

		_isFreeCamMode.SetValue(value); // OnValueChanged → InvokeDataChanged
	}

	// ================
	// Utility Functions
	// ================

	public GameplaySave ExtractSaveData()
	{
		return new GameplaySave
		{
			//IsFreeCamMode = _isFreeCamMode.Value,
		};
	}

	public void ApplySaveData(GameplaySave source, bool notifyChange = true)
	{
		if (source == null) { return; }

		bool hasChanges = false;
		_ignoreNotifications = true; // Prevent OnValueChanged from firing during copying

		// --- Free Cam Mode ---
		/*if (_isFreeCamMode.Value != source.IsFreeCamMode)
		{
			_isFreeCamMode.SetValue(source.IsFreeCamMode);
			hasChanges = true;
		}*/

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