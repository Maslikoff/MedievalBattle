using UnityEngine;

public class EnemyDefould : Enemy
{
    [SerializeField] private int _normalMaxHealth = 100;

    public override bool IsBoss => false;

    protected override void Awake()
    {
        base.Awake();
        _maxHealth = _normalMaxHealth;
    }

    public override void Initialize(Transform playerTarget)
    {
        base.Initialize(playerTarget);
    }
}