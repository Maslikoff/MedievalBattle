using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<Enemy>
{
    [SerializeField] private bool _isBossPool = false;
    [SerializeField] private Transform _playerTraget;

    private List<Enemy> _activeEnemies = new List<Enemy>();

    public bool IsBossPool => _isBossPool;
    public int ActiveEnemiesCount => _activeEnemies.Count;

    public event Action<Enemy> EnemyDeathFromPool;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnObjectGet(Enemy enemy)
    {
        enemy.Initialize(_playerTraget);
    }

    public override Enemy GetFromPool()
    {
        Enemy enemy = base.GetFromPool();
        _activeEnemies.Add(enemy);

        enemy.EnemyDeath += OnEnemyDeath;

        return enemy;
    }

    public override void ReturnToPool(Enemy enemy)
    {
        base.ReturnToPool(enemy);
        _activeEnemies.Remove(enemy);

        enemy.EnemyDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        ReturnToPool(enemy);
        EnemyDeathFromPool?.Invoke(enemy);
    }

    public void SpawnAtPosition(Vector3 position)
    {
        Enemy enemy = GetFromPool();
        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.identity;
        enemy.ResumeEnemy();
    }

    public void ClearAllActiveEnemies()
    {
        foreach (Enemy enemy in _activeEnemies.ToArray())
            ReturnToPool(enemy);
    }
}