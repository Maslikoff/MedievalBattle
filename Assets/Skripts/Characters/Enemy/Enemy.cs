using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemyAttacker))]
[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    [SerializeField] protected int _scoreValue = 100;
    [SerializeField] protected GameObject _deathEffect;

    protected EnemyMover _mover;
    protected EnemyAttacker _attacker;
    protected Health _health;
    protected NavMeshAgent _navMeshAgent;
    protected Renderer _enemyRenderer;
    protected Transform _playerTarget;

    protected int _maxHealth = 100;
    protected bool _isAlive = true;

    public event Action<Enemy> EnemyDeath;
    public event Action<Enemy> EnemySpawn;

    public abstract bool IsBoss { get; }
    public bool IsAlive => _isAlive;
    public int CurrentHealth => _health != null ? _health.CurrentHealth : 0;
    public int ScoreValue => _scoreValue;

    protected virtual void Awake()
    {
        _mover = GetComponent<EnemyMover>();
        _attacker = GetComponent<EnemyAttacker>();
        _health = GetComponent<Health>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyRenderer = GetComponentInChildren<Renderer>();
    }

    protected virtual void Start()
    {
        SubscribeToEvents();
        EnemySpawn?.Invoke(this);
    }

    protected virtual void SubscribeToEvents()
    {
        if (_health != null)
        {
            _health.Death += OnHandleDeath;
            _health.HealthChanged += OnHandleHealthChanged;
            _health.DamageTaken += OnHandleDamageTaken;
        }
    }

    protected virtual void OnHandleDeath()
    {
        if (_isAlive == false)
            return;

        _isAlive = false;

        if (_mover != null)
        {
            _mover.StopMovement();
            _mover.enabled = false;
        }

        if (_attacker != null)
            _attacker.enabled = false;

        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true;

        if (_deathEffect != null)
            Instantiate(_deathEffect, transform.position, Quaternion.identity);

        EnemyDeath?.Invoke(this);
    }

    protected virtual void OnHandleHealthChanged(int currentHealth) {}

    protected virtual void OnHandleDamageTaken(int damage)
    {
        StartCoroutine(DamageFlash());
    }

    protected virtual System.Collections.IEnumerator DamageFlash()
    {
        if (_enemyRenderer == null)
            yield break;

        Color originalColor = _enemyRenderer.material.color;
        _enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (_enemyRenderer != null)
            _enemyRenderer.material.color = originalColor;
    }

    public virtual void Initialize(Transform playerTarget)
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
        }

        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = false;

        if (_health != null)
        {
            _health.SetMaxHealth(_maxHealth);
            _health.Heal(_maxHealth);
        }

        Debug.Log($"{GetType().Name} initialized");
    }

    public virtual void ResetEnemy()
    {
        _isAlive = true;

        if (_health != null)
        {
            _health.SetMaxHealth(_maxHealth);
            _health.Heal(_maxHealth);
        }
    }

    public virtual void StopEnemy()
    {
        if (_mover != null)
            _mover.StopMovement();

        if (_attacker != null)
            _attacker.SetCanAttack(false);
    }

    public virtual void ResumeEnemy()
    {
        if (_mover != null)
            _mover.ResumeMovement();

        if (_attacker != null)
            _attacker.SetCanAttack(true);
    }

    public float GetHealthPercentage() => _health != null ? (float)_health.CurrentHealth / _maxHealth : 0f;

    protected virtual void OnDestroy()
    {
        if (_health != null)
        {
            _health.Death -= OnHandleDeath;
            _health.HealthChanged -= OnHandleHealthChanged;
            _health.DamageTaken -= OnHandleDamageTaken;
        }
    }
}