using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
   [SerializeField] CharacterMovementSO movementSO;

    private CharacterController _controller;
    private PlayerInputs        _inputs;

    private Vector2 _moveInput;
    private bool    _jumpQueued;
    private bool    _isSprinting;

    private Vector3 _velocity;
    private Vector3 _horizontalVelocity;
    private float   _currentSpeed;
    private float   _coyoteTimer;
    private float   _currentMomentum;

    private void Awake()
    {
        _controller   = GetComponent<CharacterController>();
        _inputs       = GetComponent<PlayerInputs>();
        _currentSpeed = movementSO.walkSpeed;
    }

    private void OnEnable()
    {
        _inputs.MoveEvent   += OnMove;
        _inputs.JumpEvent   += OnJump;
        _inputs.SprintEvent += OnSprint;
    }

    private void OnDisable()
    {
        _inputs.MoveEvent   -= OnMove;
        _inputs.JumpEvent   -= OnJump;
        _inputs.SprintEvent -= OnSprint;
    }

    private void OnMove(Vector2 input)   => _moveInput   = input;
    private void OnJump()                => _jumpQueued  = true;
    private void OnSprint(bool isSprint) => _isSprinting = isSprint;

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
            _coyoteTimer = movementSO.coyoteTime;

            if (_velocity.y < 0f)
                _velocity.y = -2f;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;

            if (_velocity.y < 0f)
                _velocity.y += movementSO.gravityValue * movementSO.fallMultiplier * Time.deltaTime;
            else
                _velocity.y += movementSO.gravityValue * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if (_jumpQueued && _coyoteTimer > 0f)
        {
            _velocity.y      = Mathf.Sqrt(movementSO.jumpHeight * -2f * movementSO.gravityValue);
            _coyoteTimer     = 0f;
            _jumpQueued      = false;
            _currentMomentum = movementSO.jumpMomentum;

            _horizontalVelocity = transform.forward * (_moveInput.y * _currentSpeed * movementSO.jumpMomentum)
                                + transform.right   * (_moveInput.x * _currentSpeed * movementSO.jumpMomentum);
        }
        else if (_jumpQueued)
        {
            _jumpQueued = false;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = (_isSprinting && _moveInput.magnitude > 0.1f)
            ? movementSO.sprintSpeed
            : movementSO.walkSpeed;

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed,
            movementSO.sprintAcceleration * Time.deltaTime);

        Vector3 inputDir = transform.forward * _moveInput.y
                         + transform.right   * _moveInput.x;

        if (_controller.isGrounded)
        {
            if (_moveInput.magnitude > 0.1f)
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * _currentSpeed,
                    movementSO.sprintAcceleration * Time.deltaTime);
            }
            else
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    Vector3.zero,
                    movementSO.deceleration * Time.deltaTime);
            }
        }
        else
        {
            _currentMomentum = Mathf.MoveTowards(
                _currentMomentum, 0f,
                (movementSO.jumpMomentum / movementSO.momentumDecayDuration) * Time.deltaTime);

            if (_moveInput.magnitude > 0.1f && _horizontalVelocity.magnitude > 0.01f)
            {
                float currentMagnitude = _horizontalVelocity.magnitude;

                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * (currentMagnitude * _currentMomentum),
                    movementSO.airControl);
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