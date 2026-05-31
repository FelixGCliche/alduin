using UnityEngine;

public class ArmsProceduralMotion : MonoBehaviour
{
    [Header("Bob")]
    [SerializeField, Range(0f, 0.1f)]  private float walkBobAmount   = 0.005f;
    [SerializeField, Range(0f, 0.1f)]  private float sprintBobAmount  = 0.012f;
    [SerializeField, Range(0f, 30f)]   private float walkBobSpeed     = 14f;
    [SerializeField, Range(0f, 30f)]   private float sprintBobSpeed   = 20f;

    [Header("Sprint pose")]
    [SerializeField, Range(0f, 0.3f)]  private float sprintDropY      = 0.08f;
    [SerializeField, Range(0f, 10f)]   private float sprintTiltZ      = 5f;
    [SerializeField, Range(0f, 20f)]   private float transitionSpeed  = 8f;

    private PlayerInputs _inputs;
    private Vector3      _initialPosition;
    private Quaternion   _initialRotation;

    private Vector2 _moveInput;
    private bool    _isSprinting;
    private float   _bobTimer;

    private void Awake()
    {
        _inputs          = GetComponentInParent<PlayerInputs>();
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
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
        Vector3    bobOffset    = ComputeBobOffset();
        Vector3    sprintOffset = ComputeSprintPositionOffset();
        Quaternion sprintRot    = ComputeSprintRotation();

        Vector3 targetPos = _initialPosition + bobOffset + sprintOffset;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, targetPos,
            transitionSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, _initialRotation * sprintRot,
            transitionSpeed * Time.deltaTime);
    }

    private Vector3 ComputeBobOffset()
    {
        bool isMoving = _moveInput != Vector2.zero;

        if (!isMoving)
        {
            _bobTimer = 0f;
            return Vector3.zero;
        }

        float bobSpeed  = _isSprinting ? sprintBobSpeed  : walkBobSpeed;
        float bobAmount = _isSprinting ? sprintBobAmount : walkBobAmount;

        _bobTimer += Time.deltaTime * bobSpeed;

        return new Vector3(
            Mathf.Cos(_bobTimer * 0.5f) * bobAmount,  
            Mathf.Sin(_bobTimer)        * bobAmount, 
            0f);
    }

    private Vector3 ComputeSprintPositionOffset()
    {
        float targetY = _isSprinting ? -sprintDropY : 0f;
        return new Vector3(0f, targetY, 0f);
    }

    private Quaternion ComputeSprintRotation()
    {
        float targetTilt = _isSprinting ? -sprintTiltZ : 0f;
        return Quaternion.Euler(0f, 0f, targetTilt);
    }
}