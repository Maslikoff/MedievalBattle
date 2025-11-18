using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string _mixerParameterName;

    [SerializeField] private VolumeControl _volumeControl;
    [SerializeField] private Slider _slider;

    private void Start()
    {
        SetupSliders();
        LoadSavedValues();
    }

    private void SetupSliders()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        if (_volumeControl == null || string.IsNullOrEmpty(_mixerParameterName))
            return;

        switch (_mixerParameterName)
        {
            case AudioConstants.MasterVolume:
                _volumeControl.ChangeMasterVolume(value);
                break;
            case AudioConstants.MusicVolume:
                _volumeControl.ChangeMusicVolume(value);
                break;
            case AudioConstants.EffectsVolume:
                _volumeControl.ChangeEffectVolume(value);
                break;
            default:
                _volumeControl.ChangeVolume(_mixerParameterName, value);
                break;
        }
    }

    private void LoadSavedValues()
    {
        if (_slider == null || string.IsNullOrEmpty(_mixerParameterName))
            return;

        float savedValue = PlayerPrefs.GetFloat(_mixerParameterName, AudioConstants.DefaultVolume);
        _slider.value = savedValue;
    }
}