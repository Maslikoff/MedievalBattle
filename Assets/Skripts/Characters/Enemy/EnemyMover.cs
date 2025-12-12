using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMover : Mover
{
    [SerializeField] private float _stoppingDistance = 1f;

    private NavMeshAgent _navMeshAgent;
    private Transform _playerTarget;
    private Enemy _enemy;

    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemy = GetComponent<Enemy>();

        if (_navMeshAgent != null)
        {
            _navMeshAgent.speed = MoveSpeed;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
            _navMeshAgent.acceleration = 8f;
            _navMeshAgent.angularSpeed = 120f;
        }
    }

    private void Update()
    {
        if (CanMove() == false || _playerTarget == null) 
            return;

        if (_navMeshAgent != null && _navMeshAgent.isActiveAndEnabled)
        {
            _navMeshAgent.SetDestination(_playerTarget.position);

            if (_navMeshAgent.velocity.magnitude > 0.1f)
            {
                Vector3 direction = (_playerTarget.position - transform.position).normalized;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
        }
    }

    public override void Move(Vector2 input) { }

    public override void Look(Vector2 mouseInput) { }

    public override void StopMovement()
    {
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = true;
    }

    public override void ResumeMovement()
    {
        if (_navMeshAgent != null)
            _navMeshAgent.isStopped = false;
    }

    public void SetPlayer(Transform playerTransform)
    {
        _playerTarget = playerTransform;
    }

    public float GetDistanceToPlayer()
    {
        if (_playerTarget == null)
            return Mathf.Infinity;

        return Vector3.Distance(transform.position, _playerTarget.position);
    }

    public bool IsPlayerInRange(float range)
    {
        if (_playerTarget == null)
            return false;

        return Vector3.Distance(transform.position, _playerTarget.position) <= range;
    }

    private bool CanMove() => _enemy != null && _enemy.IsAlive;
}