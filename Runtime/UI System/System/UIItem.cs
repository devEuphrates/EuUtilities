using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EUI System/Item", 2)]
public class UIItem : MonoBehaviour
{
    [SerializeField, Tooltip("Only drag components of IUIAnim interface!")] List<Object> _enabledAnims = new List<Object>();
    [SerializeField, Tooltip("Only drag components of IUIAnim interface!")] List<Object> _disabledAnims = new List<Object>();

    List<IUIAnim> _ienableAnims = new List<IUIAnim>();
    List<IUIAnim> _idisableAnims = new List<IUIAnim>();

    public event System.Action OnDisableAnimsComplete;

    private void OnEnable()
    {
        _ienableAnims.Clear();
        _idisableAnims.Clear();

        foreach (var an in _enabledAnims)
        {
            if (an is not IUIAnim anim)
                continue;

            _ienableAnims.Add(anim);
        }

        foreach (var an in _disabledAnims)
        {
            if (an is not IUIAnim anim)
                continue;

            anim.OnFinished += DisableAnimComplete;
            _idisableAnims.Add(anim);
        }
    }

    private void OnDisable()
    {
        foreach (var anim in _idisableAnims)
            anim.OnFinished -= DisableAnimComplete;
    }

    public virtual void Enable()
    {
        foreach (var anim in _ienableAnims)
            anim.Play();
    }

    public virtual void Disable()
    {
        if (_idisableAnims.Count == 0)
        {
            OnDisableAnimsComplete?.Invoke();
            return;
        }

        _completedAnims = 0;
        foreach (var anim in _idisableAnims)
            anim.Play();
    }

    int _completedAnims = 0;
    void DisableAnimComplete()
    {
        if (++_completedAnims < _idisableAnims.Count)
            return;

        _completedAnims = 0;
        OnDisableAnimsComplete?.Invoke();
    }

    public void AddEnableAnim(IUIAnim anim)
    {
        if (_enabledAnims.Contains((Object)anim))
            return;

        _enabledAnims.Add((Object)anim);
        _ienableAnims.Add(anim);
    }

    public void RemoveEnableAnim(IUIAnim anim)
    {
        if (!_enabledAnims.Contains((Object)anim))
            return;

        _enabledAnims.Remove((Object)anim);
        _ienableAnims.Remove(anim);
    }

    public void AddDisableAnim(IUIAnim anim)
    {
        if (_disabledAnims.Contains((Object)anim))
            return;

        anim.OnFinished += DisableAnimComplete;
        _disabledAnims.Add((Object)anim);
        _idisableAnims.Add(anim);
    }

    public void RemoveDisableAnim(IUIAnim anim)
    {
        if (!_disabledAnims.Contains((Object)anim))
            return;

        anim.OnFinished -= DisableAnimComplete;
        _disabledAnims.Remove((Object)anim);
        _idisableAnims.Remove(anim);
    }
}
