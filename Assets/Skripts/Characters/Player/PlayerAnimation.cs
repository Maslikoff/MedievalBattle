using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation Parameters")]
    [SerializeField] private string _moveSpeedParam = "MoveSpeed";
    [SerializeField] private string _isShootingParam = "IsShooting";
    [SerializeField] private string _isMeleeAttackingParam = "IsMeleeAttacking";
    [SerializeField] private string _isDeadParam = "IsDead";
    [SerializeField] private string _weaponTypeParam = "WeaponType";

    [Header("Dependencies")]
    [SerializeField] private Animator _animator;
    [SerializeField] private InputHandler _inputHandler;

    private Vector3 _lastInput;
    private float _currentSpeed;

    private void Start()
    {
        _inputHandler.MoveInput += OnMoveInput;
    }

    public void PlayShootAnimation()
    {
        _animator.SetBool(_isShootingParam, true);
        StartCoroutine(ResetBoolNextFrame(_isShootingParam));
    }

    public void PlayMeleeAttackAnimation()
    {
        _animator.SetBool(_isMeleeAttackingParam, true);
        StartCoroutine(ResetBoolNextFrame(_isMeleeAttackingParam));
    }

    public void PlayDeathAnimation()
    {
        _animator.SetBool(_isDeadParam, true);
    }

    public void SetWeaponType(WeaponType weaponType)
    {
        int weaponTypeInt = (int)weaponType;
        _animator.SetInteger(_weaponTypeParam, weaponTypeInt);
    }

    private void OnMoveInput(Vector2 input)
    {
        _lastInput = input;
        _currentSpeed = _lastInput.magnitude;

        _animator.SetFloat(_moveSpeedParam, _currentSpeed);
    }

    private IEnumerator ResetBoolNextFrame(string paramName)
    {
        yield return null;

        _animator.SetBool(paramName, false);
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
            _inputHandler.MoveInput -= OnMoveInput;
    }
}