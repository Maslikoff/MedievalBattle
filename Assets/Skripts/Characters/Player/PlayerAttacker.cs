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

    [Header("Attack Settings")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private LayerMask _enemyLayerMask;

    [Header("Weapon Models")]
    [SerializeField] private Weapon _firearmModel;
    [SerializeField] private Weapon _meleeWeaponModel;

    [Header("Dependencies")]
    [SerializeField] private BulletPool _bulletPool;

    public event Action<WeaponType> OnWeaponSwitched;

    private Player _player;
    private Camera _playerCamera;
    private WeaponType _currentWeapon = WeaponType.Firearm;

    public WeaponType CurrentWeapon => _currentWeapon;

    private void Start()
    {
        _player = GetComponent<Player>();
        _playerCamera = Camera.main;

        UpdateWeaponVisuals();
    }

    public override void Attack()
    {
        if (CanAttack() == false || CanPerformActions() == false)
            return;

        switch(_currentWeapon)
        {
            case WeaponType.Firearm:
                PerformFirearmAttack();
                break;

            case WeaponType.Melee:
                PerformMeleeAttack();
                break;
        }

        _lastAttackTime = Time.time;
    }

    public void OnWeaponSwitch()
    {
        _currentWeapon = _currentWeapon == WeaponType.Firearm ? WeaponType.Melee : WeaponType.Firearm;
        UpdateWeaponVisuals();
    }

    public override bool CanAttack() => _canAttack && IsCooldownReady();

    private bool CanPerformActions() => _player != null && _player.IsAlive;

    private void PerformFirearmAttack()
    {
        if(_bulletPool == null || _firePoint == null)
            return;

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
        if(_playerCamera == null)
            return;

        RaycastHit hit;
        Vector3 rayOrigin = _playerCamera.transform.position;
        Vector3 rayDirection = _playerCamera.transform.forward;

        if(Physics.Raycast(rayOrigin, rayDirection, out hit, _attackRange, _enemyLayerMask))
            ApplyDamageToTarget(hit.collider.transform);
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