using UnityEngine;

public class BulletPool : ObjectPool<Bullet>
{
    [SerializeField] private EffectPool _hitEffectPool;

    public Bullet GetBullet(Vector3 position, Quaternion rotation)
    {
        Bullet bullet = GetFromPool();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;

        return bullet;
    }

    protected override void OnObjectGet(Bullet bullet)
    {
        bullet.Owner = null;

        bullet.Hit += OnBulletHit;
    }

    protected override void OnObjectReturn(Bullet bullet)
    {
        bullet.Hit -= OnBulletHit;

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnBulletHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        _hitEffectPool.PlayEffect(hitPoint, Quaternion.LookRotation(hitNormal));
    }
}