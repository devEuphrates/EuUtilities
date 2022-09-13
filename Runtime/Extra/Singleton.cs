using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;
    public static T Instance 
    {
        get 
        {
            if (_instance != null)
                return _instance;

            GameObject go = new GameObject(typeof(T).Name);

            T comp = go.AddComponent<T>();
            _instance = comp;

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            if (_instance == this)
                return;

            Destroy(this);
        }

        _instance = this as T;
    }
}