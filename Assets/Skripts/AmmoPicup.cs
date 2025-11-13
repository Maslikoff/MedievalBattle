using UnityEngine;

public class AmmoPicup : MonoBehaviour
{
    [SerializeField] private int _ammoAmount = 30;

    private void OnTriggerEnter(Collider other)
    {
        PlayerAttacker player = other.GetComponent<PlayerAttacker>();

        if(player != null)
        {
            player.AddAmmo(_ammoAmount);
            gameObject.SetActive(false);
        }
    }

    public void SetAmmoAmount(int amount)
    {
        _ammoAmount = amount;
    }
}