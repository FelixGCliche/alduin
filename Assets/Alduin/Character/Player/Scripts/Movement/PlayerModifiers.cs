using UnityEngine;

public class PlayerModifiers : MonoBehaviour
{
    [Tooltip("How long the speed boost lingers after leaving the pad.")]
    [SerializeField, Range(0f, 2f)] private float padCoyoteTime = 0.5f;

    public float SpeedMultiplier    { get; private set; } = 1f;
    public float MomentumMultiplier { get; private set; } = 1f;

    private float _padCoyoteTimer;
    private bool  _isOnPad;

    private void Update()
    {
        if (_isOnPad) return;
        if (_padCoyoteTimer <= 0f) return;

        _padCoyoteTimer -= Time.deltaTime;

        if (_padCoyoteTimer <= 0f)
        {
            _padCoyoteTimer = 0f;
            ClearModifier();
        }
    }

    public void ApplyModifier(float speedMultiplier, float momentumMultiplier)
    {
        SpeedMultiplier    = speedMultiplier;
        MomentumMultiplier = momentumMultiplier;
        _isOnPad           = true;
        _padCoyoteTimer    = padCoyoteTime;
    }

    public void StartCoyote()
    {
        _isOnPad = false;
    }

    private void ClearModifier()
    {
        SpeedMultiplier    = 1f;
        MomentumMultiplier = 1f;
    }
}