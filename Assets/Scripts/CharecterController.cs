using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class CharecterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    [Header("Interaction")]
    private List<GameObject> nearbyFlowers = new();

    private Rigidbody2D rb;
    private bool isGrounded;

    private void OnEnable()
    {
        // Subscribe to InputHandler events if needed (not strictly necessary with the current design)
         InputHandler.Instance.OnJump += Jump;
         InputHandler.Instance.OnInteract += TryCollectFlower;
    }
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
         InputHandler.Instance.OnJump -= Jump;
         InputHandler.Instance.OnInteract -= TryCollectFlower;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. Check Ground Status
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 2. Handle Movement
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        // We take the X from our InputHandler's Vector2
        float moveInput = InputHandler.Instance.MoveDirection.x;

        // Apply velocity but KEEP the current Y velocity (so gravity still works!)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip Sprite based on direction
        if (moveInput > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Jump()
    {
        if (!isGrounded) return;

        // Reset Y velocity before jumping for consistent height
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        Debug.Log("Ruby Jumped!");
    }

    private void TryCollectFlower()
    {
        if (nearbyFlowers.Count > 0 && nearbyFlowers[0] != null)
        {
            GameObject flower = nearbyFlowers[0];
            nearbyFlowers.RemoveAt(0);

            // Tell the DayTimeManager we got one!
            FindObjectOfType<DayTimeManager>().AddFlowerToSession();

            Destroy(flower);
        }
    }

    // Trigger Logic stays the same
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Flower") && !nearbyFlowers.Contains(col.gameObject))
            nearbyFlowers.Add(col.gameObject);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Flower")) nearbyFlowers.Remove(col.gameObject);
    }
}