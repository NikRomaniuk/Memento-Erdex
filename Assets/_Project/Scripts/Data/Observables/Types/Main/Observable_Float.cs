using System;
using UnityEngine;
using MementoErdex.Observables;

[CreateAssetMenu(fileName = "Float_NewObservable", menuName = "Observables/Float")]
public class Observable_Float : ScriptableObject, IObservableValue<float>
{
    [SerializeField]
    private float _value;
    public event Action<float> OnValueChanged;

    public float Value
    {
        get => _value;
        set
        {
            if (Mathf.Approximately(_value, value)) { return; }

            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public override string ToString() => $"Current Value: {_value}";
}

[Serializable]
public class Reference_Float : FlexibleReference<float> { }
