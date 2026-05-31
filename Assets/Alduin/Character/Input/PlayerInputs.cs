using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputs : MonoBehaviour
{
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action          JumpEvent;
    public event Action<bool>    SprintEvent;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _playerInput.actions["Move"].performed  += OnMove;
        _playerInput.actions["Move"].canceled   += OnMove;
        _playerInput.actions["Look"].performed  += OnLook;
        _playerInput.actions["Look"].canceled   += OnLook;
        _playerInput.actions["Jump"].performed  += OnJump;
        _playerInput.actions["Sprint"].performed += OnSprint;
        _playerInput.actions["Sprint"].canceled  += OnSprint;
    }

    private void OnDestroy()
    {
        _playerInput.actions["Move"].performed  -= OnMove;
        _playerInput.actions["Move"].canceled   -= OnMove;
        _playerInput.actions["Look"].performed  -= OnLook;
        _playerInput.actions["Look"].canceled   -= OnLook;
        _playerInput.actions["Jump"].performed  -= OnJump;
        _playerInput.actions["Sprint"].performed -= OnSprint;
        _playerInput.actions["Sprint"].canceled  -= OnSprint;
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