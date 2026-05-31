using UnityEngine;

public class ArmsBob : MonoBehaviour
{
    [Header("Bob settings")]
    [SerializeField, Range(0f, 0.1f)]  private float walkBobAmount    = 0.005f;
    [SerializeField, Range(0f, 0.1f)]  private float sprintBobAmount  = 0.012f;
    [SerializeField, Range(0f, 30f)]   private float walkBobSpeed     = 14f;
    [SerializeField, Range(0f, 30f)]   private float sprintBobSpeed   = 20f;

    private PlayerInputs _inputs;
    private Vector3      _initialPosition;
    private float        _bobTimer;
    private Vector2      _moveInput;
    private bool         _isSprinting;

    private void Awake()
    {
        _inputs          = GetComponentInParent<PlayerInputs>();
        _initialPosition = transform.localPosition;
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
        bool isMoving = _moveInput != Vector2.zero;

        if (!isMoving)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _initialPosition,
                Time.deltaTime * 10f);
            return;
        }

        float bobSpeed  = _isSprinting ? sprintBobSpeed  : walkBobSpeed;
        float bobAmount = _isSprinting ? sprintBobAmount : walkBobAmount;

        _bobTimer += Time.deltaTime * bobSpeed;

        Vector3 bobOffset = new Vector3(
            Mathf.Cos(_bobTimer * 0.5f) * bobAmount,
            Mathf.Sin(_bobTimer)        * bobAmount,
            0f);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            _initialPosition + bobOffset,
            Time.deltaTime * 15f);
    }
}