using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action          JumpEvent;
    public event Action<bool>    SprintEvent;

    private PlayerInputActions _actions;

    private void Awake()
    {
        _actions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _actions.Enable();

        _actions.Player.Move.performed  += OnMove;
        _actions.Player.Move.canceled   += OnMove;
        _actions.Player.Look.performed  += OnLook;
        _actions.Player.Look.canceled   += OnLook;
        _actions.Player.Jump.performed  += OnJump;
        _actions.Player.Sprint.performed += OnSprint;
        _actions.Player.Sprint.canceled  += OnSprint;
    }

    private void OnDisable()
    {
        _actions.Player.Move.performed  -= OnMove;
        _actions.Player.Move.canceled   -= OnMove;
        _actions.Player.Look.performed  -= OnLook;
        _actions.Player.Look.canceled   -= OnLook;
        _actions.Player.Jump.performed  -= OnJump;
        _actions.Player.Sprint.performed -= OnSprint;
        _actions.Player.Sprint.canceled  -= OnSprint;

        _actions.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
        => MoveEvent?.Invoke(ctx.ReadValue<Vector2>());

    private void OnLook(InputAction.CallbackContext ctx)
        => LookEvent?.Invoke(ctx.ReadValue<Vector2>());

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) JumpEvent?.Invoke();
    }

    private void OnSprint(InputAction.CallbackContext ctx)
        => SprintEvent?.Invoke(ctx.performed);
}