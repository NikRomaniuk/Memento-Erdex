using System;
using UnityEngine;
using MementoErdex.Observables;

[CreateAssetMenu(fileName = "Vector3_NewObservable", menuName = "Observables/Vector3")]
public class Observable_Vector3 : ScriptableObject, IObservableValue<Vector3>
{
    [SerializeField]
    private Vector3 _value;
    public event Action<Vector3> OnValueChanged;

    public Vector3 Value
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
public class Reference_Vector3 : FlexibleReference<Vector3> { }
