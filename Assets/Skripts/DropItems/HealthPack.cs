using System;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequency = 1f;
    [SerializeField] private AudioClip _pickupSound;

    private AudioSource _audioSource;
    private Vector3 _startPosition;

    public event Action HealthPackTaken;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);

        float newY = _startPosition.y + Mathf.Sin(Time.time * _frequency) * _amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health player = other.GetComponent<Health>();

        if (player != null && player.CurrentCount < player.MaxCount)
        {
            _audioSource.PlayOneShot(_pickupSound);
            player.Heal(healAmount);
            OnHealthPackTaken();
        }
    }

    private void OnHealthPackTaken()
    {
        HealthPackTaken?.Invoke();
        gameObject.SetActive(false);
    }
}