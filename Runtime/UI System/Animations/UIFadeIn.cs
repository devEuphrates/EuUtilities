using Euphrates;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("EUI System/Animations/Fade In")]
public class UIFadeIn : MonoBehaviour, IUIAnim
{
    CanvasGroup _canvasGroup;

    [SerializeField] float _duration = 1f;
    [SerializeField] float _delay = 0f;

    public event Action OnFinished;

    private void Start() => _canvasGroup = GetComponent<CanvasGroup>();

    private void Reset()
    {
        var item = GetComponent<UIItem>();
        
        if (!item)
            return;

        item.AddEnableAnim(this);
    }

    public void Play()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;

        void Fade()
        {
            if (_duration == 0f)
            {
                _canvasGroup.alpha = 1f;
                OnFinished?.Invoke();
                return;
            }

            _canvasGroup.DoAlpha(1f, _duration, Ease.Lerp, null, () => OnFinished?.Invoke());
        }

        if (_delay != 0f)
        {
            GameTimer.CreateTimer("Anim Fade-In", _delay, Fade);
            return;
        }

        Fade();
    }
}
