using UnityEngine;

public class HealthPackPool : ObjectPool<HealthPack>
{
    protected override void OnObjectGet(HealthPack healthPack)
    {
        healthPack.transform.position = Vector3.zero;
        healthPack.transform.rotation = Quaternion.identity;
    }

    public void SpawnHealthPack(Vector3 position)
    {
        HealthPack healthPack = GetFromPool();
        healthPack.transform.position = position;
    }
}