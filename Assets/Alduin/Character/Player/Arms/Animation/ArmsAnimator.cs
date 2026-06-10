using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArmsAnimator : MonoBehaviour
{
    private static readonly int SpeedHash    = Animator.StringToHash("Speed");
    private static readonly int IsSprintHash = Animator.StringToHash("IsSprinting");
    private static readonly int FireHash     = Animator.StringToHash("Fire");
    private static readonly int ReloadHash   = Animator.StringToHash("Reload");

    private Animator   _animator;
    private IArmsInput _input;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _input    = GetComponentInParent<IArmsInput>();
    }

    private void Update()
    {
        float speed = _input.MoveInput.magnitude;
        _animator.SetFloat(SpeedHash,   speed);
        _animator.SetBool(IsSprintHash, _input.IsSprinting && speed > 0.1f);
    }

    public void PlayFire()   => _animator.SetTrigger(FireHash);
    public void PlayReload() => _animator.SetTrigger(ReloadHash);
}