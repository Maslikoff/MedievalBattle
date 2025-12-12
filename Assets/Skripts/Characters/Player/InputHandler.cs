using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private const string AxisHorizontal = "Horizontal";
    private const string AxisVertocal = "Vertical";
    private const string MouseAxisHorizontal = "Mouse X";
    private const string MouseAxisVertocal = "Mouse Y";
    private const int AttackNumber = 0;
    private const KeyCode ChoiceWeapone = KeyCode.Q;
    private const KeyCode Pause = KeyCode.Q;

    public event Action<Vector2> MoveInput;
    public event Action<Vector2> MouseLookInput;

    public event Action PrimaryAttack;
    public event Action WeaponSwitch;
    public event Action EscapePressed;

    private void Update()
    {
        HandleMovementInput();
        HandleMpuseInput();
        HandleCombatInput();
        HandleEscapeInput();
    }

    private void HandleMovementInput()
    {
        Vector2 movmentInput = new Vector2(Input.GetAxis(AxisHorizontal), Input.GetAxis(AxisVertocal));

        MoveInput?.Invoke(movmentInput);
    }

    private void HandleMpuseInput()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxis(MouseAxisHorizontal), Input.GetAxis(MouseAxisVertocal));

        MouseLookInput?.Invoke(mouseInput);
    }

    private void HandleCombatInput()
    {
        if(Input.GetMouseButton(AttackNumber))
            PrimaryAttack?.Invoke();

        if(Input.GetKeyDown(ChoiceWeapone))
            WeaponSwitch?.Invoke();
    }

    private void HandleEscapeInput()
    {
        if (Input.GetKeyDown(Pause))
            EscapePressed?.Invoke();
    }
}