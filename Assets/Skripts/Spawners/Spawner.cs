using System.Collections;
using UnityEngine;

public abstract class Spawner<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected float _spawnInterval = 30f;
    [SerializeField] protected int _maxObjects = 5;
    [SerializeField] protected Vector3 _spawnArea = new Vector3(10f, 0f, 10f);

    protected int _currentObjectsCount;
    protected Coroutine _spawnCoroutine;
    protected ObjectPool<T> _objectPool;

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void OnEnable()
    {
        StartSpawning();
    }

    protected virtual void OnDisable()
    {
        StopSpawning();
    }

    protected virtual void Initialize()
    {
        if (_objectPool == null)
            _objectPool = GetComponent<ObjectPool<T>>();

        StartSpawning();
    }

    protected virtual void StartSpawning()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    protected virtual void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    protected virtual IEnumerator SpawnRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(_spawnInterval);

        while (enabled)
        {
            yield return waitForSeconds;

            if (CanSpawn())
                SpawnObject();
        }
    }

    protected virtual bool CanSpawn()
    {
        return _currentObjectsCount < _maxObjects;
    }

    protected abstract void SpawnObject();

    protected virtual Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-_spawnArea.x, _spawnArea.x), 0f, Random.Range(-_spawnArea.z, _spawnArea.z));

        return transform.position + randomPoint;
    }

    public virtual void OnObjectTaken()
    {
        _currentObjectsCount--;

        if (CanSpawn())
            StartCoroutine(DelayedSpawnCheck());
    }

    protected virtual IEnumerator DelayedSpawnCheck()
    {
        yield return new WaitForSeconds(_spawnInterval);

        if (CanSpawn())
            StartSpawning();
    }

    public virtual void IncreaseObjectCount()
    {
        _currentObjectsCount++;
    }

    public virtual void DecreaseObjectCount()
    {
        _currentObjectsCount = Mathf.Max(0, _currentObjectsCount - 1);
    }

    public void SetMaxObjects(int maxObjects)
    {
        _maxObjects = maxObjects;
    }

    public void SetSpawnInterval(float interval)
    {
        _spawnInterval = interval;
    }
}