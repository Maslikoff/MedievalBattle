using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private float _maxVolume = 0f;
    [SerializeField] private float _minVolume = -80f;

    private void Start()
    {
        LoadAllVolumes();
    }

    public void ChangeMasterVolume(float volume)
    {
        ChangeVolume(AudioConstants.MasterVolume, volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        ChangeVolume(AudioConstants.MusicVolume, volume);
    }

    public void ChangeEffectVolume(float volume)
    {
        ChangeVolume(AudioConstants.EffectsVolume, volume);
    }

    public void ChangeVolume(string parameter, float volume)
    {
        _audioMixer.SetFloat(parameter, Mathf.Lerp(_minVolume, _maxVolume, volume));

        PlayerPrefs.SetFloat(parameter, volume);
        PlayerPrefs.Save();
    }

    private void LoadAllVolumes()
    {
        LoadVolume(AudioConstants.MasterVolume);
        LoadVolume(AudioConstants.MusicVolume);
        LoadVolume(AudioConstants.EffectsVolume);
    }

    private void LoadVolume(string volumeKey)
    {
        if (PlayerPrefs.HasKey(volumeKey))
        {
            float volume = PlayerPrefs.GetFloat(volumeKey, AudioConstants.DefaultVolume);
            ChangeVolume(volumeKey, volume);
        }
    }
}