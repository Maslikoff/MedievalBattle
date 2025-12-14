using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPool : ObjectPool<Enemy>
{
    [SerializeField] private bool _isBossPool = false;
    [SerializeField] private Transform _playerTarget; 
    [SerializeField] private HandlingEnemyDeath _deathHandler;

    private List<Enemy> _activeEnemies = new List<Enemy>();

    public IEnumerable<object> ActiveEnemies => _activeEnemies;
    public bool IsBossPool => _isBossPool;

    public event Action<Enemy> EnemyDeathFromPool;
    public event Action<Vector3> BossDeath;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnObjectGet(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.Initialize(_playerTarget);
            _activeEnemies.Add(enemy);
            _deathHandler?.RegisterEnemy(enemy);
        }
    }

    protected override void OnObjectReturn(Enemy enemy)
    {
        base.OnObjectReturn(enemy);

        if (enemy != null)
        {
            enemy.ResetEnemy();

            _deathHandler?.UnregisterEnemy(enemy);

            NavMeshAgent navMeshAgent = enemy.GetComponent<NavMeshAgent>();

            if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.velocity = Vector3.zero;
            }
        }

        _activeEnemies.Remove(enemy);
    }

    public override Enemy GetFromPool() => base.GetFromPool();

    public override void ReturnToPool(Enemy enemy)
    {
        if (enemy == null)
            return;

        base.ReturnToPool(enemy);
    }

    public void SpawnAtPosition(Vector3 position)
    {
        Enemy enemy = GetFromPool();

        if (enemy != null)
        {
            NavMeshAgent navMeshAgent = enemy.GetComponent<NavMeshAgent>();

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
                enemy.transform.position = position;
                enemy.transform.rotation = Quaternion.identity;
                navMeshAgent.enabled = true;

                StartCoroutine(ActivateAgentNextFrame(navMeshAgent));
            }

            enemy.ResumeEnemy();
        }
    }

    public void ClearAllActiveEnemies()
    {
        foreach (Enemy enemy in _activeEnemies.ToArray())
            if (enemy != null)
                ReturnToPool(enemy);

        _activeEnemies.Clear();
    }

    private IEnumerator ActivateAgentNextFrame(NavMeshAgent agent)
    {
        yield return null; 

        if (agent != null && agent.isActiveAndEnabled)
            agent.isStopped = false;
    }
}