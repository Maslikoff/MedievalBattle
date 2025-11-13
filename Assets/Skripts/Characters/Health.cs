using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private bool _isInvulnerable = false;

    private int _currentHealth;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;

    public event Action Death;
    public event Action<int> HealthChanged;
    public event Action<int> DamageTaken;

    private void Start()
    {
        _currentHealth = _maxHealth;

        HealthChanged?.Invoke(_currentHealth);
    }

    public int GetCurrentHealth() => _currentHealth;

    public void TakeDamage(int damage)
    {
        if (_isInvulnerable || _currentHealth <= 0)
            return;

        _currentHealth -= damage;

        HealthChanged?.Invoke(_currentHealth);
        DamageTaken?.Invoke(damage);

        if (_currentHealth <= 0)
            Die();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        _maxHealth = newMaxHealth;

        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;

        HealthChanged?.Invoke(_currentHealth);
    }

    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;

        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;

        HealthChanged?.Invoke(_currentHealth);
    }

    private void Die()
    {
        Death?.Invoke();
    }
}