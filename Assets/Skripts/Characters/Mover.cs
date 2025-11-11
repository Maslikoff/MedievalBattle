using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Mover : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed = 5f;
    [SerializeField] protected float _mouseSensitivity = 2f;

    protected Rigidbody _rigidbody;
    protected Camera _viewCamera;
    protected float _xRotation = 0f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _viewCamera = Camera.main;
    }

    public abstract void Move(Vector2 input);
    public abstract void Look(Vector2 mouseInput);
    public abstract void StopMovement();
    public abstract void ResumeMovement();

    protected Vector3 GetWorldMoveDirection(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        return transform.TransformDirection(moveDirection);
    }

    protected void ApplyCameraRotation(Vector2 mouseInput)
    {
        if (_viewCamera == null) return;

        float mouseX = mouseInput.x * _mouseSensitivity;
        float mouseY = mouseInput.y * _mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _viewCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    protected void ApplyMovement(Vector3 worldMove)
    {
        if (_rigidbody == null) return;
        _rigidbody.MovePosition(_rigidbody.position + worldMove * _moveSpeed * Time.fixedDeltaTime);
    }
}