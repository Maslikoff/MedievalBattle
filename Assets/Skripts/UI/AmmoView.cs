using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAttacker _playerAttacker;
    [SerializeField] private TextMeshProUGUI _ammoText;

    [Header("Settings")]
    [SerializeField] private bool _showText = true;
    [SerializeField] private string _format = "{0}/{1}";

    [Header("Low Ammo Warning")]
    [SerializeField] private bool _enableLowAmmoWarning = true;
    [SerializeField] private int _lowAmmoThreshold = 5;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _lowAmmoColor = Color.red;
    [SerializeField] private Image _lowAmmoWarningIcon;

    private void Start()
    {
        _playerAttacker.AmmoChanged += OnAmmoChanged;
        _playerAttacker.WeaponSwitched += OnWeaponSwitched;

        UpdateAmmoDisplay(_playerAttacker.CurrentAmmo, _playerAttacker.MaxAmmo);
        UpdateWeaponVisibility();
    }

    private void OnAmmoChanged(int currentAmmo)
    {
        UpdateAmmoDisplay(currentAmmo, _playerAttacker.MaxAmmo);
        UpdateLowAmmoWarning(currentAmmo);
    }

    private void OnWeaponSwitched(WeaponType weaponType)
    {
        UpdateWeaponVisibility();
    }

    private void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        if (_ammoText != null && _showText)
        {
            _ammoText.text = string.Format(_format, currentAmmo, maxAmmo);

            if (_enableLowAmmoWarning)
                _ammoText.color = currentAmmo <= _lowAmmoThreshold ? _lowAmmoColor : _normalColor;
        }

        if (_ammoText != null)
            _ammoText.enabled = _playerAttacker.CurrentWeapon == WeaponType.Firearm;
    }

    private void UpdateLowAmmoWarning(int currentAmmo)
    {
        if (_lowAmmoWarningIcon != null && _enableLowAmmoWarning)
        {
            bool showWarning = currentAmmo <= _lowAmmoThreshold && _playerAttacker.CurrentWeapon == WeaponType.Firearm;
            _lowAmmoWarningIcon.gameObject.SetActive(showWarning);
        }
    }

    private void UpdateWeaponVisibility()
    {
        bool isFirearm = _playerAttacker.CurrentWeapon == WeaponType.Firearm;

        _ammoText.enabled = isFirearm && _showText;

        _lowAmmoWarningIcon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_playerAttacker != null)
        {
            _playerAttacker.AmmoChanged -= OnAmmoChanged;
            _playerAttacker.WeaponSwitched -= OnWeaponSwitched;
        }
    }
}