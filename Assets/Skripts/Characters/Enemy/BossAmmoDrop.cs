using UnityEngine;

public class BossAmmoDrop : MonoBehaviour
{
    [SerializeField] private int _minAmmoDrop = 1;
    [SerializeField] private int _maxAmmoDrop = 3;
    [SerializeField] private int _dropCount = 3;

    private Health _health;
    private Enemy _enemy;

    private void Start()
    {
        _health = GetComponent<Health>();
        _enemy = GetComponent<Enemy>();

        _health.Death += OnDeath;
    }

    private void OnDeath()
    {
        if (_enemy != null && _enemy.IsBoss)
        {
            for (int i = 0; i < _dropCount; i++)
            {
                Vector3 spawnPosition = CalculateSpawnPosition(i);
                int ammoAmount = Random.Range(_minAmmoDrop, _maxAmmoDrop + 1);
                TriggerAmmoDrop(spawnPosition, ammoAmount);
            }
        }
    }

    private void TriggerAmmoDrop(Vector3 position, int amount)
    {
        EnemyPool enemyPool = GetComponentInParent<EnemyPool>();

        if (enemyPool != null)
            enemyPool.OnBossDeathWithAmmo(position, amount);
    }

    private Vector3 CalculateSpawnPosition(int index)
    {
        float angle = (index * 360f / _dropCount) * Mathf.Deg2Rad;
        float radius = 1.5f;

        Vector3 bossPosition = transform.position;
        Vector3 horizontalOffset = new Vector3(Mathf.Cos(angle) * radius, 1f, Mathf.Sin(angle) * radius);

        return bossPosition + horizontalOffset + Vector3.up;
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.Death -= OnDeath;
    }
}