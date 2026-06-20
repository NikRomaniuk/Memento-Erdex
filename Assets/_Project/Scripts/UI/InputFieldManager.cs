using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldManager : MonoBehaviour
{
    public enum Type
    {
        None,
        Seed,
    }

    [SerializeField] private Type _type = Type.None;

    [ShowIf(nameof(_type), Type.Seed)]
    [SerializeField] private SettingsData _activeSettings;

    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        _inputField.onEndEdit.AddListener(OnEndEdit);
        SyncValue();
    }

    private void OnDisable()
    {
        _inputField.onEndEdit.RemoveListener(OnEndEdit);
    }

    private void OnEndEdit(string input)
    {
        if (_type == Type.None) { return; }

        // Set Seed value in Settings based on Input
        if (_type == Type.Seed && _activeSettings != null)
        {
            int seedValue = string.IsNullOrWhiteSpace(input)
                ? Random.Range(0, 1_000_000_000)
                : int.TryParse(input, out int parsedValue) ? parsedValue : Random.Range(0, 1_000_000_000);

            _activeSettings.SetSeed(seedValue);
            return;
        }

        SyncValue();
    }

    public void SyncValue()
	{
		if (_type == Type.None) { return; }

        // Sync Label Text with Seed value from Settings
		if (_type == Type.Seed && _activeSettings != null)
        {
            _inputField.text = _activeSettings.GetSeed().ToString();
            return;
        }
	}
}
