using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCameraSO", menuName = "Alduin/Character/Camera Settings")]
public class CharacterCameraSO : ScriptableObject
{
    [Header("Sensitivity")]
    [Tooltip("Horizontal look sensitivity.")]
    [Range(0.01f, 2f)] public float lookSensitivityX = 0.1f;

    [Tooltip("Vertical look sensitivity.")]
    [Range(0.01f, 2f)] public float lookSensitivityY = 0.08f;

    [Header("Clamp vertical")]
    [Tooltip("Maximum angle in degrees the player can look up.")]
    [Range(0f, 89f)] public float topClamp    = 80f;

    [Tooltip("Maximum angle in degrees the player can look down.")]
    [Range(0f, 89f)] public float bottomClamp = 80f;
}
