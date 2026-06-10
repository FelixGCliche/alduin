using UnityEngine;

public class ArmsProceduralMotion : MonoBehaviour
{
    [Header("Bob")]
    [SerializeField, Range(0f, 0.1f)] private float walkBobAmount  = 0.005f;
    [SerializeField, Range(0f, 0.1f)] private float sprintBobAmount = 0.012f;
    [SerializeField, Range(0f, 30f)]  private float walkBobSpeed    = 14f;
    [SerializeField, Range(0f, 30f)]  private float sprintBobSpeed  = 20f;

    [Header("Sprint pose")]
    [SerializeField, Range(0f, 0.3f)] private float sprintDropY     = 0.08f;
    [SerializeField, Range(0f, 10f)]  private float sprintTiltZ     = 5f;
    [SerializeField, Range(0f, 20f)]  private float transitionSpeed = 8f;

    private IArmsInput _input;
    private Vector3    _initialPosition;
    private Quaternion _initialRotation;
    private float      _bobTimer;

    private void Awake()
    {
        _input           = GetComponentInParent<IArmsInput>();
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    private void Update()
    {
        bool isMoving = _input.MoveInput.magnitude > 0.1f;

        Vector3    bobOffset    = ComputeBobOffset(isMoving);
        Vector3    sprintOffset = ComputeSprintPositionOffset(isMoving);
        Quaternion sprintRot    = ComputeSprintRotation(isMoving);

        Vector3 targetPos = _initialPosition + bobOffset + sprintOffset;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition, targetPos,
            transitionSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, _initialRotation * sprintRot,
            transitionSpeed * Time.deltaTime);
    }

    private Vector3 ComputeBobOffset(bool isMoving)
    {
        if (!isMoving)
        {
            _bobTimer = 0f;
            return Vector3.zero;
        }

        float bobSpeed  = _input.IsSprinting ? sprintBobSpeed  : walkBobSpeed;
        float bobAmount = _input.IsSprinting ? sprintBobAmount : walkBobAmount;

        _bobTimer += Time.deltaTime * bobSpeed;

        return new Vector3(
            Mathf.Cos(_bobTimer * 0.5f) * bobAmount,
            Mathf.Sin(_bobTimer)        * bobAmount,
            0f);
    }

    private Vector3 ComputeSprintPositionOffset(bool isMoving)
    {
        float targetY = (_input.IsSprinting && isMoving) ? -sprintDropY : 0f;
        return new Vector3(0f, targetY, 0f);
    }

    private Quaternion ComputeSprintRotation(bool isMoving)
    {
        float targetTilt = (_input.IsSprinting && isMoving) ? -sprintTiltZ : 0f;
        return Quaternion.Euler(0f, 0f, targetTilt);
    }
}