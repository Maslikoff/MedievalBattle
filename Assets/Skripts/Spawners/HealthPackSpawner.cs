using System.Collections;
using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    [SerializeField] private HealthPackPool _healthPackPool;
    [SerializeField] private float _spawnInterval = 30f;
    [SerializeField] private int _maxHealthPacks = 5;
    [SerializeField] private Vector3 _spawnArea = new Vector3(10f, 0f, 10f);

    private int _currentHealthPacks;
    private Coroutine _spawnCoroutine;

    private void Start()
    {
        _healthPackPool.HealthPackTaken += OnHealthPackTaken;

        StartSpawning();
    }

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void StartSpawning()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(_spawnInterval);

        while (_currentHealthPacks < _maxHealthPacks)
        {
            SpawnHealthPack();

            yield return waitForSeconds;
        }
    }

    private void SpawnHealthPack()
    {
        if (_currentHealthPacks >= _maxHealthPacks) 
            return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        _healthPackPool.SpawnHealthPack(spawnPosition);
        _currentHealthPacks++;
    }

    private void OnHealthPackTaken()
    {
        _currentHealthPacks--;
        _currentHealthPacks = Mathf.Max(0, _currentHealthPacks);

        if (_spawnCoroutine == null)
            StartSpawning();
    }


    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-_spawnArea.x, _spawnArea.x), 0f, Random.Range(-_spawnArea.z, _spawnArea.z));

        return transform.position + randomPoint;
    }

    private void OnDestroy()
    {
        if (_healthPackPool != null)
            _healthPackPool.HealthPackTaken -= OnHealthPackTaken;
    }
}