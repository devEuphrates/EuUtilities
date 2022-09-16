using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pooler : Singleton<Pooler>
{
    // Data of objects to be pooled.
    [SerializeField] List<Data> _pooledObjects = new List<Data>();

    // Dictionary that will be populated with pools. Keys are set to names from _pooledObjects list.
    Dictionary<string, ObjectPool<GameObject>> _pools = new Dictionary<string, ObjectPool<GameObject>>();

    // Collection check setting is global unlike pool size so it can be enabled only if the code is consistent.
    [SerializeField] bool _collectionCheck = false;

    protected override void Awake()
    {
        base.Awake();

        List<GameObject> spawned = new List<GameObject>();

        // Initializ pool of each data set and populate the dictionary.
        foreach (var dt in _pooledObjects)
        {
            var pool = (_pools[dt.Name] = new ObjectPool<GameObject>(() => CreatePoolObject(dt.Name, dt.Prefab), OnObjectTaken, OnObjectReturned, OnDestroyObject
                , _collectionCheck, dt.MinCapacity, dt.MaxCapacity));

            //  We get and release min capacity amount of objects to pre spawn min amount of objects and deactivate them.

            for (int i = 0; i < dt.MinCapacity; i++)
                spawned.Add(pool.Get());

            foreach (var item in spawned)
                pool.Release(item);
        }

    }

    GameObject CreatePoolObject(string name, GameObject prefab)
    {
        // Spawn the object and set the name so it's pool can be identified from the inspector.
        GameObject go = Instantiate(prefab);
        go.name = $"Pooled {name}";

        // Set pooler as parent so it does not populate scene view.
        go.transform.SetParent(transform);
        // Deactivate object to prevent 
        go.SetActive(false);

        return go;
    }

    void OnObjectTaken(GameObject go)
    {
        // Activate GameObject when it's taken from the pool.
        go.SetActive(true);

        // If it has a comnponent with IPoolable interface trigger OnGet method.
        if (go.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnGet();
    }

    void OnObjectReturned(GameObject go)
    {
        // If it has a component with IPoolable interface trigger OnReleased method.
        if (go.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnReleased();

        // Deactivate GameObject if it's released back to its pool.
        go.SetActive(false);
        go.transform.parent = transform;
    }

    void OnDestroyObject(GameObject go)
    {
        // Call the OnDestroyed Callback if there is a IPoolable component.
        if (go.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnDestroyed();

        // Destroy the GameObject.
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

    // This static methode handles spawning from existing _pools.
    public static GameObject Spawn(string poolName, Transform parent, Vector3 position, Quaternion rotation)
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

    public static GameObject Spawn(string poolName, Transform parent) => Spawn(poolName, parent, Vector3.zero, Quaternion.identity);

    public static GameObject Spawn(string poolName, Transform parent, Vector3 position) => Spawn(poolName, parent, position, Quaternion.identity);

    public static GameObject Spawn(string poolName, Transform parent, Quaternion rotation) => Spawn(poolName, parent, Vector3.zero, rotation);

    public static GameObject Spawn(string poolName, Vector3 position, Quaternion rotation) => Spawn(poolName, null, position, rotation);

    public static GameObject Spawn(string poolName, Vector3 position) => Spawn(poolName, null, position, Quaternion.identity);

    public static GameObject Spawn(string poolName, Quaternion rotation) => Spawn(poolName, null, Vector3.zero, rotation);

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
