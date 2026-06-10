using UnityEngine;

public interface IArmsInput
{
    Vector2 MoveInput   { get; }
    bool    IsSprinting { get; }
}