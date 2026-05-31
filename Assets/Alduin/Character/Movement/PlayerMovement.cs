using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed          = 5f;
    [SerializeField] private float sprintSpeed        = 9f;
    [SerializeField] private float sprintAcceleration = 8f;
    [SerializeField] private float jumpHeight         = 1.2f;
    [SerializeField] private float gravityValue       = -9.81f;

    [Header("Jump")]
    [SerializeField, Range(0f, 0.3f)] private float coyoteTime = 0.15f;
    
    private CharacterController _controller;
    private PlayerInputs        _inputs;

    private Vector2 _moveInput;
    private bool    _jumpQueued;
    private bool    _isSprinting;

    private Vector3 _velocity;
    private float   _currentSpeed;
    private float _coyoteTimer;

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
        }

        _velocity.y += gravityValue * Time.deltaTime;
    }

    private void HandleJump()
    {
        if (_jumpQueued && _coyoteTimer > 0f)
        {
            _velocity.y  = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            _coyoteTimer = 0f; 
            _jumpQueued  = false;
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

        Vector3 moveDir = transform.forward * _moveInput.y
                        + transform.right   * _moveInput.x;

        _controller.Move((moveDir.normalized * _currentSpeed + _velocity) * Time.deltaTime);
    }
}