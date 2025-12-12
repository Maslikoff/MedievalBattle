using UnityEngine;

public class PlayerMover : Mover
{
    private Player _player;

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();

        Rigidbody.freezeRotation = true;
        Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Move(Vector2 input)
    {
        if (CanMove() == false)
            return;

        Vector3 worldMove = GetWorldMoveDirection(input);

        ApplyMovement(worldMove);
    }

    public override void Look(Vector2 mouseInput)
    {
        if (CanMove() == false)
            return;

        ApplyCameraRotation(mouseInput);
    }

    public override void StopMovement()
    {
        if (Rigidbody != null)
            Rigidbody.velocity = Vector3.zero;
    }

    public override void ResumeMovement() { }

    private bool CanMove() => _player != null && _player.IsAlive;
}