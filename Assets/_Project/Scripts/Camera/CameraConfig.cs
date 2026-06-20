using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraConfig", menuName = "Other/CameraProfile")]
public class CameraConfig : ScriptableObject
{
    [LabelText("Min Zoom Offset (Higher)")]
    [SerializeField] public Reference_Float MinZoomOffset;
    [LabelText("Max Zoom Offset (Lower)")]
    [SerializeField] public Reference_Float MaxZoomOffset;
}
