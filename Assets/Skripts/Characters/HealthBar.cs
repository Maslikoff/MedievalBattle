using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Gradient _healthGradient;
    [SerializeField] private bool _smoothChanges = true;
    [SerializeField] private float _smoothSpeed = 2f;

    private float _targetValue;

    private void Start()
    {
        _health.HealthChanged += OnHealthChanged;
        _health.Death += OnDeath;

        _healthSlider.maxValue = _health.MaxHealth;
        _healthSlider.value = _health.CurrentHealth;
        _targetValue = _health.CurrentHealth;

        UpdateHealthColor();
    }

    private void Update()
    {
        if(_smoothChanges && _healthSlider.value != _targetValue)
        {
            _healthSlider.value = Mathf.Lerp(_healthSlider.value, _targetValue, Time.deltaTime * _smoothSpeed);
            UpdateHealthColor();
        }
    }

    private void OnHealthChanged(int currentHealth)
    {
        _targetValue = currentHealth;

        if (_smoothChanges == false)
        {
            _healthSlider.value = currentHealth;
            UpdateHealthColor();
        }
    }

    private void UpdateHealthColor()
    {
        if (_fillImage != null)
        {
            float healthPercentage = _healthSlider.value / _healthSlider.maxValue;
            _fillImage.color = _healthGradient.Evaluate(healthPercentage);
        }
    }

    private void OnDeath()
    {
        _healthSlider.value = 0;
        UpdateHealthColor();
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.HealthChanged -= OnHealthChanged;
            _health.Death -= OnDeath;
        }
    }
}