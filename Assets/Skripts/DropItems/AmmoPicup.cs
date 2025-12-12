using UnityEngine;

public class AmmoPicup : MonoBehaviour
{
    [SerializeField] private int _ammoAmount = 30;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private AudioClip _pickupSound;

    private AudioSource _audioSource;
    private Vector3 _startPosition;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);

        float newY = (_startPosition.y + 1.5f) + Mathf.Sin(Time.time * _frequency) * _amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerAttacker player = other.GetComponent<PlayerAttacker>();

        if (player != null)
        {
            _audioSource.PlayOneShot(_pickupSound);
            player.AddAmmo(_ammoAmount);
            gameObject.SetActive(false);
        }
    }

    public void SetAmmoAmount(int amount)
    {
        _ammoAmount = amount;
    }
}