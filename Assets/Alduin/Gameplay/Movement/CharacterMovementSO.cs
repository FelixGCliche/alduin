using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMovementSO", menuName = "Alduin/Character/Movement Settings")]
public class CharacterMovementSO : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("Base movement speed when walking.")]
    [Range(1f, 20f)]  public float walkSpeed          = 5f;

    [Tooltip("Movement speed when sprinting.")]
    [Range(1f, 30f)]  public float sprintSpeed        = 9f;

    [Tooltip("How quickly the player accelerates to target speed.")]
    [Range(1f, 20f)]  public float sprintAcceleration = 8f;

    [Tooltip("How quickly the player decelerates to a stop when no input is given.")]
    [Range(1f, 20f)]  public float deceleration       = 10f;

    [Tooltip("Maximum height the player reaches at the peak of a jump.")]
    [Range(0.1f, 5f)] public float jumpHeight         = 1.2f;

    [Tooltip("How much the player can influence direction while airborne. 0 = no control, 1 = full control.")]
    [Range(0f, 1f)]   public float airControl         = 0.3f;

    [Header("Jump")]
    [Tooltip("Grace period after leaving the ground where the player can still jump.")]
    [Range(0f,   0.3f)] public float coyoteTime            = 0.15f;

    [Tooltip("Gravity applied to the player. Must be negative.")]
    [Range(-50f, -1f)]  public float gravityValue          = -25f;

    [Tooltip("Gravity multiplier applied when the player is falling. Higher values mean faster fall.")]
    [Range(1f,   5f)]   public float fallMultiplier        = 2.5f;

    [Tooltip("How much horizontal velocity is preserved when jumping. 0 = no momentum, 1 = full momentum, above 1 = extra boost.")]
    [Range(0f,   3f)]   public float jumpMomentum          = 1.2f;

    [Tooltip("Time in seconds for jump momentum to fully decay. Higher values = longer airtime control.")]
    [Range(0.1f, 5f)]   public float momentumDecayDuration = 1f;
}