using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pooler : Singleton<Pooler>
{
    [SerializeField] List<Data> _pooledObjects = new List<Data>();
    Dictionary<string, ObjectPool<GameObject>> _pools = new Dictionary<string, ObjectPool<GameObject>>();

    [Space, Header("Pool Sizes")]
    [SerializeField] bool _collectionCheck = false;

    protected override void Awake()
    {
        base.Awake();

        foreach (var dt in _pooledObjects)
        {
            _pools[dt.Name] = new ObjectPool<GameObject>(() => CreatePoolObject(dt.Name, dt.Prefab), OnObjectTaken, OnObjectReturned, OnDestroyObject
                , _collectionCheck, dt.MinCapacity, dt.MaxCapacity);

            print(_pools[dt.Name].CountAll);
        }

    }

    GameObject CreatePoolObject(string name, GameObject prefab)
    {
        GameObject go = Instantiate(prefab);
        go.name = $"Pooled {name}";

        return go;
    }

    void OnObjectTaken(GameObject go)
    {
        if (go.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnGet();

        go.SetActive(true);
    }

    void OnObjectReturned(GameObject go)
    {
        if (go.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnReleased();

        go.SetActive(false);
        go.transform.parent = transform;
    }

    void OnDestroyObject(GameObject go)
    {
        if (go.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnDestroyed();

        Destroy(go);
    }

    [System.Serializable]
    struct Data
    {
        public string Name;
        public GameObject Prefab;
        public int MinCapacity;
        public int MaxCapacity;
    }

    public static GameObject PoolSpawn(string poolName, Transform parent, Vector3 position, Quaternion rotation)
    {
        if (!Instance._pools.TryGetValue(poolName, out var pool))
            return null;

        pool.Get(out var go);

        if (go == null)
            return go;

        go.transform.SetParent(parent);
        go.transform.SetPositionAndRotation(position, rotation);
        return go;
    }

    public static GameObject PoolSpawn(string poolName, Transform parent) => PoolSpawn(poolName, parent, Vector3.zero, Quaternion.identity);

    public static GameObject PoolSpawn(string poolName, Transform parent, Vector3 position) => PoolSpawn(poolName, parent, position, Quaternion.identity);

    public static GameObject PoolSpawn(string poolName, Transform parent, Quaternion rotation) => PoolSpawn(poolName, parent, Vector3.zero, rotation);

    public static GameObject PoolSpawn(string poolName, Vector3 position, Quaternion rotation) => PoolSpawn(poolName, null, position, rotation);

    public static GameObject PoolSpawn(string poolName, Vector3 position) => PoolSpawn(poolName, null, position, Quaternion.identity);

    public static GameObject PoolSpawn(string poolName, Quaternion rotation) => PoolSpawn(poolName, null, Vector3.zero, rotation);

    public static void Release(string poolName, GameObject go)
    {
        if (!Instance._pools.TryGetValue(poolName, out var pool))
        {
            Destroy(go);
            return;
        }

        pool.Release(go);
    }
}
