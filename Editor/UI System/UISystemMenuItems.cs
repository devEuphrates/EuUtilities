using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UISystemMenuItems
{
    [MenuItem("GameObject/Euphrates UI/Managed Canvas", priority = 0)]
    public static UIManager CreateManager()
    {
        GameObject go = new GameObject("Managed Canvas");
        Selection.activeGameObject = go;

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        var manager = go.AddComponent<UIManager>();

        Undo.RegisterCreatedObjectUndo(go, "Created Managed Canvas");
        return manager;
    }

    [MenuItem("GameObject/Euphrates UI/Window")]
    public static UIWindow CreateWindow()
    {
        GameObject sel = Selection.activeGameObject;

        if (sel == null || !sel.TryGetComponent<UIManager>(out var manager))
            manager = CreateManager();

        GameObject go = new GameObject("UI Window");
        go.transform.SetParent(manager.transform);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Selection.activeGameObject = go;

        var window = go.AddComponent<UIWindow>();

        manager.AddWindow(new UIManager.Window()
        {
            UIWindow = window
        });

        Undo.RegisterCreatedObjectUndo(go, "Created UI Window");
        return window;
    }

    [MenuItem("GameObject/Euphrates UI/Items/Item")]
    public static UIItem CreateItem()
    {
        GameObject sel = Selection.activeGameObject;

        if (sel == null || !sel.TryGetComponent<UIWindow>(out var window))
            window = CreateWindow();

        GameObject go = new GameObject("UI Item");
        go.transform.SetParent(window.transform);

        var rt = go.AddComponent<RectTransform>();
        Vector2 center = new Vector2(.5f, .5f); 
        rt.anchorMin = center;
        rt.anchorMax = center;
        rt.anchoredPosition = Vector2.zero;

        Selection.activeGameObject = go;

        UIItem item = go.AddComponent<UIItem>();

        window.AddItem(item);

        Undo.RegisterCreatedObjectUndo(go, "Created UI Item");
        return item;
    }

    [MenuItem("GameObject/Euphrates UI/Items/Fade In-Out Item")]
    public static UIItem CreateFadeInOutItem()
    {
        UIItem item = CreateItem();
        GameObject go = item.gameObject;

        var fadeIn = go.AddComponent<UIFadeIn>();
        var fadeOut = go.AddComponent<UIFadeOut>();

        item.AddEnableAnim(fadeIn);
        item.AddDisableAnim(fadeOut);

        return item;
    }
}
