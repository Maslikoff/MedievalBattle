using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemySound : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip _screamSound;
    [SerializeField] private AudioClip _meleeSound;
    [SerializeField] private AudioClip _deathSound;

    [Header("Settings")]
    [SerializeField] private float _screamDelay = 0.5f;

    private AudioSource _audioSource;

    private float _screamTimer;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlayScreamSound();
    }

    public void PlayMeleeSound()
    {
        _audioSource.PlayOneShot(_meleeSound);
    }

    public void PlayDeathSound()
    {
        _audioSource.PlayOneShot(_deathSound);
    }

    private void PlayScreamSound()
    {
        bool isMoving = GetComponent<NavMeshAgent>().velocity.magnitude > 0.1f;

        if (isMoving)
        {
            _screamTimer -= Time.deltaTime;

            if (_screamTimer <= 0)
            {
                _audioSource.PlayOneShot(_screamSound);
                _screamTimer = _screamDelay;
            }
        }
    }
}