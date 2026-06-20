using System;
using UnityEngine;
using Sirenix.OdinInspector;
using MementoErdex.Observables;

[Serializable]
public abstract class FlexibleReference<T>
{
    public bool UseConstant = true;

    [ShowIf("$UseConstant"), LabelText("Value")]
    public T ConstantValue;

    [SerializeField, HideIf("$UseConstant"), InfoBox("@_observable != null ? _observable.ToString() : \"No Observable assigned\"")]
    private ScriptableObject _observable;

    /// <summary>
    /// Fired whenever SetValue() is called -> forward to _onDataChanged
    /// </summary>
    public event Action<T> OnValueChanged;

    public T Value
    {
        get
        {
            if (UseConstant || _observable == null) return ConstantValue;
            
            if (_observable is IVariable<T> varInterface) return varInterface.Value;

            return ConstantValue;
        }
    }

    public void SetValue(T value)
    {
        if (UseConstant || _observable == null) // UseConstant? -> Write to ConstantValue
        {
            ConstantValue = value;
            OnValueChanged?.Invoke(value);
            return;
        }

        if (_observable is IVariable<T> mutable) // HasObservable? -> Write to Observable
        {
            mutable.Value = value;
            OnValueChanged?.Invoke(value);
            return;
        }

        ConstantValue = value; // Fallback: Write to ConstantValue
        OnValueChanged?.Invoke(value);
    }

    public static implicit operator T(FlexibleReference<T> reference)
    {
        return reference != null ? reference.Value : default;
    }

    /// <summary>
    /// Subscribes handler to both SetValue() and the underlying Observable's native OnValueChanged.
    /// Safe to call multiple times — auto-unsubscribes first to prevent duplicates.
    /// </summary>
    public void SubscribeToSource(Action<T> handler)
    {
        UnsubscribeFromSource(handler);

        OnValueChanged += handler;

        if (!UseConstant && _observable is IObservableValue<T> obs)
            obs.OnValueChanged += handler;
    }

    /// <summary>
    /// Unsubscribes handler from both SetValue() and the underlying Observable
    /// </summary>
    public void UnsubscribeFromSource(Action<T> handler)
    {
        OnValueChanged -= handler;

        if (!UseConstant && _observable is IObservableValue<T> obs)
            obs.OnValueChanged -= handler;
    }
}