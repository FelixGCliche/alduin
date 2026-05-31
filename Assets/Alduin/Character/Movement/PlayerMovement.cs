using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Base movement speed when walking.")]
    [SerializeField, Range(1f, 20f)]  private float walkSpeed          = 5f;

    [Tooltip("Movement speed when sprinting.")]
    [SerializeField, Range(1f, 30f)]  private float sprintSpeed        = 9f;

    [Tooltip("How quickly the player accelerates to target speed.")]
    [SerializeField, Range(1f, 20f)]  private float sprintAcceleration = 8f;

    [Tooltip("How quickly the player decelerates to a stop when no input is given.")]
    [SerializeField, Range(1f, 20f)]  private float deceleration       = 10f;

    [Tooltip("Maximum height the player reaches at the peak of a jump.")]
    [SerializeField, Range(0.1f, 5f)] private float jumpHeight         = 1.2f;

    [Tooltip("How much the player can influence direction while airborne. 0 = no control, 1 = full control.")]
    [SerializeField, Range(0f, 1f)]   private float airControl         = 0.3f;

    [Header("Jump")]
    [Tooltip("Grace period after leaving the ground where the player can still jump.")]
    [SerializeField, Range(0f,   0.3f)] private float coyoteTime        = 0.15f;

    [Tooltip("Gravity applied to the player. Must be negative.")]
    [SerializeField, Range(-50f, -1f)]  private float gravityValue      = -25f;

    [Tooltip("Gravity multiplier applied when the player is falling. Higher values mean faster fall.")]
    [SerializeField, Range(1f,   5f)]   private float fallMultiplier    = 2.5f;

    [Tooltip("How much horizontal velocity is preserved when jumping. 0 = no momentum, 1 = full momentum.")]
    [SerializeField, Range(0f,   1f)]   private float jumpMomentum      = 0.5f;

    [Tooltip("Time in seconds for jump momentum to fully decay. Higher values = longer airtime control.")]
    [SerializeField, Range(0.1f, 5f)] private float momentumDecayDuration = 1f;

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
        _currentSpeed = walkSpeed;
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
            _coyoteTimer = coyoteTime;

            if (_velocity.y < 0f)
                _velocity.y = -2f;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;

            if (_velocity.y < 0f)
                _velocity.y += gravityValue * fallMultiplier * Time.deltaTime;
            else
                _velocity.y += gravityValue * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if (_jumpQueued && _coyoteTimer > 0f)
        {
            _velocity.y      = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            _coyoteTimer     = 0f;
            _jumpQueued      = false;
            _currentMomentum = jumpMomentum;

            _horizontalVelocity = transform.forward * (_moveInput.y * _currentSpeed * jumpMomentum)
                                + transform.right   * (_moveInput.x * _currentSpeed * jumpMomentum);
        }
        else if (_jumpQueued)
        {
            _jumpQueued = false;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = (_isSprinting && _moveInput.magnitude > 0.1f)
            ? sprintSpeed
            : walkSpeed;

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed,
            sprintAcceleration * Time.deltaTime);

        Vector3 inputDir = transform.forward * _moveInput.y
                         + transform.right   * _moveInput.x;

        if (_controller.isGrounded)
        {
            if (_moveInput.magnitude > 0.1f)
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * _currentSpeed,
                    sprintAcceleration * Time.deltaTime);
            }
            else
            {
                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    Vector3.zero,
                    deceleration * Time.deltaTime);
            }
        }
        else
        {
            _currentMomentum = Mathf.MoveTowards(
                _currentMomentum, 0f,
                (jumpMomentum / momentumDecayDuration) * Time.deltaTime);

            if (_moveInput.magnitude > 0.1f && _horizontalVelocity.magnitude > 0.01f)
            {
                float currentMagnitude = _horizontalVelocity.magnitude;

                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * (currentMagnitude * _currentMomentum),
                    airControl);
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