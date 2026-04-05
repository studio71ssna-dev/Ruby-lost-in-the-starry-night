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
    public PickupState PickupState { get; private set; }
    public HurtState HurtState { get; private set; }
    public RestState RestState { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;

    // Cached systems
    //public DayTimeManager DayTimeManager { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimationManager>();

        StateMachine = new PlayerStateMachine();

        // Initialize States
        IdleState = new IdleState(this, StateMachine, anim);
        WalkState = new WalkState(this, StateMachine, anim);
        JumpState = new JumpState(this, StateMachine, anim);
        PickupState = new PickupState(this, StateMachine, anim);
        HurtState = new HurtState(this, StateMachine, anim);
        RestState = new RestState(this, StateMachine, anim);
    }

    private void Start()
    {
        // Cache manager (ONLY once)
       // DayTimeManager = FindFirstObjectByType<DayTimeManager>();

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
        float move = GetMoveInput();

        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
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
        if (!IsGrounded()) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // ===== PICKUP ENTRY POINT =====

    public void TriggerPickup(FlowerInteractable flower)
    {
        if (flower == null) return;

        PickupState.SetTarget(flower);
        StateMachine.ChangeState(PickupState);
    }
}