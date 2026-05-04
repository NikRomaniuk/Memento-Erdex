using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LabelTextManager : MonoBehaviour
{
	public enum Type
	{
		None,
		Seed,
	}

	[SerializeField] private Type _type = Type.None;

	[ShowIf(nameof(_type), Type.Seed)]
	[SerializeField] private SettingsData _activeSettings;

	private TMP_Text _labelText;

	private void Awake()
	{
        _labelText = GetComponent<TMP_Text>();
	}

    private void OnEnable()
	{
        //SyncValue();
	}

	public void SyncValue()
	{
		if (_type == Type.None) { return; }

		if (_type == Type.Seed && _activeSettings != null)
        {
            _labelText.text = _activeSettings.GetSeed().ToString();
            return;
        }
	}
}