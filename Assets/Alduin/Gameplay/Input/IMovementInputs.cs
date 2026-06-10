using UnityEngine;

public interface IMovementInput
{
    Vector2 MoveInput   { get; }
    Vector2 LookInput   { get; }
    bool    IsSprinting { get; }
    bool    ConsumeJump();
}