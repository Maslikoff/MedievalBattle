using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemyAttacker))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(EnemySound))]
[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Settings")]
    [SerializeField] protected int _scoreValue = 100;
    [SerializeField] protected Renderer _enemyRenderer;

    protected EnemyMover _mover;
    protected EnemyAttacker _attacker;
    protected EnemyAnimator _enemyAnimator;
    protected EnemySound _sound;
    protected Health _health;
    protected NavMeshAgent _navMeshAgent;
    protected Transform _playerTarget;
    protected Color _originalColor;

    protected int _maxHealth = 100;
    protected bool _isAlive = true;

    public event Action<Enemy> EnemyDeath;
    public event Action<Enemy> EnemySpawn;

    public abstract bool IsBoss { get; }
    public bool IsAlive => _isAlive;
    public int CurrentHealth => _health != null ? _health.CurrentCount : 0;
    public int ScoreValue => _scoreValue;

    protected virtual void Awake()
    {
        _mover = GetComponent<EnemyMover>();
        _attacker = GetComponent<EnemyAttacker>();
        _health = GetComponent<Health>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyAnimator = GetComponent<EnemyAnimator>();

        _originalColor = _enemyRenderer.material.color;
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
            _health.Changed += OnHandleHealthChanged;
            _health.DamageTaken += OnHandleDamageTaken;
        }
    }

    protected virtual void OnHandleDeath()
    {
        if (_isAlive == false)
            return;

        _isAlive = false;

        _sound?.PlayDeathSound();
        _enemyAnimator.PlayDeathAnimation();

        if (_mover != null)
        {
            _mover.StopMovement();
            _mover.enabled = false;
        }

        if (_attacker != null)
            _attacker.enabled = false;

        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true;

        EnemyDeath?.Invoke(this);
    }

    protected virtual void OnHandleHealthChanged(int currentHealth) { }

    protected virtual void OnHandleDamageTaken(int damage)
    {
        StartCoroutine(DamageFlash());
    }

    protected virtual IEnumerator DamageFlash()
    {
        if (_enemyRenderer == null)
            yield break;

        _enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (_enemyRenderer != null)
            _enemyRenderer.material.color = _originalColor;
    }

    protected virtual void ResetEnemyState()
    {
        _enemyAnimator?.ResetAnimator();

        if (_enemyRenderer != null)
            _enemyRenderer.material.color = _originalColor;

        StopAllCoroutines();
    }

    public virtual void Initialize(Transform playerTarget)
    {
        _playerTarget = playerTarget;
        _isAlive = true;

        ResetEnemyState();

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
    }

    public virtual void ResetEnemy()
    {
        _isAlive = true;

        ResetEnemyState();

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

    public void PlayAttackAnimation()
    {
        if (_isAlive)
            _enemyAnimator.PlayAttackAnimation();
    }

    public float GetHealthPercentage() => _health != null ? (float)_health.CurrentCount / _maxHealth : 0f;

    protected virtual void OnDestroy()
    {
        if (_health != null)
        {
            _health.Death -= OnHandleDeath;
            _health.Changed -= OnHandleHealthChanged;
            _health.DamageTaken -= OnHandleDamageTaken;
        }
    }
}