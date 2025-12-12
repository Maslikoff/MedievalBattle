using System;
using UnityEngine;

public class HealthPackPool : ObjectPool<HealthPack>
{
    public event Action HealthPackTaken;

    protected override void OnObjectGet(HealthPack healthPack)
    {
        base.OnObjectGet(healthPack);

        healthPack.HealthPackTaken += OnHealthPackTaken;
    }

    protected override void OnObjectReturn(HealthPack healthPack)
    {
        base.OnObjectReturn(healthPack);

        healthPack.HealthPackTaken -= OnHealthPackTaken;
    }


    public void SpawnHealthPack(Vector3 position)
    {
        HealthPack healthPack = GetFromPool();
        healthPack.transform.position = position;
        healthPack.gameObject.SetActive(true);
    }

    private void OnHealthPackTaken()
    {
        HealthPackTaken?.Invoke();
    }
}