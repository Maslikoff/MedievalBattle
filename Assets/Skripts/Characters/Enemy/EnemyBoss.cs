using UnityEngine;

public class EnemyBoss : Enemy
{
    [SerializeField] private int _bossMaxHealth = 300;
    [SerializeField] private int _bossScoreValue = 500;
    [SerializeField] private float _bossScale = 1.5f;
    [SerializeField] private float _damageMultiplier = 2f;
    [SerializeField] private Material _bossMaterial;

    private Material _originalMaterial;

    public override bool IsBoss => true;

    protected override void Awake()
    {
        base.Awake();

        _maxHealth = _bossMaxHealth;
        _scoreValue = _bossScoreValue;
        _originalMaterial = _enemyRenderer.material;
    }

    protected override void OnHandleHealthChanged(int currentHealth)
    {
        Debug.Log($"Boss health: {currentHealth}/{_maxHealth}");
    }

    protected override void OnHandleDeath()
    {
        base.OnHandleDeath();
        Debug.Log("BOSS DEFEATED!");
    }

    public override void Initialize(Transform playerTarget)
    {
        _playerTarget = playerTarget;
        _isAlive = true;

        if (_mover != null)
        {
            _mover.enabled = true;
            _mover.ResumeMovement();
            _mover.SetPlayer(_playerTarget);
        }

        if (_attacker != null)
        {
            _attacker.enabled = true;
            _attacker.SetCanAttack(true);
            _attacker.SetPlayer(_playerTarget);
            _attacker.IncreaseDamage(_damageMultiplier);
        }

        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = false;

        if (_health != null)
        {
            _health.SetMaxHealth(_maxHealth);
            _health.Heal(_maxHealth);
        }

        SetupBossVisuals();
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        SetupBossVisuals();
    }

    private void SetupBossVisuals()
    {
        if (_bossMaterial != null && _enemyRenderer != null)
            _enemyRenderer.material = _bossMaterial;

        transform.localScale = Vector3.one * _bossScale;
    }
}