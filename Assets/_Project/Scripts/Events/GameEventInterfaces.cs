/// <summary>
/// Listener interface for parameterless GameEvent
/// </summary>
public interface IGameEventListener_Void
{
    void OnEventInvoked();
}

/// <summary>
/// Listener interface for parameterized GameEvent
/// </summary>
public interface IGameEventListener<T>
{
    void OnEventInvoked(T value);
}
