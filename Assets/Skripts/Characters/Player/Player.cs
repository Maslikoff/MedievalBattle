using System;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerAttacker))]
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    private InputHandler _inputHandler;
    private PlayerMover _mover;
    private PlayerAttacker _attacker;
    private PlayerAnimation _anim;
    private Health _health;

    private bool _isAlive = true;

    public bool IsAlive => _isAlive;
    public int CurrentHealth => _health != null ? _health.CurrentHealth : 0;
    public WeaponType CurrentWeapon => _attacker != null ? _attacker.CurrentWeapon : WeaponType.Firearm;

    public event Action PlayerDeath;
    public event Action<int> HealthChanged;
    public event Action<WeaponType> WeaponSwitched;

    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _mover = GetComponent<PlayerMover>();
        _attacker = GetComponent<PlayerAttacker>();
        _health = GetComponent<Health>();
        _anim = GetComponent<PlayerAnimation>();
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

        _anim.PlayDeathAnimation();

        PlayerDeath?.Invoke();
    }

    private void OnHandleHealthChanged(int currentHealth)
    {
        HealthChanged?.Invoke(currentHealth);
    }

    private void OnHandleWeaponSwitched(WeaponType type)
    {
        WeaponSwitched?.Invoke(type);
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