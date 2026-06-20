using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

/// <summary>
/// Global Time Manager. Controls Time Scale
/// </summary>
[DisallowMultipleComponent]
public class TimeManager : MonoBehaviour
{
    // =====
    // CONFIG
    // =====

    [FoldoutGroup("Config")]
    [Tooltip("Maximum allowed time scale")]
    [SerializeField] private float _maxTimeScale = 10f;

    [FoldoutGroup("Config")]
    [Tooltip("Minimum fixedDeltaTime to prevent physics lock-up at extreme slow-mo")]
    [SerializeField] private float _minFixedDeltaTime = 0.0001f;

    // =====
    // EVENTS
    // =====

    [FoldoutGroup("Events")]
    [Tooltip("Fired on any timeScale change (including every frame during smooth transitions)")]
    [SerializeField] private GameEvent _onTimeScaleChanged;

    [FoldoutGroup("Events")]
    [Tooltip("Fired when timeScale reaches 0 (stopped)")]
    [SerializeField] private GameEvent _onTimeStopped;

    [FoldoutGroup("Events")]
    [Tooltip("Fired when timeScale becomes > 0 after being stopped")]
    [SerializeField] private GameEvent _onTimeResumed;

    // ====
    // DEBUG
    // ====

    [FoldoutGroup("Debug")]
    [Tooltip("Turn on Debug Logging")]
    [SerializeField] private bool _debug;

    private string _lastDebug;

    // ====
    // STATE
    // ====

    private float _baseFixedDeltaTime;
    private float _timeScaleBeforeStop = 1f;
    private bool _isStopped;
    private Tweener _scaleTween;

    // ===========
    // PUBLIC STATE
    // ===========

    /// <summary>
    /// Current Time Scale
    /// </summary>
    public float CurrentTimeScale => Time.timeScale;

    /// <summary>
    /// Whether Time is currently stopped
    /// </summary>
    public bool IsStopped => _isStopped;

    // ========
    // LIFECYCLE
    // ========

    private void Awake()
    {
        _baseFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void OnDisable()
    {
        KillTween();
    }

    private void OnDestroy()
    {
        // Restore defaults so project settings remain untouched
        KillTween();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _baseFixedDeltaTime;
    }

    // =========
    // PUBLIC API
    // =========

    /// <summary>
    /// Instantly set Time Scale to <paramref name="targetScale"/>.
    /// Kills any active smooth transition
    /// </summary>
    public void SetTimeScale(float targetScale)
    {
        targetScale = Mathf.Clamp(targetScale, 0f, _maxTimeScale);
        KillTween();

        HandleStoppedResumedTransition(targetScale);
        ApplyTimeScale(targetScale);

        D($"SetTimeScale: {targetScale}");
    }

    /// <summary>
    /// Smoothly transition Time Scale to <paramref name="targetScale"/>
    /// over <paramref name="duration"/> seconds (ignores Time Scale)
    /// </summary>
    public void SetTimeScaleSmoothly(float targetScale, float duration)
    {
        targetScale = Mathf.Clamp(targetScale, 0f, _maxTimeScale);
        KillTween();

        // Remember scale before smooth transition starts — ResumeTime will use this
        // if StopTime() is called and then ResumeTime() after the tween was killed.
        _timeScaleBeforeStop = Time.timeScale;

        _scaleTween = DOTween
            .To(
                getter: () => Time.timeScale,
                setter: scale =>
                {
                    ApplyTimeScale(scale);

                    // Track stopped/resumed state during smooth transitions
                    if (scale <= 0f && !_isStopped)
                    {
                        _isStopped = true;
                        _onTimeStopped?.Invoke();
                        D("Smooth transition reached 0 — time stopped");
                    }
                    else if (scale > 0f && _isStopped)
                    {
                        _isStopped = false;
                        _onTimeResumed?.Invoke();
                        D("Smooth transition left 0 — time resumed");
                    }
                },
                endValue: targetScale,
                duration: duration
            )
            .SetUpdate(true) // Run on unscaled time so the tween is immune to timeScale changes
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Ensure stopped flag matches final state
                if (targetScale <= 0f && !_isStopped)
                {
                    _isStopped = true;
                    _onTimeStopped?.Invoke();
                }

                _scaleTween = null;
                D($"Smooth transition complete: {targetScale}");
            });

        D($"SetTimeScaleSmoothly: {Time.timeScale} -> {targetScale} over {duration:F2}s");
    }

    /// <summary>
    /// Instantly stop Time.
    /// Preserves previous scale for ResumeTime().
    /// If a smooth transition is in progress, it is Paused
    /// </summary>
    public void StopTime()
    {
        if (_isStopped)
        {
            D("StopTime ignored: already stopped");
            return;
        }

        _timeScaleBeforeStop = Time.timeScale;

        // Pause instead of kill — allows ResumeTime to continue the tween from its current progress
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Pause();
            D("StopTime: paused active scale tween");
        }

        _isStopped = true;
        ApplyTimeScale(0f);
        _onTimeStopped?.Invoke();
        D($"StopTime: saved previous scale={_timeScaleBeforeStop:F3}");
    }

    /// <summary>
    /// Instantly resume Time.
    /// If a smooth transition was paused by StopTime(), it continues from where it left off
    /// </summary>
    public void ResumeTime()
    {
        if (!_isStopped)
        {
            D("ResumeTime ignored: not stopped");
            return;
        }

        _isStopped = false;

        // If a tween was paused by StopTime, resume it — it will continue
        // interpolating from its saved progress toward the original target
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Play();
            _onTimeResumed?.Invoke();
            D("ResumeTime: resumed paused scale tween");
            return;
        }

        // No paused tween — instantly restore saved scale
        float targetScale = _timeScaleBeforeStop > 0f ? _timeScaleBeforeStop : 1f;
        ApplyTimeScale(targetScale);
        _onTimeResumed?.Invoke();
        D($"ResumeTime: restored scale={targetScale:F3}");
    }

    // =============
    // HELPER METHODS
    // =============

    /// <summary>
    /// Applies Time Scale to Unity's Time system and adjusts Physics Step.
    /// Fires _onTimeScaleChanged
    /// </summary>
    private void ApplyTimeScale(float scale)
    {
        Time.timeScale = scale;

        if (scale > 0f)
        {
            // Scale physics step proportionally for consistent simulation at any speed
            float targetFixedDelta = _baseFixedDeltaTime * scale;
            Time.fixedDeltaTime = Mathf.Max(targetFixedDelta, _minFixedDeltaTime);
        }
        // When scale == 0, leave fixedDeltaTime unchanged — physics is already paused by timeScale

        _onTimeScaleChanged?.Invoke();
    }

    /// <summary>
    /// Updates _isStopped flag.
    /// Fires Stop/Resume GameEvents when crossing zero
    /// </summary>
    private void HandleStoppedResumedTransition(float targetScale)
    {
        if (targetScale <= 0f && !_isStopped)
        {
            _timeScaleBeforeStop = Time.timeScale;
            _isStopped = true;
            _onTimeStopped?.Invoke();
            D("Transition to stopped");
        }
        else if (targetScale > 0f && _isStopped)
        {
            _isStopped = false;
            _onTimeResumed?.Invoke();
            D("Transition from stopped to running");
        }
    }

    /// <summary>
    /// Kills the active Time Scale tween
    /// </summary>
    private void KillTween()
    {
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }
        _scaleTween = null;
    }

    private void D(string message)
    {
        if (!_debug) return;
        if (_lastDebug == message) return;

        _lastDebug = message;
        Debug.Log($"[TimeManager:{gameObject.name}] {message}", this);
    }
}
