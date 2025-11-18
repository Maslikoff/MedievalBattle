using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip _footstepSound;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private AudioClip _meleeSound;
    [SerializeField] private AudioClip _emptySound;

    [Header("Settings")]
    [SerializeField] private float _footstepDelay = 0.5f;

    private AudioSource _audioSource;

    private float _footstepTimer;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlayFootstepSound();
    }

    public void PlayShootSound()
    {
        _audioSource.PlayOneShot(_shootSound);
    }

    public void PlayMeleeSound()
    {
        _audioSource.PlayOneShot(_meleeSound);
    }

    public void PlayEmptySound()
    {
        _audioSource.PlayOneShot(_emptySound);
    }

    private void PlayFootstepSound()
    {
        bool isMoving = GetComponent<Rigidbody>().velocity.magnitude > 0.1f;

        if (isMoving)
        {
            _footstepTimer -= Time.deltaTime;

            if (_footstepTimer <= 0)
            {
                _audioSource.PlayOneShot(_footstepSound);
                _footstepTimer = _footstepDelay;
            }
        }
    }
}