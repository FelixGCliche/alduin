using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Range(1f, 20f)]  private float walkSpeed          = 5f;
    [SerializeField, Range(1f, 30f)]  private float sprintSpeed        = 9f;
    [SerializeField, Range(1f, 20f)]  private float sprintAcceleration = 8f;
    [SerializeField, Range(0.1f, 5f)] private float jumpHeight         = 1.2f;
    [SerializeField, Range(0f, 1f)]   private float airControl         = 0.3f;

    [Header("Jump")]
    [SerializeField, Range(0f,   0.3f)] private float coyoteTime     = 0.15f;
    [SerializeField, Range(-50f, -1f)]  private float gravityValue   = -25f;
    [SerializeField, Range(1f,   5f)]   private float fallMultiplier = 2.5f;
    [SerializeField, Range(0f,   1f)]   private float jumpMomentum   = 0.5f;
    [SerializeField, Range(0f, 10f)] private float momentumDecaySpeed = 5f;

    private CharacterController _controller;
    private PlayerInputs        _inputs;

    private Vector2 _moveInput;
    private bool    _jumpQueued;
    private bool    _isSprinting;

    private Vector3 _velocity;
    private Vector3 _horizontalVelocity;
    private float   _currentSpeed;
    private float   _coyoteTimer;
    private float _currentMomentum;

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
            _velocity.y  = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            _coyoteTimer = 0f;
            _jumpQueued  = false;
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
        float targetSpeed = (_isSprinting && _moveInput != Vector2.zero)
            ? sprintSpeed
            : walkSpeed;

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed,
            sprintAcceleration * Time.deltaTime);

        Vector3 inputDir = transform.forward * _moveInput.y
                         + transform.right   * _moveInput.x;

        if (_controller.isGrounded)
        {
            _horizontalVelocity = inputDir.normalized * _currentSpeed;
        }
        else
        {
            _currentMomentum = Mathf.MoveTowards(
                _currentMomentum, 0f,
                momentumDecaySpeed * Time.deltaTime);
            
            if (_moveInput != Vector2.zero && _horizontalVelocity.magnitude > 0.01f)
            {
                float currentMagnitude = _horizontalVelocity.magnitude;

                _horizontalVelocity = Vector3.Lerp(
                    _horizontalVelocity,
                    inputDir.normalized * (currentMagnitude * jumpMomentum),
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