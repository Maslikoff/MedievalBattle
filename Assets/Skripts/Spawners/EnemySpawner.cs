using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _maxEnemies = 20;
    [SerializeField] private bool _autoSpawn = false;
    [SerializeField] private WaveEnemy _waveManager;

    [Header("Enemy Pools")]
    [SerializeField] private List<EnemyPool> _enemyPools = new List<EnemyPool>();

    public void ClearAllEnemies()
    {
        foreach (EnemyPool pool in _enemyPools)
            pool.ClearAllActiveEnemies();
    }

    public void SetSpawnPoints(Transform[] spawnPoints)
    {
        _spawnPoints = spawnPoints;
    }

    public void SetWaveManager(WaveEnemy waveManager)
    {
        waveManager = _waveManager;
    }

    public void SpawnEnemy()
    {
        if (_spawnPoints.Length == 0 || _enemyPools.Count == 0) return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        EnemyPool randomPool = GetRandomNormalPool();

        randomPool.SpawnAtPosition(spawnPoint.position);
    }

    public void SpawnBoss()
    {
        if (_spawnPoints.Length == 0)
            return;

        EnemyPool bossPool = GetBossPool();

        if (bossPool != null)
        {
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            bossPool.SpawnAtPosition(spawnPoint.position);
        }
    }

    private EnemyPool GetBossPool()
    {
        EnemyPool bossPool = _enemyPools.Find(_pool => _pool.IsBossPool);

        if (bossPool == null)
        {
            bossPool = GetComponentInChildren<EnemyPool>();

            if (bossPool != null && bossPool.IsBossPool)
                _enemyPools.Add(bossPool);
        }

        return bossPool;
    }

    private EnemyPool GetRandomNormalPool()
    {
        List<EnemyPool> normalPools = _enemyPools.FindAll(pool => !pool.IsBossPool);

        if (normalPools.Count == 0)
            return null;

        return normalPools[Random.Range(0, normalPools.Count)];
    }
}