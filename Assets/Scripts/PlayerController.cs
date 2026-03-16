using SingletonManagers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public MusicTimer musicTimer;

    [Header("Interaction")]
    private IInteractable currentInteractable;

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerAnimationManager animManager;
    private PlayerAnimationManager.PlayerAnimState currentState;

    [SerializeField] private float walkDustInterval = 0.25f;
    private float walkDustTimer = 0f;

    private void OnEnable()
    {
        // Subscribing to events from your InputHandler
        InputHandler.Instance.OnJump += Jump;
        InputHandler.Instance.OnInteract += TryInteract;
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnJump -= Jump;
            InputHandler.Instance.OnInteract -= TryInteract;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animManager = GetComponent<PlayerAnimationManager>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        ApplyMovement();
        DetermineAnimationState();
    }


    private void ApplyMovement()
    {
        // Stop movement if we are currently in the middle of a Pickup animation
        if (currentState == PlayerAnimationManager.PlayerAnimState.Pickup)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Keep falling if in air, but stop horizontal
            return;
        }

        // Accessing MoveDirection from your InputHandler
        Vector2 moveDir = InputHandler.Instance.MoveDirection;
        rb.linearVelocity = new Vector2(moveDir.x * moveSpeed, rb.linearVelocity.y);

        // Flip Sprite based on direction
        if (moveDir.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveDir.x), 1, 1);
        }

        if (Mathf.Abs(moveDir.x) > 0.1f && isGrounded)
        {
            ExecuteAfterInterval(ref walkDustTimer, walkDustInterval, () =>
            {
                ParticleManager.Instance.PlayParticle("Walk", groundCheck.position);
            });
        }
    }

    private void DetermineAnimationState()
    {
        // GUARD CLAUSE: If we are picking something up, lock the state until it finishes.
        if (currentState == PlayerAnimationManager.PlayerAnimState.Pickup)
            return;

        // Priority-based animation switching
        if (!isGrounded)
        {
            SetState(PlayerAnimationManager.PlayerAnimState.Jump);
        }
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            SetState(PlayerAnimationManager.PlayerAnimState.Walk);
        }
        else
        {
            // We removed the Pickup check here because the guard clause above handles it
            if (currentState != PlayerAnimationManager.PlayerAnimState.Rest)
            {
                SetState(PlayerAnimationManager.PlayerAnimState.Idle);
            }
        }
    }

    private void SetState(PlayerAnimationManager.PlayerAnimState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        animManager.UpdateAnimation(newState);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void TryInteract()
    {
        currentInteractable?.Interact(this);
    }

    private void ResetToIdle() => SetState(PlayerAnimationManager.PlayerAnimState.Idle);

    private void OnTriggerEnter2D(Collider2D col)
    {
        IInteractable interactable = col.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        IInteractable interactable = col.GetComponent<IInteractable>();

        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    private void ExecuteAfterInterval(ref float timer, float interval, System.Action action)
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            action?.Invoke();
        }
    }
}