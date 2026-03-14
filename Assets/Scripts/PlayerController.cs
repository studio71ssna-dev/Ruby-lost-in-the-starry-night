using UnityEngine;
using System.Collections.Generic;
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
    private List<GameObject> nearbyFlowers = new();

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerAnimationManager animManager;
    private PlayerAnimationManager.PlayerAnimState currentState;
    private bool wolfnearby;

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
        // Accessing MoveDirection from your InputHandler
        Vector2 moveDir = InputHandler.Instance.MoveDirection;
        rb.linearVelocity = new Vector2(moveDir.x * moveSpeed, rb.linearVelocity.y);

        // Flip Sprite based on direction
        if (moveDir.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveDir.x), 1, 1);
        }
    }

    private void DetermineAnimationState()
    {
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
            // Only go back to Idle if we aren't in a special state like Rest or Pickup
            if (currentState != PlayerAnimationManager.PlayerAnimState.Pickup &&
                currentState != PlayerAnimationManager.PlayerAnimState.Rest)
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
        if (nearbyFlowers.Count > 0 && nearbyFlowers[0] != null)
        {
            // Play Pickup Animation
            SetState(PlayerAnimationManager.PlayerAnimState.Pickup);

            GameObject flowerObj = nearbyFlowers[0];
            FlowerItem flowerScript = flowerObj.GetComponent<FlowerItem>();

            if (flowerScript != null && flowerScript.data != null)
            {
                // Communication with DayTimeManager and UIManager remains intact
                FindObjectOfType<DayTimeManager>().AddFlowerToSession(flowerScript.data);
                nearbyFlowers.RemoveAt(0);
                Destroy(flowerObj,0.25f);
                // Return to idle after a delay or via Animation Event
                Invoke(nameof(ResetToIdle), 0.5f);
            }
        }
        if (wolfnearby)
        {
            musicTimer.PlayRandomSong();
        }
    }

    private void ResetToIdle() => SetState(PlayerAnimationManager.PlayerAnimState.Idle);

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Flower") && !nearbyFlowers.Contains(col.gameObject)) nearbyFlowers.Add(col.gameObject);
        if (col.CompareTag("Wolf")) wolfnearby = true;      
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Flower")) nearbyFlowers.Remove(col.gameObject);
        if (col.CompareTag("Wolf")) wolfnearby = false;
    }
}