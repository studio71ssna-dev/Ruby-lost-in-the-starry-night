using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }

    private PlayerController player;
    private float lastDamageTime;
    private float damageCooldown = 0.5f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        lastDamageTime = Time.time;

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            player.StateMachine.ChangeState(player.RestState);
        }
        else
        {
            player.StateMachine.ChangeState(player.HurtState);
        }
    }

    public void Restore(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
    }
}