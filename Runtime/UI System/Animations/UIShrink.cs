using Euphrates;
using System;
using UnityEngine;

[AddComponentMenu("EUI System/Animations/Shrink")]
public class UIShrink : MonoBehaviour, IUIAnim
{
    [SerializeField] float _duration = 1f;
    [SerializeField] float _delay = 0f;

    public event Action OnFinished;

    public void Play()
    {
        transform.localScale = Vector3.one;

        void Scale() => transform.DoScale(Vector3.zero, _duration, Ease.Lerp, null, () => OnFinished?.Invoke());

        if (_duration != 0f)
        {
            GameTimer.CreateTimer("Anim Shrink", _delay, Scale);
            return;
        }

        Scale();
    }
}
