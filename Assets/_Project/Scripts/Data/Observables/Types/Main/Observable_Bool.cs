using System;
using UnityEngine;
using MementoErdex.Observables;

[CreateAssetMenu(fileName = "Bool_NewObservable", menuName = "Observables/Bool")]
public class Observable_Bool : ScriptableObject, IObservableValue<bool>
{
    [SerializeField]
    private bool _value;
    public event Action<bool> OnValueChanged;

    public bool Value
    {
        get => _value;
        set
        {
            if (_value == value) { return; }

            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public override string ToString() => $"Current Value: {_value}";
}

[Serializable]
public class Reference_Bool : FlexibleReference<bool> { }
