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

    private Coroutine _resetBoolCoroutine;

    public void SetMovementSpeed(float speed)
    {
        _animator.SetFloat(_moveSpeedParam, speed);
    }

    public void PlayShootAnimation()
    {
        SetTriggerAnimation(_isShootingParam);
    }

    public void PlayMeleeAttackAnimation()
    {
        SetTriggerAnimation(_isMeleeAttackingParam);
    }

    public void PlayDeathAnimation()
    {
        _animator.SetBool(_isDeadParam, true);
    }

    public void SetWeaponType(WeaponType weaponType)
    {
        _animator.SetInteger(_weaponTypeParam, (int)weaponType);
    }

    public void ResetAllAnimations()
    {
        _animator.SetBool(_isDeadParam, false);
        _animator.SetBool(_isShootingParam, false);
        _animator.SetBool(_isMeleeAttackingParam, false);
        _animator.SetFloat(_moveSpeedParam, 0f);
    }

    private void SetTriggerAnimation(string paramName)
    {
        if (_resetBoolCoroutine != null)
        {
            StopCoroutine(_resetBoolCoroutine);
        }

        _animator.SetBool(paramName, true);
        _resetBoolCoroutine = StartCoroutine(ResetBoolCoroutine(paramName));
    }

    private IEnumerator ResetBoolCoroutine(string paramName)
    {
        yield return null;
        _animator.SetBool(paramName, false);
        _resetBoolCoroutine = null;
    }

    private void OnEnable()
    {
        ResetAllAnimations();
    }

    private void OnDisable()
    {
        if (_resetBoolCoroutine != null)
        {
            StopCoroutine(_resetBoolCoroutine);
            _resetBoolCoroutine = null;
        }
    }
}