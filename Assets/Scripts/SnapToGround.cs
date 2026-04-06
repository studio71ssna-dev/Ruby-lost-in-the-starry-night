using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    [Tooltip("How far down to look for the ground")]
    public float raycastDistance = 2f;

    [Tooltip("Make sure your ground is on this layer!")]
    public LayerMask groundLayer;

    void Start()
    {
        // Cast a ray straight down from the flower's current position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            // Move the flower to the exact point the raycast hit the ground
            transform.position = hit.point;
        }
    }
}