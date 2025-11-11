using UnityEngine;

public abstract class Attacker : MonoBehaviour
{
    [SerializeField] protected int _damage = 20;
    [SerializeField] protected float _attackRange = 2f;
    [SerializeField] protected float _attackCooldown = 1f;

    protected float _lastAttackTime;

    protected bool _canAttack = true;

    public abstract void Attack();
    public abstract bool CanAttack();

    protected bool IsCooldownReady()
    {
        return Time.time >= _lastAttackTime + _attackCooldown;
    }

    protected bool IsTargetInRange(Transform target)
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) <= _attackRange;
    }

    protected void ApplyDamageToTarget(Transform target)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(_damage);
            OnSuccessfulAttack(target);
        }
    }

    protected virtual void OnSuccessfulAttack(Transform target) { }

    public void SetCanAttack(bool state)
    {
        _canAttack = state;
    }

    public virtual void IncreaseDamage(float multiplier)
    {
    }

    public float GetCooldownProgress()
    {
        float timeSinceLastAttack = Time.time - _lastAttackTime;

        return Mathf.Clamp01(timeSinceLastAttack / _attackCooldown);
    }
}