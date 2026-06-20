using System;
using UnityEngine;
using MementoErdex.Observables;

[CreateAssetMenu(fileName = "Int_NewObservable", menuName = "Observables/Int")]
public class Observable_Int : ScriptableObject, IObservableValue<int>
{
    [SerializeField]
    private int _value;
    public event Action<int> OnValueChanged;

    public int Value
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
public class Reference_Int : FlexibleReference<int> { }
