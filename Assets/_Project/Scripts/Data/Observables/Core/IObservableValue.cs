using System;

namespace MementoErdex.Observables
{
    /// <summary>
    /// Observable variable — extends IVariable with a native OnValueChanged event.
    /// Allows FlexibleReference to subscribe to the underlying Observable directly.
    /// </summary>
    public interface IObservableValue<T> : IVariable<T>
    {
        event Action<T> OnValueChanged;
    }
}
