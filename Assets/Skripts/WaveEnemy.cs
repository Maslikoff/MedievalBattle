using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveEnemy : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int _totalWaves = 6;
    [SerializeField] private int _startingEnemies = 3;
    [SerializeField] private float _timeBetweenWaves = 3f;

    [Header("Spawn Settings")]
    [SerializeField] private int[] _enemiesPerWave = new int[] { 3, 5, 7, 10, 15, 20 };
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private ParticleSystem _smokeEffect;

    private int _currentWave = 0;
    private int _enemiesRemaining = 0;
    private List<Enemy> _currentEnemies = new List<Enemy>();
    private Coroutine _waveCoroutine;

    private bool _waveInProgress = false;
    private bool _waitingForBoss = false;

    public int CurrentWave => _currentWave;
    public int TotalWaves => _totalWaves;
    public bool IsWaveInProgress => _waveInProgress;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnAllWavesCompleted;

    private void Start()
    {
        InitializeWaveManager();
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (_currentWave >= _totalWaves)
        {
            AllWavesCompleted();
            return;
        }

        _currentWave++;
        _waveInProgress = true;

        OnWaveStarted?.Invoke(_currentWave);

        if (_waveCoroutine != null)
            StopCoroutine(_waveCoroutine);

        _waveCoroutine = StartCoroutine(WaveRoutine());
    }

    public void EnemyDefeated(Enemy enemy)
    {
        _currentEnemies.Remove(enemy);
        _enemiesRemaining--;

        if (_enemiesRemaining <= 0 && _waveInProgress)
        {
            _waveInProgress = false;
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    public void ResetGame()
    {
        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
            _waveCoroutine = null;
        }

        StopAllCoroutines();

        _currentWave = 0;
        _enemiesRemaining = 0;
        _waveInProgress = false;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        _enemySpawner.ClearAllEnemies();
        StartNextWave();
    }

    public void SetEnemySpawner(EnemySpawner spawner)
    {
        _enemySpawner = spawner;

        if (_enemySpawner != null && _spawnPoints.Length > 0)
            _enemySpawner.SetSpawnPoints(_spawnPoints);
    }

    public void SetSpawnPoints(Transform[] spawnPoints)
    {
        _spawnPoints = spawnPoints;

        if (_enemySpawner != null)
            _enemySpawner.SetSpawnPoints(_spawnPoints);
    }

    public void StopAllWaves()
    {
        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
            _waveCoroutine = null;
        }

        _waveInProgress = false;
        _enemySpawner.StopSpawning();
    }

    private void InitializeWaveManager()
    {
        if(_enemySpawner != null)
        {
            _enemySpawner.SetSpawnPoints(_spawnPoints);
            _enemySpawner.SetWaveManager(this);

            foreach (var pool in _enemySpawner.GetComponentsInChildren<EnemyPool>())
                pool.EnemyDeathFromPool += OnEnemyDefeated;
        }
    }

    private void OnEnemyDefeated(Enemy enemy)
    {
        if (enemy != null)
            _enemiesRemaining--;
    }

    private IEnumerator WaveRoutine()
    {
        int enemiesToSpawn = GetEnemiesForWave(_currentWave);
        _enemiesRemaining = enemiesToSpawn;


        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemyWithSmoke(false);
            yield return new WaitForSeconds(0.5f); 
        }

        yield return new WaitUntil(() => _enemiesRemaining <= 0);

        yield return new WaitForSeconds(1f);

        SpawnEnemyWithSmoke(true);
        _enemiesRemaining++;

        yield return new WaitUntil(() => _enemiesRemaining <= 0);

        WaveCompleted();
    }

    private int GetEnemiesForWave(int waveNumber)
    {
        if (waveNumber <= _enemiesPerWave.Length)
            return _enemiesPerWave[waveNumber - 1];
        else
            return _enemiesPerWave[_enemiesPerWave.Length - 1] + (waveNumber - _enemiesPerWave.Length) * 5;
    }

    private void SpawnEnemyWithSmoke(bool isBoss)
    {
        if (_spawnPoints.Length == 0) 
            return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
       
        if (_smokeEffect != null)
        {
            _smokeEffect.transform.position = spawnPoint.position;
            _smokeEffect.Play();
        }

        StartCoroutine(SpawnAfterSmoke(spawnPoint.position, isBoss));
    }

    private IEnumerator SpawnAfterSmoke(Vector3 spawnPosition, bool isBoss)
    {
        yield return new WaitForSeconds(1f);

        if (isBoss)
            _enemySpawner.SpawnBoss();
        else
            _enemySpawner.SpawnEnemy();

        if (_smokeEffect != null)
            _smokeEffect.Stop();
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        yield return new WaitForSeconds(_timeBetweenWaves);

        StartNextWave();
    }

    private void WaveCompleted()
    {
        _waveInProgress = false;
        OnWaveCompleted?.Invoke(_currentWave);

        if (_currentWave < _totalWaves)
            StartCoroutine(StartNextWaveAfterDelay());
    }

    private void AllWavesCompleted()
    {
        _waveInProgress = false;
        OnAllWavesCompleted?.Invoke();

        Victory();
    }

    private void Victory()
    {
        Debug.Log("All waves completed! Victory!");
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDestroy()
    {
        if (_waveCoroutine != null)
            StopCoroutine(_waveCoroutine);
    }
}