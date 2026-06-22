using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public enum SoundType
    {
        SFX,
        UI
    }

    // ============
    // CONFIGURATION
    // ============

    // --- Type ---
    [FoldoutGroup("Core"), EnumToggleButtons, HideLabel]
    [SerializeField] private SoundType _soundType = SoundType.SFX;

    // --- Core ---
    [FoldoutGroup("Core")]
    [SerializeField] private AudioClip _clip;

    [FoldoutGroup("Core")]
    [SerializeField] private AudioMixerGroup _mixerGroup;

    [FoldoutGroup("Core"), Range(0f, 1f)]
    [SerializeField] private float _volume = 1f;

    [FoldoutGroup("Core"), ValidateInput(nameof(ValidatePitch), "Pitch must be greater than 0")]
    [SerializeField] private float _pitch = 1f;

    [FoldoutGroup("Core"), Range(0f, 0.3f)]
    [SerializeField] private float _volumeVariance = 0f;

    [FoldoutGroup("Core"), Range(0f, 0.3f)]
    [SerializeField] private float _pitchVariance = 0f;

    // --- 3D Settings (SFX only) ---
    [FoldoutGroup("3D Settings"), Range(0f, 1f), ShowIf("@_soundType == SoundType.SFX")]
    [SerializeField] private float _spatialBlend = 0f;

    [FoldoutGroup("3D Settings"), Min(0f), ShowIf("@_soundType == SoundType.SFX")]
    [SerializeField] private float _minDistance = 1f;

    [FoldoutGroup("3D Settings"), Min(0f), ShowIf("@_soundType == SoundType.SFX")]
    [SerializeField] private float _maxDistance = 500f;

    // --- Voice Limit ---
    [FoldoutGroup("Voice Limit")]
    [SerializeField, Tooltip("Maximum number of simultaneous voices for this sound. 0 or less means unlimited")]
    private int _maxSimultaneousVoices = 0;

    // ===============
    // PUBLIC ACCESSORS
    // ===============

    public AudioClip Clip => _clip;
    public AudioMixerGroup MixerGroup => _mixerGroup;
    public SoundType Type => _soundType;
    public float Volume => _volume;
    public float Pitch => _pitch;
    public float VolumeVariance => _volumeVariance;
    public float PitchVariance => _pitchVariance;
    public float SpatialBlend => _spatialBlend;
    public float MinDistance => _minDistance;
    public float MaxDistance => _maxDistance;
    public int MaxSimultaneousVoices => _maxSimultaneousVoices;

    // =========
    // VALIDATION
    // =========

    private bool ValidatePitch(float value) => value > 0f;
}
