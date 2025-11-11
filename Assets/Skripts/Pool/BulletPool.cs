using UnityEngine;

public class BulletPool : ObjectPool<Bullet>
{
    [SerializeField] private EffectPool _hitEffectPool;

    protected override void OnObjectGet(Bullet bullet)
    {
        bullet.Owner = null;

        bullet.OnHit += OnBulletHit;
    }

    protected override void OnObjectReturn(Bullet bullet)
    {
        bullet.OnHit -= OnBulletHit;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public Bullet GetBullet(Vector3 position, Quaternion rotation)
    {
        Bullet bullet = GetFromPool();
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;

        return bullet;
    }

    private void OnBulletHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        _hitEffectPool.PlayEffect(hitPoint, Quaternion.LookRotation(hitNormal));
    }
}