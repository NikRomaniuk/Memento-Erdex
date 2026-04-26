using UnityEngine;

[DisallowMultipleComponent]
public class GameEventSender : MonoBehaviour
{
    public void InvokeGameEvent(GameEvent gameEvent)
    {
        if (gameEvent == null) { return; }

        gameEvent.Invoke();
    }
}