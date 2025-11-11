using System.Collections;
using UnityEngine;

public class PoolableEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;
    //[SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _defaultDuration = 2f;

    private EffectPool _effectPool;
    private Coroutine _autoReturnCoroutine;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
        //_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
            if (_particle != null && _particle.IsAlive() == false)
                ReturnToPool();
    }

    public void Initialize(EffectPool pool)
    {
        _effectPool = pool;
    }

    public void PlayEffect()
    {
        gameObject.SetActive(true);

        _particle.Play();
        // _audioSource.Play();

        if (_autoReturnCoroutine != null)
            StopCoroutine(_autoReturnCoroutine);

        _autoReturnCoroutine = StartCoroutine(AutoReturnAfterDuration());
    }

    public void PlayForDuration(float duration)
    {
        gameObject.SetActive(true);

        _particle.Play();
        //_audioSource.Play();

        if (_autoReturnCoroutine != null)
            StopCoroutine(_autoReturnCoroutine);

        _autoReturnCoroutine = StartCoroutine(AutoReturnAfterDuration(duration));
    }

    public void StopEffect()
    {
        _particle.Stop();
       // _audioSource.Stop();

        CancelInvoke(nameof(ReturnToPool));
    }

    public void ResetEffect()
    {
        StopEffect();

        _particle.Clear();
    }

    private IEnumerator AutoReturnAfterDuration()
    {
        float duration = GetEffectDuration();

        yield return new WaitForSeconds(duration);

        if (_particle != null)
            yield return new WaitWhile(() => _particle.IsAlive(true));

        ReturnToPool();
    }

    private IEnumerator AutoReturnAfterDuration(float customDuration)
    {
        yield return new WaitForSeconds(customDuration);

        if (_particle != null)
            yield return new WaitWhile(() => _particle.IsAlive(true));

        ReturnToPool();
    }

    private float GetEffectDuration()
    {
        if (_particle != null && _particle.main.duration > 0)
            return _particle.main.duration;

       // if (_audioSource != null && _audioSource.clip != null)
        //    return _audioSource.clip.length;

        return _defaultDuration;
    }

    private void ReturnToPool()
    {
        if (_effectPool != null)
            _effectPool.ReturnToPool(this);
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(ReturnToPool));
    }
}