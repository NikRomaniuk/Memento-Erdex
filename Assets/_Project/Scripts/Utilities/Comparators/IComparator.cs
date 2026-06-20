namespace MementoErdex.Utilities
{
    /// <summary>
    /// Common interface for Custom Comparators
    /// </summary>
    public interface IComparator
    {
        bool Evaluate();
        string ConditionName { get; }
        bool HasValidData { get; }
    }
}
