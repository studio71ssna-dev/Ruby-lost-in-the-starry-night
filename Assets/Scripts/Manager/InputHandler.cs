using Singleton;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : SingletonPersistent
{
    public static InputHandler Instance => GetInstance<InputHandler>();

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) JumpTriggered = true;
        else if (context.canceled) JumpTriggered = false;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Only trigger on the "Down" press
        if (context.started)
        {
            InteractTriggered = true;
            Debug.Log("InputHandler: Interact Pressed");
        }
    }

    private void LateUpdate()
    {
        // Very important: Clear the trigger so it doesn't stay true
        InteractTriggered = false;
        JumpTriggered = false;
    }
}