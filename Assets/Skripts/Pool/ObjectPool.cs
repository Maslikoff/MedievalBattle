using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T _prefab;
    [SerializeField] protected int _initialPoolSize = 10;

    protected Queue<T> _pool = new Queue<T>();
    protected Transform _poolParent;

    protected virtual void Awake()
    {
        _poolParent = new GameObject($"{typeof(T).Name}Pool").transform;
        _poolParent.SetParent(transform);

        InitializePool();
    }

    protected void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            T obj = CreateNewObject();
            ReturnToPool(obj);
        }
    }

    protected virtual T CreateNewObject()
    {
        T obj = Instantiate(_prefab, _poolParent);
        obj.gameObject.SetActive(false);

        PoolableObject poolable = obj.gameObject.AddComponent<PoolableObject>();
        poolable.Initialize(this);

        return obj;
    }

    protected virtual void OnObjectGet(T obj) { }
    protected virtual void OnObjectReturn(T obj) { }

    public int GetPoolSize() => _pool.Count;

    public virtual T GetFromPool()
    {
        T obj;

        if (_pool.Count > 0)
            obj = _pool.Dequeue();
        else
            obj = CreateNewObject();

        obj.gameObject.SetActive(true);
        OnObjectGet(obj);

        return obj;
    }

    public virtual void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
        OnObjectReturn(obj);
    }
}