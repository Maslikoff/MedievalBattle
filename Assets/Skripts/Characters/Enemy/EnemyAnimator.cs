using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string _moveSpeedParam = "MoveSpeed";
    [SerializeField] private string _isAttackingParam = "IsAttacking";
    [SerializeField] private string _isDeadParam = "IsDead";
    [SerializeField] private string _takeDamageParam = "TakeDamage";

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private float _currentSpeed;
    private bool _isAlive = true;

    private void Update()
    {
        if (_isAlive == false)
            return;

        UpdateMovementAnimation();
    }

    public void PlayAttackAnimation()
    {
        if (_isAlive == false)
            return;

        _animator.SetBool(_isAttackingParam, true);
        StartCoroutine(ResetAttackAnimation());
    }

    public void PlayDamageAnimation()
    {
        if (_isAlive == false)
            return;

        _animator.SetTrigger(_takeDamageParam);
    }

    public void PlayDeathAnimation()
    {
        _isAlive = false;
        _animator.SetBool(_isDeadParam, true);
        _animator.SetFloat(_moveSpeedParam, 0f);
    }

    public void ResetAnimator()
    {
        _isAlive = true;
        _animator.SetBool(_isDeadParam, false);
        _animator.SetFloat(_moveSpeedParam, 0f);
        _animator.SetBool(_isAttackingParam, false);
    }

    private void UpdateMovementAnimation()
    {
        if (_navMeshAgent != null && _animator != null)
        {
            Vector3 horizontalVelocity = new Vector3(_navMeshAgent.velocity.x, 0, _navMeshAgent.velocity.z);
            _currentSpeed = horizontalVelocity.magnitude;

            _animator.SetFloat(_moveSpeedParam, _currentSpeed);
        }
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool(_isAttackingParam, false);
    }
}