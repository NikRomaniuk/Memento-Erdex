using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CinemachineFollow))]
public class CameraManager : MonoBehaviour
{
    public enum FollowType
    {
        Soft,
        Aggressive
    }

    [System.Serializable]
    private struct FollowDampingConfig
    {
        public FollowType FollowType;
        [Min(0f)] public float Damping;
    }

    [Header("References")]
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private CinemachineFollow _cinemachineFollow;

    [Header("Follow Damping")]
    [SerializeField] private FollowDampingConfig[] _followDampingConfigs =
    {
        new FollowDampingConfig { FollowType = FollowType.Soft, Damping = 1f },
        new FollowDampingConfig { FollowType = FollowType.Aggressive, Damping = 0.25f }
    };

    private bool _isAggressiveFollowActive;
    private readonly Dictionary<FollowType, float> _followDampingByType = new();

    private void Awake()
    {
        _cinemachineFollow ??= GetComponent<CinemachineFollow>();
        BuildFollowDampingDictionary();

        if (_characterMovement == null)
        {
            _characterMovement = FindFirstObjectByType<CharacterMovement>();
        }
    }

    private void OnEnable()
    {
        UpdateFollowMode(forceUpdate: true);
    }

    private void Update()
    {
        UpdateFollowMode(forceUpdate: false);
    }

    /// <summary>
    /// Sets Soft Follow: Regular movement
    /// </summary>
    public void SetSoftFollow()
    {
        _isAggressiveFollowActive = false;
        ApplyPositionDamping(GetDamping(FollowType.Soft));
    }

    /// <summary>
    /// Sets Aggressive Follow: Falling state
    /// </summary>
    public void SetAggressiveFollow()
    {
        _isAggressiveFollowActive = true;
        ApplyPositionDamping(GetDamping(FollowType.Aggressive));
    }

    /// <summary>
    /// Switches Follow types based on Character state
    /// </summary>
    private void UpdateFollowMode(bool forceUpdate)
    {
        if (_characterMovement == null) { return; }

        bool shouldUseAggressiveFollow = _characterMovement.CurrentState == CharacterMovement.MovementState.Falling;
        if (!forceUpdate && shouldUseAggressiveFollow == _isAggressiveFollowActive)
        {
            return;
        }

        if (shouldUseAggressiveFollow)
        {
            SetAggressiveFollow();
            return;
        }

        SetSoftFollow();
    }

    /// <summary>
    /// Applies Position Damping value on all axes for Cinemachine Follow
    /// </summary>
    private void ApplyPositionDamping(float dampingValue)
    {
        Vector3 targetDamping = new Vector3(dampingValue, dampingValue, dampingValue);
        var trackerSettings = _cinemachineFollow.TrackerSettings;

        if (trackerSettings.PositionDamping == targetDamping)
        {
            return;
        }

        trackerSettings.PositionDamping = targetDamping;
        _cinemachineFollow.TrackerSettings = trackerSettings;
    }

    private void BuildFollowDampingDictionary()
    {
        _followDampingByType.Clear();

        if (_followDampingConfigs == null)
        {
            return;
        }

        for (int i = 0; i < _followDampingConfigs.Length; i++)
        {
            FollowDampingConfig config = _followDampingConfigs[i];
            _followDampingByType[config.FollowType] = Mathf.Max(0f, config.Damping);
        }
    }

    private float GetDamping(FollowType followType)
    {
        if (_followDampingByType.TryGetValue(followType, out float damping))
        {
            return damping;
        }

        // Fallback values
        return followType == FollowType.Aggressive ? 0.25f : 1f;
    }
}