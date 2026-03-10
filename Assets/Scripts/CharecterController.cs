using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

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


    [SerializeField] PlayerInput playerInput;

    private void OnEnable()
    {
        // Subscribe to InputHandler events if needed (not strictly necessary with the current design)
         InputHandler.Instance.OnJump += Jump;
         InputHandler.Instance.OnInteract += TryCollectFlower;
         InputHandler.Instance.OnLane1 += Lane1;
         InputHandler.Instance.OnLane2 += Lane2;
         InputHandler.Instance.OnLane3 += Lane3;
         InputHandler.Instance.OnFret1 += Fret1;
         InputHandler.Instance.OnFret2 += Fret2;
         InputHandler.Instance.OnFret3 += Fret3;
    }
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
         InputHandler.Instance.OnJump -= Jump;
         InputHandler.Instance.OnInteract -= TryCollectFlower;
         InputHandler.Instance.OnLane1 -= Lane1;
         InputHandler.Instance.OnLane2 -= Lane2;
         InputHandler.Instance.OnLane3 -= Lane3;
         InputHandler.Instance.OnFret1 -= Fret1;
         InputHandler.Instance.OnFret2 -= Fret2;
         InputHandler.Instance.OnFret3 -= Fret3;
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

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            InputHandler.Instance.SwitchActionMap();
             Debug.Log("Switched Action Map via J!");
        }


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

    private void Lane1()
    {
         Debug.Log("Lane 1 Triggered!");
    }
    private void Lane2()
    {
         Debug.Log("Lane 2 Triggered!");
    }
    private void Lane3()
    {
         Debug.Log("Lane 3 Triggered!");
    }

    private void Fret1(bool isPressed)
    {
         if(isPressed) Debug.Log("Fret 1 Pressed!");
         else Debug.Log("Fret 1 Released!");
    }
    private void Fret2(bool isPressed)
    {
         Debug.Log($"Fret 2 {(isPressed ? "Pressed" : "Released")}!");
    }
    private void Fret3(bool isPressed)
    {
         Debug.Log($"Fret 3 {(isPressed ? "Pressed" : "Released")}!");
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