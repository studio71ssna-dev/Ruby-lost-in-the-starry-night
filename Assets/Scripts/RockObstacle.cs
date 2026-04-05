using UnityEngine;

public class RockObstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<PlayerHealth>();

        if (health != null)
        {
            Vector2 hitDir = (collision.transform.position - transform.position).normalized;
            health.TakeDamage(1, hitDir);
        }
    }
}