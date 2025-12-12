using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Mover : MonoBehaviour
{
    [SerializeField] protected float MoveSpeed = 5f;
    [SerializeField] protected float MouseSensitivity = 2f;

    protected Rigidbody Rigidbody;
    protected Camera ViewCamera;
    protected float XRotation = 0f;

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        ViewCamera = Camera.main;
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
        if (ViewCamera == null) return;

        float mouseX = mouseInput.x * MouseSensitivity;
        float mouseY = mouseInput.y * MouseSensitivity;

        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);

        ViewCamera.transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    protected void ApplyMovement(Vector3 worldMove)
    {
        if (Rigidbody == null) return;
        Rigidbody.MovePosition(Rigidbody.position + worldMove * MoveSpeed * Time.fixedDeltaTime);
    }
}