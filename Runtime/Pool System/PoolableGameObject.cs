using UnityEngine;

public class PoolableGameObject : MonoBehaviour, IPoolable
{
    public void OnDestroyed()
    {
    }

    public void OnGet()
    {
    }

    public void OnReleased()
    {
    }
}
