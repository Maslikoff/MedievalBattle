using UnityEngine;

public class EnemyAttacker : Attacker
{
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private int _baseDamage = 10;

    private Transform _playerTarget;
    private Enemy _enemy;
    private EnemySound _sound;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();
        _sound = GetComponent<EnemySound>();
    }

    private void Update()
    {
        if (CanPerformActions() == false || _playerTarget == null)
            return;

        if (IsPlayerInRange() && CanSeePlayer())
            Attack();
    }

    public override void Attack()
    {
        if (CanAttack() == false || CanPerformActions() == false)
            return;

        PerformMeleeAttack();
        LastAttackTime = Time.time;

        _enemy?.PlayAttackAnimation();
    }

    public override void IncreaseDamage(float multiplier)
    {
        _baseDamage = Mathf.RoundToInt(_baseDamage * multiplier);
    }

    public override bool CanAttack() => IsAttack && IsCooldownReady() && _playerTarget != null;

    private void PerformMeleeAttack()
    {
        if (_playerTarget == null)
            return;

        _sound.PlayMeleeSound();

        if (IsTargetInRange(_playerTarget) && CanSeePlayer())
            ApplyDamageToTarget(_playerTarget);
    }

    private bool IsPlayerInRange() => IsTargetInRange(_playerTarget);

    private bool CanSeePlayer()
    {
        if (_playerTarget == null) 
            return false;

        RaycastHit hit;
        Vector3 directionToPlayer = (_playerTarget.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, _detectionRange))
            return hit.collider.CompareTag("Player");

        return false;
    }

    protected override void OnSuccessfulAttack(Transform target) { }

    public void SetPlayer(Transform playerTransform)
    {
        _playerTarget = playerTransform;
    }

    private bool CanPerformActions() => _enemy != null && _enemy.IsAlive;
}