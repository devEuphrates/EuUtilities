using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("EUI System/Window", 1)]
public class UIWindow : MonoBehaviour
{
    [SerializeReference] List<UIItem> _items = new List<UIItem>();

    [Space]
    [Header("Events")]
    [SerializeField] UnityEvent _onEnabled;
    [SerializeField] UnityEvent _onDisabled;

    private void OnEnable()
    {
        foreach (var item in _items)
            item.OnDisableAnimsComplete += OnItemDisabled;
    }

    private void OnDisable()
    {
        foreach (var item in _items)
            item.OnDisableAnimsComplete -= OnItemDisabled;

        _onDisabled.Invoke();
    }

    public void EnableWindow()
    {
        foreach (var item in _items)
            item.Enable();

        _onEnabled.Invoke();
    }

    int _finishedItems = 0;
    void OnItemDisabled()
    {
        if (++_finishedItems < _items.Count)
            return;

        _finishedItems = 0;
        DisableInstant();
    }

    public void DisableWindow()
    {
        _finishedItems = 0;

        foreach (var item in _items)
            item.Disable();

    }

    public void DisableInstant()
    {
        gameObject.SetActive(false);
    }

    public void AddItem(UIItem item)
    {
        if (CheckItem(item))
            return;

        _items.Add(item);
    }

    public void RemoveItem(UIItem item)
    {
        if (!CheckItem(item))
            return;

        _items.Add(item);
    }

    public bool CheckItem(UIItem item)
    {
        foreach (var itm in _items)
        {
            UIItem uiItem = itm as UIItem;
            if (uiItem == null || uiItem != item)
                continue;

            return true;
        }

        return false;
    }
}
