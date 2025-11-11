using UnityEngine;

public class EffectPool : ObjectPool<PoolableEffect>
{
    protected override void OnObjectGet(PoolableEffect effect)
    {
        effect.Initialize(this);
        effect.ResetEffect();
    }

    protected override void OnObjectReturn(PoolableEffect effect)
    {
        effect.StopEffect();
    }

    public void PlayEffect(Vector3 position, Quaternion rotation)
    {
        PoolableEffect effect = GetFromPool();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.PlayEffect();
    }

    public void PlayEffect(Vector3 position, Quaternion rotation, float duration)
    {
        PoolableEffect effect = GetFromPool();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.PlayForDuration(duration);
    }
}