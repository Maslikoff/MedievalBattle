using System;
using UnityEngine;

public enum WeaponType
{
    Firearm,
    Melee
}

public class PlayerAttacker : Attacker
{
    private const float Diference = 1.5f;
    private const float MaxDistance = 1000f;
    private const float Balance = 100f;
    private const float MiddleScreen = 2f;
    private const float AnimationDelay = 1f;

    [Header("Attack Settings")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private LayerMask _enemyLayerMask;

    [Header("Ammo Settings")]
    [SerializeField] private int _maxAmmo = 120;

    [Header("Weapon Models")]
    [SerializeField] private Weapon _firearmModel;
    [SerializeField] private Weapon _meleeWeaponModel;

    [Header("Dependencies")]
    [SerializeField] private BulletPool _bulletPool;

    private Player _player;
    private PlayerAnimation _playerAnimation;
    private Camera _playerCamera;
    private WeaponType _currentWeapon = WeaponType.Firearm;

    private int _currentAmmo;
    private bool _isAttacking;

    public event Action<WeaponType> OnWeaponSwitched;
    public event Action<int> OnAmmoChanged;

    public WeaponType CurrentWeapon => _currentWeapon;
    public int CurrentAmmo => _currentAmmo;
    public int MaxAmmo => _maxAmmo;

    private void Start()
    {
        _player = GetComponent<Player>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _playerCamera = Camera.main;

        _currentAmmo = _maxAmmo;

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

        _lastAttackTime = Time.time;
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
        OnAmmoChanged?.Invoke(_currentAmmo);
    }

    public override bool CanAttack() => _canAttack && IsCooldownReady();

    private bool CanPerformActions() => _player != null && _player.IsAlive;

    private void PerformFirearmAttack()
    {
        if (_bulletPool == null || _firePoint == null)
            return;

        if (_currentAmmo <= 0)
        {
            _isAttacking = false;
            return;
        }

        _currentAmmo--;
        OnAmmoChanged?.Invoke(_currentAmmo);

        _playerAnimation.PlayShootAnimation();

        Vector3 targetPoint = GetCenterScreenPoint();
        Vector3 shootDirection = (targetPoint - GetShootPosition()).normalized;

        Bullet bullet = _bulletPool.GetBullet(GetShootPosition(), Quaternion.LookRotation(shootDirection));
        bullet.Owner = gameObject;
        bullet.SetSpeed(_bulletSpeed);

        Collider playerCollider = GetComponent<Collider>();

        if (playerCollider != null)
            bullet.IgnoreCollision(playerCollider);
    }

    private void PerformMeleeAttack()
    {
        _playerAnimation.PlayMeleeAttackAnimation();

        Invoke(nameof(ApplyMeleeDamage), AnimationDelay);
    }

    private void ApplyMeleeDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyLayerMask);

        foreach (Collider collider in hitColliders)
        {
            Vector3 directionToEnemy = (collider.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToEnemy);

            if (dotProduct > 0)
                ApplyDamageToTarget(collider.transform);
        }
    }

    private void UpdateWeaponVisuals()
    {
        if (_firearmModel != null)
            _firearmModel.gameObject.SetActive(_currentWeapon == WeaponType.Firearm);

        if (_meleeWeaponModel != null)
            _meleeWeaponModel.gameObject.SetActive(_currentWeapon == WeaponType.Melee);
    }

    private Vector3 GetCenterScreenPoint()
    {
        if (_playerCamera == null)
            return transform.position + transform.forward * Balance;

        Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / MiddleScreen, Screen.height / MiddleScreen, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MaxDistance, _enemyLayerMask))
            return hit.point;

        return ray.origin + ray.direction * Balance;
    }

    private Vector3 GetShootPosition()
    {
        if (_firePoint != null)
            return _firePoint.position;

        if (_playerCamera != null)
            return _playerCamera.transform.position;

        return transform.position + Vector3.up * Diference;
    }
}