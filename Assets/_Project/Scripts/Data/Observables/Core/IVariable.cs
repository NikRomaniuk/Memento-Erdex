namespace MementoErdex.Observables
{
    public interface IVariable<T>
    {
        T Value { get; set; }
    }
}