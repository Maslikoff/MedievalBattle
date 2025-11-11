using System;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerAttacker))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    private InputHandler _inputHandler;
    private PlayerMover _mover;
    private PlayerAttacker _attacker;
    private Health _health;

    private bool _isAlive = true;

    public bool IsAlive => _isAlive;
    public int CurrentHealth => _health != null ? _health.CurrentHealth : 0;
    public WeaponType CurrentWeapon => _attacker != null ? _attacker.CurrentWeapon : WeaponType.Firearm;

    public event Action OnPlayerDeath;
    public event Action<int> OnHealthChanged;
    public event Action<WeaponType> OnWeaponSwitched;

    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _mover = GetComponent<PlayerMover>();
        _attacker = GetComponent<PlayerAttacker>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.Death += OnHandleDeath;
        _health.HealthChanged += OnHandleHealthChanged;
        _attacker.OnWeaponSwitched += OnHandleWeaponSwitched;

        ConnectInputHandler();
    }

    private void ConnectInputHandler()
    {
        _inputHandler.MoveInput += _mover.Move;
        _inputHandler.MouseLookInput += _mover.Look;

        _inputHandler.PrimaryAttack += _attacker.Attack;
        _inputHandler.WeaponSwitch += _attacker.OnWeaponSwitch; 
    }

    private void OnHandleDeath()
    {
        if(_isAlive == false)
            return;

        _isAlive = false;
       
        OnPlayerDeath?.Invoke();
    }

    private void OnHandleHealthChanged(int currentHealth)
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void OnHandleWeaponSwitched(WeaponType type)
    {
        OnWeaponSwitched?.Invoke(type);
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
        {
            _inputHandler.MoveInput -= _mover.Move;
            _inputHandler.MouseLookInput -= _mover.Look;
            _inputHandler.PrimaryAttack -= _attacker.Attack;
            _inputHandler.WeaponSwitch -= _attacker.OnWeaponSwitch;
        }

        if (_health != null)
        {
            _health.Death -= OnHandleDeath;
            _health.HealthChanged -= OnHandleHealthChanged;
        }

        if (_attacker != null)
        {
            _attacker.OnWeaponSwitched -= OnHandleWeaponSwitched;
        }
    }
}