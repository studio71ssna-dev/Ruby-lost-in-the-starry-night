using UnityEngine;

public class RockObstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(1);
        }
    }
}