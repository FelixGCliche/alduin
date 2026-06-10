using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterMovementSO _settings;

    private CharacterController _controller;
    private IMovementInput      _input;       

    private Vector3 _velocity;
    private Vector3 _horizontalVelocity;
    private float   _currentSpeed;
    private float   _coyoteTimer;
    private float   _currentMomentum;

    private void Awake()
    {
        _controller   = GetComponent<CharacterController>();
        _input        = GetComponent<IMovementInput>();  
        _currentSpeed = _settings.walkSpeed;
    }

    private void Update()
    {
        HandleGravity();
        HandleJump();
        HandleMovement();
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded)
        {
            _coyoteTimer = _settings.coyoteTime;

            if (_velocity.y < 0f)
                _velocity.y = -2f;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;

            if (_velocity.y < 0f)
                _velocity.y += _settings.gravityValue * _settings.fallMultiplier * Time.deltaTime;
            else
                _velocity.y += _settings.gravityValue * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if (_input.ConsumeJump() && _coyoteTimer > 0f)
        {
            _velocity.y      = Mathf.Sqrt(_settings.jumpHeight * -2f * _settings.gravityValue);
            _coyoteTimer     = 0f;
            _currentMomentum = _settings.jumpMomentum;

            _horizontalVelocity = transform.forward * (_input.MoveInput.y * _currentSpeed * _settings.jumpMomentum)
                                + transform.right   * (_input.MoveInput.x * _currentSpeed * _settings.jumpMomentum);
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = (_input.IsSprinting && _input.MoveInput.magnitude > 0.1f)
            ? _settings.sprintSpeed
            : _settings.walkSpeed;

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed,
            _settings.sprintAcceleration * Time.deltaTime);

        Vector3 inputDir = transform.forward * _input.MoveInput.y
                         + transform.right   * _input.MoveInput.x;

        if (_controller.isGrounded)
        {
            if (_input.MoveInput.magnitude > 0.1f)
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * _currentSpeed,
                    _settings.sprintAcceleration * Time.deltaTime);
            }
            else
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    Vector3.zero,
                    _settings.deceleration * Time.deltaTime);
            }
        }
        else
        {
            _currentMomentum = Mathf.MoveTowards(
                _currentMomentum, 0f,
                (_settings.jumpMomentum / _settings.momentumDecayDuration) * Time.deltaTime);

            if (_input.MoveInput.magnitude > 0.1f && _horizontalVelocity.magnitude > 0.01f)
            {
                float currentMagnitude = _horizontalVelocity.magnitude;

                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * (currentMagnitude * _currentMomentum),
                    _settings.airControl);
            }
            else
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    Vector3.zero,
                    0.02f);
            }
        }

        _controller.Move((_horizontalVelocity + _velocity) * Time.deltaTime);
    }
}