using UnityEngine;
using SingletonManagers;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerAnimationManager anim;

    public PlayerStateMachine StateMachine { get; private set; }

    // ===== STATES =====
    public IdleState IdleState { get; private set; }
    public WalkState WalkState { get; private set; }
    public JumpState JumpState { get; private set; }
    public HurtState HurtState { get; private set; }
    public RestState RestState { get; private set; }
    public PickupState PickupState { get; private set; }

    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;
    public int CurrentHealth => currentHealth;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;

    [Header("Music")]
    public MusicTimer musicTimer;

    // ===== INTERACTION =====
    private IInteractable currentInteractable;
    private FlowerInteractable pickupTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimationManager>();

        StateMachine = new PlayerStateMachine();

        IdleState = new IdleState(this, StateMachine, anim);
        WalkState = new WalkState(this, StateMachine, anim);
        JumpState = new JumpState(this, StateMachine, anim);
        HurtState = new HurtState(this, StateMachine, anim);
        RestState = new RestState(this, StateMachine, anim);
        PickupState = new PickupState(this, StateMachine, anim);


    }

    private void Start()
    {
        currentHealth = maxHealth;
        StateMachine.Initialize(IdleState);
    }

    private void OnEnable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnJump += Jump;
            InputHandler.Instance.OnInteract += HandleInteract;
        }
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnJump -= Jump;
            InputHandler.Instance.OnInteract -= HandleInteract;
        }
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
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    public void TakeDamage(int amount)
    {
        // Ignore damage if already resting
        if (StateMachine.CurrentState == RestState) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StateMachine.ChangeState(RestState);
        }
        else
        {
            StateMachine.ChangeState(HurtState);
        }
    }

    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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

    // ===== INTERACTION SYSTEM =====

    private void HandleInteract()
    {
        currentInteractable?.Interact(this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var interactable = col.GetComponent<IInteractable>();
        if (interactable != null)
            currentInteractable = interactable;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        var interactable = col.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
            currentInteractable = null;
    }

    // ===== PICKUP FLOW =====

    public void TriggerPickup(FlowerInteractable flower)
    {
        pickupTarget = flower;
        StateMachine.ChangeState(PickupState);
    }

    public void PerformPickup()
    {
        if (pickupTarget == null) return;

        // Add to session
        var flowerData = pickupTarget.data;

        FindObjectOfType<DayTimeManager>().AddFlowerToSession(flowerData);

        // Effects
        ParticleManager.Instance.PlayParticle(
            "PickUp",
            pickupTarget.transform.position,
            flowerData.glowColor
        );

        AudioManager.Instance.Play("Pickup", pickupTarget.transform.position);

        Destroy(pickupTarget.gameObject, 0.2f);

        pickupTarget = null;
    }
}