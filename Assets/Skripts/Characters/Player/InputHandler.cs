using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> MoveInput;
    public event Action<Vector2> MouseLookInput;

    public event Action PrimaryAttack;
    public event Action WeaponSwitch;

    private void Update()
    {
        HandleMovementInput();
        HandleMpuseInput();
        HandleCombatInput();
    }

    private void HandleMovementInput()
    {
        Vector2 movmentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        MoveInput?.Invoke(movmentInput);
    }

    private void HandleMpuseInput()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MouseLookInput?.Invoke(mouseInput);
    }

    private void HandleCombatInput()
    {
        if(Input.GetMouseButton(0))
            PrimaryAttack?.Invoke();

        if(Input.GetKeyDown(KeyCode.Q))
            WeaponSwitch?.Invoke();
    }
}