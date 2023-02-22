using Euphrates;
using System;
using UnityEngine;

[AddComponentMenu("EUI System/Animations/Grow")]
public class UIGrow : MonoBehaviour, IUIAnim
{
    [SerializeField] float _duration = 1f;
    [SerializeField] float _delay = 0f;

    public event Action OnFinished;

    public void Play()
    {
        transform.localScale = Vector3.zero;
        
        void Scale() => transform.DoScale(Vector3.one, _duration, Ease.Lerp, null, () => OnFinished?.Invoke());

        if (_duration != 0f)
        {
            GameTimer.CreateTimer("Anim Grow", _delay, Scale);
            return;
        }

        Scale();
    }
}
