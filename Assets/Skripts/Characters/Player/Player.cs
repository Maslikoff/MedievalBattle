using System;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerAttacker))]
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerSound))]
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

    public event Action PlayerDeath;
    public event Action<int> HealthChanged;
    public event Action<WeaponType> WeaponSwitched;

    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _mover = GetComponent<PlayerMover>();
        _attacker = GetComponent<PlayerAttacker>();
        _anim = GetComponent<PlayerAnimation>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        _health.Death += OnHandleDeath;
        _health.Changed += OnHandleHealthChanged;
        _attacker.WeaponSwitched += OnHandleWeaponSwitched;

        ConnectInputHandler();
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
        {
            _inputHandler.MoveInput -= OnMoveInput;
            _inputHandler.MouseLookInput -= _mover.Look;
            _inputHandler.PrimaryAttack -= _attacker.Attack;
            _inputHandler.WeaponSwitch -= _attacker.OnWeaponSwitch;
        }

        if (_health != null)
        {
            _health.Death -= OnHandleDeath;
            _health.Changed -= OnHandleHealthChanged;
        }

        if (_attacker != null)
        {
            _attacker.WeaponSwitched -= OnHandleWeaponSwitched;
        }
    }

    private void ConnectInputHandler()
    {
        _inputHandler.MoveInput += OnMoveInput;
        _inputHandler.MouseLookInput += _mover.Look;

        _inputHandler.PrimaryAttack += _attacker.Attack;
        _inputHandler.WeaponSwitch += _attacker.OnWeaponSwitch; 
    }

    private void OnMoveInput(Vector2 input)
    {
        if (_isAlive == false) 
            return;

        _mover.Move(input);
        _anim.SetMovementSpeed(input.magnitude);
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
        _anim.SetWeaponType(type);
        WeaponSwitched?.Invoke(type);
    }
}