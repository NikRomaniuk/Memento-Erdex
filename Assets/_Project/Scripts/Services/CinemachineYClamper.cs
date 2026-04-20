using Unity.Cinemachine;
using UnityEngine;

//[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Y Clamper")]
[DisallowMultipleComponent]
[ExecuteAlways]
[SaveDuringPlay]
public class CinemachineYClamper : CinemachineExtension
{
    [Header("Tree Bounds")]
    [SerializeField] private TreeGenerator _treeGenerator;
    [SerializeField] private float _bottomOffset = 0f;
    [SerializeField] private float _topOffset = 0f;

    [Header("Fallback Bounds")]
    [SerializeField] private bool _useFallbackWhenTreeMissing = true;
    [SerializeField] private float _fallbackMinY = 0f;
    [SerializeField] private float _fallbackMaxY = 100f;

    private void Reset()
    {
        EnsureOrderedFallbackBounds();
    }

    private void OnValidate()
    {
        EnsureOrderedFallbackBounds();
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize)
        {
            return;
        }

        if (!TryGetYBounds(out float minY, out float maxY))
        {
            return;
        }

        Vector3 correctedPosition = state.GetCorrectedPosition();
        float clampedY = Mathf.Clamp(correctedPosition.y, minY, maxY);

        if (Mathf.Approximately(clampedY, correctedPosition.y))
        {
            return;
        }

        state.PositionCorrection += new Vector3(0f, clampedY - correctedPosition.y, 0f);
    }

    private bool TryGetYBounds(out float minY, out float maxY)
    {
        if (_treeGenerator != null)
        {
            float treeBottomY = _treeGenerator.transform.position.y;
            minY = treeBottomY + _bottomOffset;
            maxY = treeBottomY + _treeGenerator.TreeHeight + _topOffset;
            EnsureOrderedBounds(ref minY, ref maxY);
            return true;
        }

        if (_useFallbackWhenTreeMissing)
        {
            minY = _fallbackMinY;
            maxY = _fallbackMaxY;
            EnsureOrderedBounds(ref minY, ref maxY);
            return true;
        }

        minY = 0f;
        maxY = 0f;
        return false;
    }

    private void EnsureOrderedFallbackBounds()
    {
        EnsureOrderedBounds(ref _fallbackMinY, ref _fallbackMaxY);
    }

    private static void EnsureOrderedBounds(ref float minValue, ref float maxValue)
    {
        if (maxValue < minValue)
        {
            (minValue, maxValue) = (maxValue, minValue);
        }
    }
}
