using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Central audio manager that handles audio playback requests, voice limiting, and pooling of AudioSource components.
/// Uses UnityEngine.Pool for AudioSource pooling, subscribes to SFX and UI AudioEventChannels.
/// </summary>
[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    // --- Singleton ---
    public static AudioManager Instance { get; private set; }

    // --- Channels ---
    [Header("Channels")]
    [SerializeField, Tooltip("SFX event channel (3D sounds with position/parent)")]
    private SFXAudioEventChannel _sfxEventChannel;

    [SerializeField, Tooltip("UI event channel (2D sounds, no position/parent)")]
    private UIAudioEventChannel _uiEventChannel;

    // --- Pool ---
    [Header("Pool")]
    [SerializeField, Tooltip("Prefab with an AudioSource component for the pool")]
    private GameObject _audioSourcePrefab;

    [SerializeField, Min(1)]
    private int _defaultCapacity = 10;

    [SerializeField, Min(1)]
    private int _maxPoolSize = 50;

    [SerializeField, Tooltip("Parent Transform for unused pooled AudioSources")]
    private Transform _poolContainer;

    // --- Debug ---
    [Header("Debug")]
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    // --- Pool ---
    private ObjectPool<AudioSource> _pool;
    private Transform _runtimePoolContainer;

    // --- Voice Limiting ---
    /// <summary>
    /// Tracks the number of active voices for each SoundData to enforce limits.
    /// Key: SoundData, Value: Active voice count
    /// </summary>
    private readonly Dictionary<SoundData, int> _activeVoiceCounts = new Dictionary<SoundData, int>();

    // ========
    // LIFECYCLE
    // ========

    private void Awake()
    {
        // Singleton setup
        if (Instance != null)
        {
            Debug.LogWarning($"[AudioManager] Duplicate instance detected. Destroying '{name}'");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure pool container exists
        if (_poolContainer == null)
        {
            GameObject containerGo = new GameObject("AudioSourcePool");
            containerGo.transform.SetParent(transform);
            _runtimePoolContainer = containerGo.transform;
        }
        else
        {
            _runtimePoolContainer = _poolContainer;
        }

        InitializePool();
    }

    private void OnEnable()
    {
        if (_sfxEventChannel != null)
        {
            _sfxEventChannel.OnAudioRequested += HandleSFXAudioRequest;
            D("Subscribed to SFXAudioEventChannel");
        }

        if (_uiEventChannel != null)
        {
            _uiEventChannel.OnAudioUIRrequested += HandleUIAudioRequest;
            D("Subscribed to UIAudioEventChannel");
        }
    }

    private void OnDisable()
    {
        if (_sfxEventChannel != null)
        {
            _sfxEventChannel.OnAudioRequested -= HandleSFXAudioRequest;
            D("Unsubscribed from SFXAudioEventChannel");
        }

        if (_uiEventChannel != null)
        {
            _uiEventChannel.OnAudioUIRrequested -= HandleUIAudioRequest;
            D("Unsubscribed from UIAudioEventChannel");
        }
    }

    private void OnDestroy()
    {
        if (_pool != null)
        {
            // Cleanup: Destroy all pooled AudioSources
            _pool.Clear();
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnValidate()
    {
        if (_audioSourcePrefab != null && _audioSourcePrefab.GetComponent<AudioSource>() == null)
        {
            Debug.LogError(
                $"[AudioManager] Prefab '{_audioSourcePrefab.name}' does not contain an AudioSource component!",
                this);
        }
    }

    // ==================
    // POOL INITIALIZATION
    // ==================

    private void InitializePool()
    {
        _pool = new ObjectPool<AudioSource>(
            createFunc: CreateAudioSource,
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReleaseToPool,
            actionOnDestroy: DestroyAudioSource,
            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxPoolSize
        );

        D($"Pool initialized. DefaultCapacity={_defaultCapacity}, MaxSize={_maxPoolSize}");
    }

    private AudioSource CreateAudioSource()
    {
        if (_audioSourcePrefab == null)
        {
            Debug.LogError("[AudioManager] Cannot create AudioSource: _audioSourcePrefab is null!");
            return null;
        }

        GameObject go = Instantiate(_audioSourcePrefab, _runtimePoolContainer);
        go.name = $"{_audioSourcePrefab.name}_Pooled";

        AudioSource source = go.GetComponent<AudioSource>();
        source.playOnAwake = false;

        D($"Created new AudioSource: '{go.name}'");
        return source;
    }

    private void OnGetFromPool(AudioSource source)
    {
        if (source == null) { return; }

        source.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(AudioSource source)
    {
        if (source == null) { return; }

        source.Stop();
        source.clip = null;

        // Back to pool container
        source.transform.SetParent(_runtimePoolContainer);
        source.transform.localPosition = Vector3.zero;
        source.transform.localRotation = Quaternion.identity;

        source.gameObject.SetActive(false);
    }

    private void DestroyAudioSource(AudioSource source)
    {
        if (source != null)
        {
            Destroy(source.gameObject);
        }
    }

    // =====================
    // AUDIO REQUEST HANDLING
    // =====================

    /// <summary>
    /// Handles SFX audio playback requests with full 3D positioning and optional parent attachment.
    /// Enforces voice limits, retrieves an AudioSource from the pool, configures it, and plays the sound
    /// </summary>
    private void HandleSFXAudioRequest(SoundData data, Vector3 position, Transform parent)
    {
        if (data == null)
        {
            Debug.LogWarning("[AudioManager] Received request with null SoundData");
            return;
        }

        if (data.Clip == null)
        {
            Debug.LogWarning($"[AudioManager] SoundData '{data.name}' has no AudioClip assigned");
            return;
        }

        // --- Voice Limiting ---
        // Check if the maximum simultaneous voice limit has been exceeded for this SoundData
        int limit = data.MaxSimultaneousVoices;
        if (limit > 0)
        {
            int currentCount = _activeVoiceCounts.TryGetValue(data, out int count) ? count : 0;
            if (currentCount >= limit)
            {
                D($"Voice limit reached for '{data.name}' ({currentCount}/{limit}). Skipping.");
                return;
            }
        }

        // --- Get from pool ---
        AudioSource source = _pool.Get();
        if (source == null)
        {
            Debug.LogWarning("[AudioManager] Pool returned null AudioSource");
            return;
        }

        // --- Configure AudioSource ---
        ConfigureSource(source, data, position, parent);

        source.Play();

        // --- Track voice count ---
        _activeVoiceCounts.TryGetValue(data, out int voiceCount);
        _activeVoiceCounts[data] = voiceCount + 1;

        // --- Schedule return to pool ---
        float finalPitch = source.pitch;
        float duration = data.Clip.length / Mathf.Abs(finalPitch);
        StartCoroutine(ReturnToPoolRoutine(source, data, duration, parent != null, _sfxEventChannel.RaiseCompleted));

        D($"Playing '{data.Clip.name}' " +
           $"(vol={source.volume:F2}, pitch={finalPitch:F2}, " +
           $"voices={_activeVoiceCounts[data]}/{limit}, " +
           $"pool={_pool.CountActive}/{_pool.CountAll})");
    }

    /// <summary>
    /// Handles UI audio playback requests.
    /// UI sounds are always 2D (spatialBlend forced to 0) with no position tracking.
    /// Shares the same pool and voice limiting infrastructure as SFX sounds
    /// </summary>
    private void HandleUIAudioRequest(SoundData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[AudioManager] Received UI request with null SoundData");
            return;
        }

        if (data.Clip == null)
        {
            Debug.LogWarning($"[AudioManager] SoundData '{data.name}' has no AudioClip assigned");
            return;
        }

        // --- Voice Limiting ---
        int limit = data.MaxSimultaneousVoices;
        if (limit > 0)
        {
            int currentCount = _activeVoiceCounts.TryGetValue(data, out int count) ? count : 0;
            if (currentCount >= limit)
            {
                D($"Voice limit reached for '{data.name}' ({currentCount}/{limit}). Skipping.");
                return;
            }
        }

        // --- Get from pool ---
        AudioSource source = _pool.Get();
        if (source == null)
        {
            Debug.LogWarning("[AudioManager] Pool returned null AudioSource");
            return;
        }

        // --- Configure AudioSource (2D, no position/parent) ---
        ConfigureSource(source, data, Vector3.zero, null);

        // Force 2D for UI sounds
        source.spatialBlend = 0f;

        source.Play();

        // --- Track voice count ---
        _activeVoiceCounts.TryGetValue(data, out int voiceCount);
        _activeVoiceCounts[data] = voiceCount + 1;

        // --- Schedule return to pool ---
        float finalPitch = source.pitch;
        float duration = data.Clip.length / Mathf.Abs(finalPitch);
        StartCoroutine(ReturnToPoolRoutine(source, data, duration, false, _uiEventChannel.RaiseCompleted));

        D($"Playing UI '{data.Clip.name}' " +
           $"(vol={source.volume:F2}, pitch={finalPitch:F2}, " +
           $"voices={_activeVoiceCounts[data]}/{limit}, " +
           $"pool={_pool.CountActive}/{_pool.CountAll})");
    }

    /// <summary>
    /// Configures an AudioSource according to the SoundData settings.
    /// If a parent is provided, attaches to it and resets local coordinates.
    /// Otherwise places the source at the specified world position
    /// </summary>
    private void ConfigureSource(AudioSource source, SoundData data, Vector3 position, Transform parent)
    {
        Transform sourceTransform = source.transform;

        if (parent != null)
        {
            // Attach to parent so the sound follows the object
            sourceTransform.SetParent(parent);
            sourceTransform.localPosition = Vector3.zero;
            sourceTransform.localRotation = Quaternion.identity;
        }
        else
        {
            // Place at the world position
            sourceTransform.SetParent(null);
            sourceTransform.position = position;
        }

        // Apply settings with randomized volume and pitch
        float finalVolume = data.Volume + Random.Range(-data.VolumeVariance, data.VolumeVariance);
        finalVolume = Mathf.Clamp01(finalVolume);

        float finalPitch = data.Pitch + Random.Range(-data.PitchVariance, data.PitchVariance);
        finalPitch = Mathf.Max(0.01f, finalPitch);

        source.clip = data.Clip;
        source.volume = finalVolume;
        source.pitch = finalPitch;
        source.spatialBlend = data.SpatialBlend;
        source.minDistance = data.MinDistance;
        source.maxDistance = data.MaxDistance;

        // Assign AudioMixerGroup if provided
        if (data.MixerGroup != null)
        {
            source.outputAudioMixerGroup = data.MixerGroup;
        }
    }

    // =============
    // RETURN TO POOL
    // =============

    /// <summary>
    /// Waits for the sound to finish playing (accounting for pitch changes),
    /// then returns the AudioSource to the pool.
    /// If the parent object was destroyed, handles the return safely.
    /// Fires the completion callback on return
    /// </summary>
    private IEnumerator ReturnToPoolRoutine(AudioSource source, SoundData data, float duration, bool hadParent, System.Action<SoundData> onComplete)
    {
        yield return new WaitForSecondsRealtime(duration + 0.1f); // Extra buffer to ensure playback is complete

        // AudioSource was destroyed externally (e.g. with its parent)
        if (source == null)
        {
            DecrementVoiceCount(data);
            D($"AudioSource for '{data.name}' was destroyed externally");
            yield break;
        }

        source.Stop();

        // Detach from parent if attached and return under pool container
        if (hadParent)
        {
            source.transform.SetParent(_runtimePoolContainer);
            source.transform.localPosition = Vector3.zero;
            source.transform.localRotation = Quaternion.identity;
        }

        // Release back to pool (OnReleaseToPool handles Stop and deactivation)
        _pool.Release(source);

        // Decrement voice count
        DecrementVoiceCount(data);

        D($"Returned to pool: '{data.Clip.name}'. " +
           $"Pool: active={_pool.CountActive}, inactive={_pool.CountInactive}, total={_pool.CountAll}");

        // Completion callback — notify channel subscribers
        onComplete?.Invoke(data);
    }

    /// <summary>
    /// Decrements the active voice count for a given SoundData.
    /// Removes the dictionary entry when count reaches zero to free memory
    /// </summary>
    private void DecrementVoiceCount(SoundData data)
    {
        if (_activeVoiceCounts.TryGetValue(data, out int count))
        {
            if (count <= 1)
            {
                _activeVoiceCounts.Remove(data);
            }
            else
            {
                _activeVoiceCounts[data] = count - 1;
            }
        }
    }

    // ====
    // DEBUG
    // ====

    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[AudioManager] {message}", this);
    }
}

