using UnityEngine;

public class AmmoDropPool : ObjectPool<AmmoPicup>
{
    protected override void OnObjectGet(AmmoPicup ammoPickup)
    {
        ammoPickup.gameObject.SetActive(true);
    }

    protected override void OnObjectReturn(AmmoPicup ammoPickup)
    {
        ammoPickup.gameObject.SetActive(false);
    }

    public void SpawnAmmo(Vector3 position, int amount)
    {
        AmmoPicup ammoPickup = GetFromPool();
        ammoPickup.transform.position = position;
        ammoPickup.SetAmmoAmount(amount);
    }
}