using System;
using UnityEngine;

public class HandlingEnemyDeath : MonoBehaviour
{
    [SerializeField] private AmmoDropPool _ammoPool;
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private EnemyPool _bossPool;

    public event Action<Enemy> EnemyDeathFromPool;
    public event Action<Vector3> BossDeath;

    private void OnEnable()
    {
        foreach (Enemy enemy in _enemyPool.ActiveEnemies)
            enemy.EnemyDeath += OnEnemyDeath;

        foreach (Enemy boss in _bossPool.ActiveEnemies)
            boss.EnemyDeath += OnEnemyDeath;
    }

    private void OnDisable()
    {
        foreach (Enemy enemy in _enemyPool.ActiveEnemies)
            enemy.EnemyDeath -= OnEnemyDeath;

        foreach (Enemy boss in _bossPool.ActiveEnemies)
            boss.EnemyDeath -= OnEnemyDeath;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemy.EnemyDeath += OnEnemyDeath;
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemy.EnemyDeath -= OnEnemyDeath;
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        if (enemy.IsBoss)
        {
            BossDeath?.Invoke(enemy.transform.position);

            if (_ammoPool != null && enemy.TryGetComponent<BossAmmoDrop>(out var ammoDrop))
                _ammoPool.SpawnAmmo(enemy.transform.position, ammoDrop.AmmoAmount);
        }

        if (enemy.IsBoss)
            _bossPool.ReturnToPool(enemy);
        else
            _enemyPool.ReturnToPool(enemy);

        EnemyDeathFromPool?.Invoke(enemy);

        enemy.EnemyDeath -= OnEnemyDeath;
    }
}