using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAnimationManager))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerInteraction))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAnimationManager anim;

    public PlayerStateMachine StateMachine { get; private set; }

    // ===== STATES =====
    public IdleState IdleState { get; private set; }
    public WalkState WalkState { get; private set; }
    public JumpState JumpState { get; private set; }
    public FallState FallState { get; private set; }
    public PickupState PickupState { get; private set; }
    public HurtState HurtState { get; private set; }
    public RestState RestState { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.25f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimationManager>();

        StateMachine = new PlayerStateMachine();
        IdleState = new IdleState(this, StateMachine, anim);
        WalkState = new WalkState(this, StateMachine, anim);
        JumpState = new JumpState(this, StateMachine, anim);
        FallState = new FallState(this, StateMachine, anim);
        PickupState = new PickupState(this, StateMachine, anim);
        HurtState = new HurtState(this, StateMachine, anim);
        RestState = new RestState(this, StateMachine, anim);
    }

    private void OnEnable()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnJump -= HandleJump;
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.Update();
        HandleMovement();
    }

    // ===== MOVEMENT =====

    private void HandleMovement()
    {
        // 1. Block normal input if we are doing a locked action
        if (StateMachine.CurrentState == PickupState ||
            StateMachine.CurrentState == HurtState ||
            StateMachine.CurrentState == RestState)
        {
            if (StateMachine.CurrentState == PickupState)
            {
                // Instantly freeze in place when picking up flowers
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else if (StateMachine.CurrentState == HurtState || StateMachine.CurrentState == RestState)
            {
                // Smoothly slide to a stop! (Artificial Friction)
                // The '5f' controls the brake strength. Higher number = stops faster.
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0f, Time.deltaTime * 5f), rb.linearVelocity.y);
            }
            return;
        }

        // 2. Normal movement
        float move = GetMoveInput();
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
        }
    }

    private void HandleJump()
    {
        // 2. Block jumping during locked actions
        if (StateMachine.CurrentState == PickupState ||
            StateMachine.CurrentState == HurtState ||
            StateMachine.CurrentState == RestState)
            return;

        if (IsGrounded())
        {
            StateMachine.ChangeState(JumpState);
        }
    }

    public float GetMoveInput()
    {
        return InputHandler.Instance.MoveDirection.x;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public Rigidbody2D GetRB() => rb;

    // ===== PICKUP =====
    public void TriggerPickup(FlowerInteractable flower)
    {
        if (flower == null) return;

        PickupState.SetTarget(flower);
        StateMachine.ChangeState(PickupState);
    }

    public void AE_CollectFlower()
    {
        if (StateMachine.CurrentState == PickupState)
        {
            PickupState.PerformPickup();
        }
    }

    // The Animation will trigger this at the very last frame to unlock the player
    public void AE_FinishPickup()
    {
        if (StateMachine.CurrentState == PickupState)
        {
            StateMachine.ChangeState(IdleState);
        }
    }
}