using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour, IMovementInput, ICameraInput, IArmsInput
{
    public Vector2 MoveInput   { get; private set; }
    public Vector2 LookInput   { get; private set; }
    public bool    IsSprinting { get; private set; }

    private bool _jumpQueued;

    public bool ConsumeJump()
    {
        if (!_jumpQueued) return false;
        _jumpQueued = false;
        return true;
    }

    public event Action<Vector2> LookEvent;

    private PlayerInputActions _actions;

    private void Awake()
    {
        _actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _actions.Enable();
        _actions.Player.Move.performed   += OnMove;
        _actions.Player.Move.canceled    += OnMove;
        _actions.Player.Look.performed   += OnLook;
        _actions.Player.Look.canceled    += OnLook;
        _actions.Player.Jump.performed   += OnJump;
        _actions.Player.Sprint.performed += OnSprint;
        _actions.Player.Sprint.canceled  += OnSprint;
    }

    private void OnDisable()
    {
        _actions.Player.Move.performed   -= OnMove;
        _actions.Player.Move.canceled    -= OnMove;
        _actions.Player.Look.performed   -= OnLook;
        _actions.Player.Look.canceled    -= OnLook;
        _actions.Player.Jump.performed   -= OnJump;
        _actions.Player.Sprint.performed -= OnSprint;
        _actions.Player.Sprint.canceled  -= OnSprint;
        _actions.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
        => MoveInput = ctx.ReadValue<Vector2>();

    private void OnLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
        LookEvent?.Invoke(LookInput);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _jumpQueued = true;
    }

    private void OnSprint(InputAction.CallbackContext ctx)
        => IsSprinting = ctx.performed;
}