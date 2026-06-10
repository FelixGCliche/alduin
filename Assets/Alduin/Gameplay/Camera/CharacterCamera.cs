using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    [SerializeField] private CharacterCameraSO _settings;

    private ICameraInput _input;
    private Transform    _body;
    private float        _targetPitch;

    private void Awake()
    {
        _input      = GetComponentInParent<ICameraInput>();
        _body = transform.parent;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void LateUpdate()
    {
        if (_input.LookInput.sqrMagnitude < 0.01f) return;

        _body.Rotate(Vector3.up, _input.LookInput.x * _settings.lookSensitivityX);

        _targetPitch -= _input.LookInput.y * _settings.lookSensitivityY;
        _targetPitch  = Mathf.Clamp(_targetPitch, -_settings.topClamp, _settings.bottomClamp);

        transform.localRotation = Quaternion.Euler(_targetPitch, 0f, 0f);
    }
}