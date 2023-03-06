using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;
    public static T Instance => _instance;

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