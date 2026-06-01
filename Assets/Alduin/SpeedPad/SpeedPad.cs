using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    [SerializeField, Range(1f, 5f)] private float speedMultiplier    = 2f;
    [SerializeField, Range(1f, 5f)] private float momentumMultiplier = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerModifiers modifiers))
            modifiers.ApplyModifier(speedMultiplier, momentumMultiplier);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerModifiers modifiers))
            modifiers.StartCoyote();
    }
}