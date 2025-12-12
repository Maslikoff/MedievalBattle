using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T Prefab;
    [SerializeField] protected int InitialPoolSize = 10;

    protected Queue<T> Pool = new Queue<T>();
    protected Transform PoolParent;

    private bool _isInitialized = false;

    protected virtual void Awake()
    {
        PoolParent = new GameObject($"{typeof(T).Name}Pool").transform;
        PoolParent.SetParent(transform);
    }

    protected virtual void Start()
    {
        if (_isInitialized == false)
        {
            InitializePool();
            _isInitialized = true;
        }
    }

    protected virtual T CreateNewObject()
    {
        T obj = Instantiate(Prefab, PoolParent);
        obj.gameObject.SetActive(false);

        PoolableObject poolable = obj.gameObject.AddComponent<PoolableObject>();
        poolable.Initialize(this);

        return obj;
    }

    protected virtual void OnObjectGet(T obj) { }
    protected virtual void OnObjectReturn(T obj) { }

    protected void InitializePool()
    {
        for (int i = 0; i < InitialPoolSize; i++)
        {
            T obj = CreateNewObject();
            ReturnToPool(obj);
        }
    }

    public virtual T GetFromPool()
    {
        T obj;

        if (Pool.Count > 0)
            obj = Pool.Dequeue();
        else
            obj = CreateNewObject();

        obj.gameObject.SetActive(true);
        OnObjectGet(obj);

        return obj;
    }

    public virtual void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        Pool.Enqueue(obj);
        OnObjectReturn(obj);
    }

    public int GetPoolSize() => Pool.Count;
}