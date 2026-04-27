using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionBtnManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    private class Arrow
    {
        [SerializeField] private Image _image;

        private enum State
        {
            Hidden,
            Transition,
            Shown
        }

        private State _state = State.Hidden;
        private Tween _transitionTween;

        public Image Image => _image;

        public void SetHiddenImmediate()
        {
            KillTween();

            if (_image == null)
            {
                return;
            }

            var color = _image.color;
            color.a = 0f;
            _image.color = color;
            _state = State.Hidden;
        }

        // --- States ---

        public void MarkTransition() { _state = State.Transition; }

        public void MarkShown() { _state = State.Shown; }

        public void MarkHidden() { _state = State.Hidden; }

        // --- Tweens ---

        public void SetTransitionTween(Tween tween) { _transitionTween = tween; }

        public void KillTween()
        {
            if (_transitionTween != null && _transitionTween.IsActive())
            {
                _transitionTween.Kill();
            }

            _transitionTween = null;
        }
    }

    [Serializable]
    private class BtnText
    {
        [SerializeField] private TMP_Text _text;

        private enum State
        {
            Idle,
            Transition,
            Active
        }

        private State _state = State.Idle;
        private Tween _transitionTween;

        public TMP_Text Text => _text;

        public void SetIdleImmediate(Color passiveColor)
        {
            KillTween();

            if (_text == null)
            {
                return;
            }

            _text.color = passiveColor;
            _state = State.Idle;
        }

        // --- States ---

        public void MarkTransition() { _state = State.Transition; }

        public void MarkActive() { _state = State.Active; }

        public void MarkIdle() { _state = State.Idle; }

        // --- Tweens ---

        public void SetTransitionTween(Tween tween) { _transitionTween = tween; }

        public void KillTween()
        {
            if (_transitionTween != null && _transitionTween.IsActive())
            {
                _transitionTween.Kill();
            }

            _transitionTween = null;
        }
    }

    // --- References ---
    [SerializeField] private Arrow _leftArrow;
    [SerializeField] private Arrow _rightArrow;
    [SerializeField] private BtnText _label;

    // --- Settings ---
    [SerializeField] private Color _activeTextColor = Color.white;
    [SerializeField] private Color _passiveTextColor = Color.gray;
    [SerializeField] private float _transitionDuration = 0.2f;

    void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void Awake()
    {
        InitializeIdleState();
    }

    private void OnDisable()
    {
        _leftArrow?.KillTween();
        _rightArrow?.KillTween();
        _label?.KillTween();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUnhover();
    }

    public void OnHover()
    {
        Show(_leftArrow);
        Show(_rightArrow);
        Activate(_label);
    }

    public void OnUnhover()
    {
        Hide(_leftArrow);
        Hide(_rightArrow);
        Deactivate(_label);
    }

    /// <summary>
    /// Animates an Arrow into Shown state
    /// </summary>
    private void Show(Arrow arrow)
    {
        if (arrow == null || arrow.Image == null)
        {
            return;
        }

        arrow.KillTween();
        arrow.MarkTransition();

        Tween tween = arrow.Image
            .DOFade(1f, _transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(arrow.MarkShown);

        arrow.SetTransitionTween(tween);
    }

    /// <summary>
    /// Animates an Arrow into Hidden state
    /// </summary>
    private void Hide(Arrow arrow)
    {
        if (arrow == null || arrow.Image == null)
        {
            return;
        }

        arrow.KillTween();
        arrow.MarkTransition();

        Tween tween = arrow.Image
            .DOFade(0f, _transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(arrow.MarkHidden);

        arrow.SetTransitionTween(tween);
    }

    /// <summary>
    /// Animates the Label into Active state
    /// </summary>
    private void Activate(BtnText text)
    {
        if (text == null || text.Text == null)
        {
            return;
        }

        text.KillTween();
        text.MarkTransition();

        Tween tween = text.Text
            .DOColor(_activeTextColor, _transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(text.MarkActive);

        text.SetTransitionTween(tween);
    }

    /// <summary>
    /// Animates the Label into Idle state
    /// </summary>
    private void Deactivate(BtnText text)
    {
        if (text == null || text.Text == null)
        {
            return;
        }

        text.KillTween();
        text.MarkTransition();

        Tween tween = text.Text
            .DOColor(_passiveTextColor, _transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(text.MarkIdle);

        text.SetTransitionTween(tween);
    }

    /// <summary>
    /// Applies the default non-hover visual state
    /// </summary>
    private void InitializeIdleState()
    {
        _leftArrow?.SetHiddenImmediate();
        _rightArrow?.SetHiddenImmediate();
        _label?.SetIdleImmediate(_passiveTextColor);
    }
}
