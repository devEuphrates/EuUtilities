using Euphrates;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[AddComponentMenu("EUI System/Manager", 0)]
public class UIManager : MonoBehaviour
{
    [SerializeField] List<Window> _windows = new List<Window>();

    private void Awake()
    {
        //foreach (var windows in _windows)
            
    }

    private void OnEnable() => SetWindows();

    private void OnDisable() => UnsetWindows();

    public void AddWindow(Window window)
    {
        _windows.Add(window);
    }

    void SetWindows()
    {
        foreach (var window in _windows)
            window.SubscribeEvents();
    }
    void UnsetWindows()
    {
        foreach (var window in _windows)
            window.UnsubscribeEvents();
    }

    [System.Serializable]
    public struct Window
    {
        public string Name;
        public UIWindow UIWindow;
        public List<TriggerChannelSO> EnableEvents;
        public List<TriggerChannelSO> DisableEvents;

        public void Enable()
        {
            UIWindow.gameObject.SetActive(true);
            UIWindow.EnableWindow();
        }

        public void Disable()
        {
            UIWindow.DisableWindow();
        }

        public void SubscribeEvents()
        {
            foreach (var ev in EnableEvents)
                ev.AddListener(Enable);

            foreach (var ev in DisableEvents)
                ev.AddListener(Disable);
        }

        public void UnsubscribeEvents()
        {
            foreach (var ev in EnableEvents)
                ev.RemoveListener(Enable);

            foreach (var ev in DisableEvents)
                ev.RemoveListener(Disable);
        }
    }
}