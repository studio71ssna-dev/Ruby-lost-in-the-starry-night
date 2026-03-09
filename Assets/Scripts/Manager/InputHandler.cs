using Singleton;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.PlayerInput;

public class InputHandler : SingletonPersistent
{
    public static InputHandler Instance => GetInstance<InputHandler>();

    private InputAction MoveInput;
    public Vector2 MoveDirection { get; private set; }

    public event Action OnInteract;
    public event Action OnJump;


    private void Start()
    {
        MoveInput = gameObject.GetComponent<PlayerInput>().actions.FindAction("Move");
        if (MoveInput == null) { print($"Input System is missing on {gameObject.name}"); }
    }
    private void Update()
    {
        MoveAction();
    }
    public void MoveAction()
    {
        MoveDirection = MoveInput.ReadValue<Vector2>();
    }

    public void JumpAction(InputAction.CallbackContext context)
    {
        if (context.performed) OnJump?.Invoke();
    }

    public void InteractAction(InputAction.CallbackContext context)
    {
        if (context.performed) OnInteract?.Invoke();
    }
}