using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class Pooler : Singleton<Pooler>
{
    public static readonly int PRE_INSTANTIATE_BATCH = 50;
    public delegate void PreInstantiateEvent(string poolName, int instantiatedAmount);
    public static event PreInstantiateEvent OnPoolPreInstantiateComplete;

    public static Transform ObjParent => Pooler.Instance.transform;

    // Data of objects to be pooled.
    [SerializeField] List<PoolData> _pooledObjects = new List<PoolData>();

    // Dictionary that will be populated with pools. Keys are set to names from _pooledObjects list.
    Dictionary<string, IObjectPool<GameObject>> _pools = new Dictionary<string, IObjectPool<GameObject>>();

    // Collection check setting is global unlike pool size so it can be enabled only if the code is consistent.
    [SerializeField] bool _collectionCheck = false;

    protected override void Awake()
    {
        base.Awake();

        List<GameObject> spawned = new List<GameObject>();

        // Initializ pool of each data set and populate the dictionary.
        foreach (var dt in _pooledObjects)
        {
            var pool = (_pools[dt.Name] = new LinkedPool<GameObject>(dt.CreateObject, OnObjectTaken, OnObjectReturned, OnDestroyObject
                , _collectionCheck, dt.MaxCapacity));

            //  We get and release min capacity amount of objects to pre spawn min amount of objects and deactivate them.

            if (dt.PreInstantiate < 1)
            {
                OnPoolPreInstantiateComplete?.Invoke(dt.Name, 0);
                return;
            }

            PreInstantiate(dt.Name, dt.PreInstantiate);

            //for (int i = 0; i < dt.MinCapacity; i++)
            //    spawned.Add(pool.Get());

            //foreach (var item in spawned)
            //    pool.Release(item);
        }
    }

    async void PreInstantiate(string poolName, int amount)
    {
        if (!_pools.TryGetValue(poolName, out var pool))
            return;

        GameObject[] spawned = new GameObject[amount];

        for (int i = 0; i < amount; i++)
        {
            GameObject go = pool.Get();
            spawned[i] = go;
            go.SetActive(false);

            if (i % PRE_INSTANTIATE_BATCH == 0)
                await Task.Yield();
        }

        for (int i = 0; i < amount; i++)
            pool.Release(spawned[i]);

        OnPoolPreInstantiateComplete?.Invoke(poolName, amount);
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
    struct PoolData
    {
        public string Name;
        public GameObject Prefab;
        public int PreInstantiate;
        public int MaxCapacity;

        public GameObject CreateObject()
        {
            // Spawn the object and set the name so it's pool can be identified from the inspector.
            GameObject go = Instantiate(Prefab);
            go.name = $"Pooled {Name}";

            // Set pooler as parent so it does not populate scene view.
            go.transform.SetParent(ObjParent);
            // Deactivate object to prevent 
            go.SetActive(false);

            return go;
        }
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
