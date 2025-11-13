using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPool : ObjectPool<Enemy>
{
    [SerializeField] private bool _isBossPool = false;
    [SerializeField] private Transform _playerTraget;
    [SerializeField] private AmmoDropPool _ammoPool;

    private List<Enemy> _activeEnemies = new List<Enemy>();

    public bool IsBossPool => _isBossPool;
    public int ActiveEnemiesCount => _activeEnemies.Count;

    public event Action<Enemy> EnemyDeathFromPool;
    public event Action<Vector3> BossDeath;

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

    public void OnBossDeathWithAmmo(Vector3 position, int amount)
    {
        if (_ammoPool != null)
            _ammoPool.SpawnAmmo(position, amount);
    }

    public void SpawnAtPosition(Vector3 position)
    {
        Enemy enemy = GetFromPool();

        NavMeshAgent navMeshAgent = enemy.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;

        enemy.transform.position = position;
        enemy.transform.rotation = Quaternion.identity;

        navMeshAgent.enabled = true;

        enemy.ResumeEnemy();
    }

    public void ClearAllActiveEnemies()
    {
        foreach (Enemy enemy in _activeEnemies.ToArray())
            ReturnToPool(enemy);
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        if (enemy.IsBoss)
            BossDeath?.Invoke(enemy.transform.position);

        ReturnToPool(enemy);
        EnemyDeathFromPool?.Invoke(enemy);
    }
}