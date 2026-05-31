using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private float lookSensitivityX = 1f;
    [SerializeField] private float lookSensitivityY = 0.8f;

    [Header("Clamp vertical")]
    [SerializeField] private float topClamp    = 80f;
    [SerializeField] private float bottomClamp = -80f;

    private PlayerInputs _inputs;
    private Transform    _playerBody;

    private Vector2 _lookInput;
    private float   _targetPitch;

    private void Awake()
    {
        _inputs     = GetComponentInParent<PlayerInputs>();
        _playerBody = transform.parent;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void OnEnable()  => _inputs.LookEvent += OnLook;
    private void OnDisable() => _inputs.LookEvent -= OnLook;

    private void OnLook(Vector2 input) => _lookInput = input;

    private void LateUpdate()
    {
        if (_lookInput.sqrMagnitude < 0.01f) return;

        // Yaw 
        _playerBody.Rotate(Vector3.up, _lookInput.x * lookSensitivityX);

        // Pitch 
        _targetPitch -= _lookInput.y * lookSensitivityY;
        _targetPitch  = Mathf.Clamp(_targetPitch, bottomClamp, topClamp);

        // Apply pitch (CameraTarget)
        transform.localRotation = Quaternion.Euler(_targetPitch, 0f, 0f);
    }
}