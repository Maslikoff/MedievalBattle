using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using UnityEngine;

public enum WeaponType
{
    Firearm,
    Melee
}

[RequireComponent(typeof(AimHelper))]
public class PlayerAttacker : Attacker
{
    private const float Diference = 1.5f;
    private const float MaxDistance = 1000f;
    private const float Balance = 100f;
    private const float MiddleScreen = 2f;
    private const float AnimationDelay = 1f;
    private const int MaxHitColliders = 10;

    [Header("Attack Settings")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private LayerMask _bulletIgnoreLayerMask;

    [Header("Ammo Settings")]
    [SerializeField] private int _maxAmmo = 120;
    [SerializeField] private int _currentAmmo;

    [Header("Weapon Models")]
    [SerializeField] private Weapon _firearmModel;
    [SerializeField] private Weapon _meleeWeaponModel;

    [Header("Dependencies")]
    [SerializeField] private BulletPool _bulletPool;

    private AimHelper _aimHelper;
    private Player _player;
    private PlayerAnimation _playerAnimation;
    private PlayerSound _playerSound;
    private Camera _playerCamera;
    private WeaponType _currentWeapon = WeaponType.Firearm;

    private Collider[] _hitColliders;
    private Coroutine _meleeAttackCoroutine;

    private bool _isAttacking;

    public event Action<WeaponType> WeaponSwitched;
    public event Action<int> AmmoChanged;

    public WeaponType CurrentWeapon => _currentWeapon;
    public int CurrentAmmo => _currentAmmo;
    public int MaxAmmo => _maxAmmo;

    private void Start()
    {
        _player = GetComponent<Player>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerSound = GetComponent<PlayerSound>();
        _playerCamera = Camera.main;

        _hitColliders = new Collider[MaxHitColliders];

        UpdateWeaponVisuals();
    }

    public override void Attack()
    {
        if (CanAttack() == false || CanPerformActions() == false || _isAttacking)
            return;

        _isAttacking = true;

        switch (_currentWeapon)
        {
            case WeaponType.Firearm:
                PerformFirearmAttack();
                break;

            case WeaponType.Melee:
                PerformMeleeAttack();
                break;
        }

        LastAttackTime = Time.time;
        _isAttacking = false;
    }

    public void OnWeaponSwitch()
    {
        _currentWeapon = _currentWeapon == WeaponType.Firearm ? WeaponType.Melee : WeaponType.Firearm;
        UpdateWeaponVisuals();
    }

    public void AddAmmo(int amount)
    {
        _currentAmmo = Mathf.Clamp(_currentAmmo + amount, 0, _maxAmmo);
        AmmoChanged?.Invoke(_currentAmmo);
    }

    public override bool CanAttack() => IsAttack && IsCooldownReady();

    private bool CanPerformActions() => _player != null && _player.IsAlive;

    private void PerformFirearmAttack()
    {
        if (_bulletPool == null || _firePoint == null)
            return;

        if (_currentAmmo <= 0)
        {
            _playerSound?.PlayEmptySound();
            _isAttacking = false;
            return;
        }

        _currentAmmo--;
        AmmoChanged?.Invoke(_currentAmmo);

        _playerAnimation.PlayShootAnimation();

        _playerSound?.PlayShootSound();

        Vector3 shootDirection = _aimHelper.GetAimDirection(_firePoint.position, _enemyLayerMask);

        Bullet bullet = _bulletPool.GetBullet(GetShootPosition(), Quaternion.LookRotation(shootDirection));
        bullet.Owner = gameObject;
        bullet.SetSpeed(_bulletSpeed);

        bullet.SetIgnoreLayerMask(_bulletIgnoreLayerMask);
    }

    private void PerformMeleeAttack()
    {
        _playerAnimation.PlayMeleeAttackAnimation();
        _playerSound?.PlayMeleeSound();

        if (_meleeAttackCoroutine != null)
            StopCoroutine(_meleeAttackCoroutine);

        _meleeAttackCoroutine = StartCoroutine(MeleeAttackRoutine());
    }

    private IEnumerator MeleeAttackRoutine()
    {
        yield return new WaitForSeconds(AnimationDelay);

        ApplyMeleeDamage();

        _isAttacking = false;
        _meleeAttackCoroutine = null;
    }

    private void ApplyMeleeDamage()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, AttackRange, _hitColliders, _enemyLayerMask);

        for (int i = 0; i < numColliders; i++)
        {
            if (_hitColliders[i] != null)
            {
                Vector3 directionToEnemy = (_hitColliders[i].transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToEnemy);

                if (dotProduct > 0)
                    ApplyDamageToTarget(_hitColliders[i].transform);
            }
        }
    }

    private void UpdateWeaponVisuals()
    {
        if (_firearmModel != null)
            _firearmModel.gameObject.SetActive(_currentWeapon == WeaponType.Firearm);

        if (_meleeWeaponModel != null)
            _meleeWeaponModel.gameObject.SetActive(_currentWeapon == WeaponType.Melee);
    }

    private Vector3 GetShootPosition()
    {
        if (_firePoint != null)
            return _firePoint.position;

        if (_playerCamera != null)
            return _playerCamera.transform.position;

        return transform.position + Vector3.up * Diference;
    }

    private void OnDisable()
    {
        if (_meleeAttackCoroutine != null)
        {
            StopCoroutine(_meleeAttackCoroutine);
            _meleeAttackCoroutine = null;
        }
    }
}