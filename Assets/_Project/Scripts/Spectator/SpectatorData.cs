using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpectatorProfile", menuName = "Profiles/Spectator")]
public class SpectatorData : ScriptableObject
{
    [TitleGroup("Movement")]
    [TabGroup("Movement/Speed", "Speed")] [LabelText("Move")]
    [SerializeField] public Reference_Float MoveSpeed;
    [TabGroup("Movement/Speed", "Speed")] [LabelText("Acceleration")]
    [SerializeField] public Reference_Float AccelerationSpeed;
    [TabGroup("Movement/Speed", "Speed")] [LabelText("Deceleration")]
    [SerializeField] public Reference_Float DecelerationSpeed;

}
