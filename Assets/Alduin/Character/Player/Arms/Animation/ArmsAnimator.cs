using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArmsAnimator : MonoBehaviour
{
    private static readonly int SpeedHash    = Animator.StringToHash("Speed");
    private static readonly int IsSprintHash = Animator.StringToHash("IsSprinting");
    private static readonly int FireHash     = Animator.StringToHash("Fire");
    private static readonly int ReloadHash   = Animator.StringToHash("Reload");

    private Animator     _animator;
    private PlayerInputs _inputs;
    private Vector2      _moveInput;
    private bool         _isSprinting;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputs   = GetComponentInParent<PlayerInputs>();
    }

    private void OnEnable()
    {
        _inputs.MoveEvent   += OnMove;
        _inputs.SprintEvent += OnSprint;
    }

    private void OnDisable()
    {
        _inputs.MoveEvent   -= OnMove;
        _inputs.SprintEvent -= OnSprint;
    }

    private void OnMove(Vector2 input)   => _moveInput   = input;
    private void OnSprint(bool isSprint) => _isSprinting = isSprint;

    private void Update()
    {
        float speed = _moveInput.magnitude;
        _animator.SetFloat(SpeedHash,    _moveInput.magnitude);
        _animator.SetBool(IsSprintHash, _isSprinting && speed > 0.1f);
    }

    public void PlayFire()   => _animator.SetTrigger(FireHash);
    public void PlayReload() => _animator.SetTrigger(ReloadHash);
}