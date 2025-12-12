using UnityEngine;

public abstract class Attacker : MonoBehaviour
{
    [SerializeField] protected int Damage = 20;
    [SerializeField] protected float AttackRange = 2f;
    [SerializeField] protected float AttackCooldown = 1f;

    protected float LastAttackTime;

    protected bool IsAttack = true;

    public abstract void Attack();
    public abstract bool CanAttack();

    protected bool IsCooldownReady() => Time.time >= LastAttackTime + AttackCooldown;

    protected bool IsTargetInRange(Transform target)
    {
        if (target == null)
            return false;

        float distanceSqr = (transform.position - target.position).sqrMagnitude;

        return distanceSqr <= AttackRange * AttackRange;
    }

    protected void ApplyDamageToTarget(Transform target)
    {
        if (target.TryGetComponent<Health>(out var hp))
        {
            hp.TakeDamage(Damage);
            OnSuccessfulAttack(target);
        }
    }

    protected virtual void OnSuccessfulAttack(Transform target) { }

    public void SetCanAttack(bool state)
    {
        IsAttack = state;
    }

    public virtual void IncreaseDamage(float multiplier) 
    {
        Damage = Mathf.RoundToInt(Damage * multiplier);
    }

    public float GetCooldownProgress()
    {
        float timeSinceLastAttack = Time.time - LastAttackTime;

        return Mathf.Clamp01(timeSinceLastAttack / AttackCooldown);
    }
}