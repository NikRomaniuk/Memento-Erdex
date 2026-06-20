using Unity.Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CinemachineFollow))]
public class CameraManager : MonoBehaviour
{
    // --- References ---
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private CinemachineCamera _cinemachine;
    [SerializeField] private CinemachineFollow _follow;
    [SerializeField] private CinemachineConfiner2D _confiner;
    [SerializeField] private CameraZoom _zoom;

    // Velocity-Based Damping
    [SerializeField, Min(0f)] private float _defaultDamping = 1f;
    [SerializeField, Min(0f)] private float _minDamping = 0.25f;
    [SerializeField, Min(0f)] private float _velocityThreshold = 3f;
    [SerializeField, Min(0f)] private float _velocityMaxRange = 15f;

    private void Update()
    {
        UpdateFollowDamping();
    }

    /// <summary>
    /// Scales follow damping based on the Character's linear velocity
    /// Below _velocityThreshold -> default damping (soft)
    /// Above _velocityThreshold -> damping decreases toward _minDamping (aggressive)
    /// </summary>
    private void UpdateFollowDamping()
    {
        float velocity = _characterManager.RB.linearVelocity.magnitude;

        float t = Mathf.InverseLerp(_velocityThreshold, _velocityThreshold + _velocityMaxRange, velocity);
        float damping = Mathf.Lerp(_defaultDamping, _minDamping, t);

        ApplyPositionDamping(damping);
    }

    /// <summary>
    /// Applies Position Damping value on all axes for Cinemachine Follow
    /// </summary>
    private void ApplyPositionDamping(float dampingValue)
    {
        Vector3 targetDamping = new Vector3(dampingValue, dampingValue, dampingValue);
        var trackerSettings = _follow.TrackerSettings;

        if (trackerSettings.PositionDamping == targetDamping)
        {
            return;
        }

        trackerSettings.PositionDamping = targetDamping;
        _follow.TrackerSettings = trackerSettings;
    }

    public void ClearCameraTarget()
    {
        _cinemachine.Follow = null;
    }

    public void ConfinerSetActive(bool active)
    {
        _confiner.enabled = active;
    }

    public void SetBounds(Collider2D newBounds)
    {
        if (newBounds == null)
        {
            Debug.LogWarning($"[CameraManager] SetBounds called with null bounds");
            return;
        }

        _confiner.BoundingShape2D = newBounds;
        _confiner.InvalidateBoundingShapeCache();
    }
}