using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    private Object _pool;

    public void Initialize(Object pool)
    {
        _pool = pool;
    }

    public void ReturnToPool()
    {
        if( _pool != null)
        {
            var method = _pool.GetType().GetMethod("ReturnToPool");
            method?.Invoke(_pool, new object[] { GetComponent<Component>() });
        }
        else
        {
            Destroy(gameObject);
        }
    }
}