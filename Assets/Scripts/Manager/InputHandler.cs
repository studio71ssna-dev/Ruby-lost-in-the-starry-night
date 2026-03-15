using Singleton;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.PlayerInput;

public class InputHandler : SingletonPersistent
{
    public static InputHandler Instance => GetInstance<InputHandler>();

    private PlayerInput playerInput;
    private InputAction MoveInput;

    public Vector2 MoveDirection { get; private set; }

    public event Action OnLane1;
    public event Action OnLane2;
    public event Action OnLane3;
    public event Action<bool> OnFret1;
    public event Action<bool> OnFret2;
    public event Action<bool> OnFret3;


    public event Action OnInteract;
    public event Action OnJump;
    public event Action OnPause;


    private void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) { print($"PlayerInput component is missing on {gameObject.name}"); }
    }

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

    public void PauseAction(InputAction.CallbackContext context)
    {
        if (context.performed) OnPause?.Invoke();
    }

    public void Lane1Trigger(InputAction.CallbackContext context)
    {
        if (context.performed) OnLane1?.Invoke();
    }
    public void Lane2Trigger(InputAction.CallbackContext context)
    {
        if (context.performed) OnLane2?.Invoke();
    }
    public void Lane3Trigger(InputAction.CallbackContext context)
    {
        if (context.performed) OnLane3?.Invoke();
    }

    public void Fret1Trigger(InputAction.CallbackContext context)
    {
        OnFret1?.Invoke(context.performed);
    }
    public void Fret2Trigger(InputAction.CallbackContext context)
    {
        OnFret2?.Invoke(context.performed);
    }
    public void Fret3Trigger(InputAction.CallbackContext context)
    {
        OnFret3?.Invoke(context.performed);
    }


    public void SwitchActionMap(string actionMapName)
    {
        playerInput.SwitchCurrentActionMap(actionMapName);
    }
}