using UnityEngine;

public class ArmsSprint : MonoBehaviour
{
    [Header("Sprint pose")]
    [SerializeField, Range(0f, 0.3f)] private float sprintDropY       = 0.08f;
    [SerializeField, Range(0f, 10f)]  private float sprintTiltZ       = 5f;
    [SerializeField, Range(0f, 20f)]  private float transitionSpeed   = 8f;

    private PlayerInputs  _inputs;
    private Vector3       _initialPosition;
    private Quaternion    _initialRotation;
    private bool          _isSprinting;

    private void Awake()
    {
        _inputs          = GetComponentInParent<PlayerInputs>();
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    private void OnEnable()  => _inputs.SprintEvent += OnSprint;
    private void OnDisable() => _inputs.SprintEvent -= OnSprint;

    private void OnSprint(bool isSprint) => _isSprinting = isSprint;

    private void Update()
    {
        Vector3    targetPos = _isSprinting
            ? _initialPosition + Vector3.down * sprintDropY
            : _initialPosition;

        Quaternion targetRot = _isSprinting
            ? _initialRotation * Quaternion.Euler(0f, 0f, -sprintTiltZ)
            : _initialRotation;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition, targetPos,
            Time.deltaTime * transitionSpeed);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, targetRot,
            Time.deltaTime * transitionSpeed);
    }
}