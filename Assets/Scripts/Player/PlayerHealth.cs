using UnityEngine;
using DG.Tweening;
using SingletonManagers;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }

    private PlayerController player;
    private SpriteRenderer sprite;

    [Header("Damage Settings")]
    [SerializeField] private float damageCooldown = 0.5f;
    [SerializeField] private float knockbackForce = 6f;

    [Header("Visual")]
    [SerializeField] private Color flashColor = Color.red;

    private float lastDamageTime;
    private Color originalColor;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();

        CurrentHealth = maxHealth;
        originalColor = sprite.color;
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        // NEW: If Ruby is already knocked out and resting, she cannot be hurt again!
        if (player.StateMachine.CurrentState == player.RestState) return;

        if (Time.time - lastDamageTime < damageCooldown) return;
        lastDamageTime = Time.time;

        CurrentHealth -= amount;

        // 🔥 Knockback
        Rigidbody2D rb = player.GetRB();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);

        // 🔥 DOTween Effects
        PlayHitFlash();
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
        transform.DOPunchPosition(hitDirection * 0.5f, 0.2f, 10, 1);

        // 🔥 Camera Shake
        //CameraShakeCM.Instance?.Shake(2f, 0.2f);

        // 🔥 FX
        ParticleManager.Instance?.PlayParticle("Hit", transform.position);
        AudioManager.Instance?.Play("Hit", transform.position);

        // 🔥 UI
        HealthUI.Instance?.UpdateHealth(CurrentHealth, maxHealth);

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

    private void PlayHitFlash()
    {
        sprite.DOColor(flashColor, 0.05f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => sprite.color = originalColor);
    }

    public void Restore(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        // 🔥 Heal effect
        transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 10, 1);
        sprite.DOColor(Color.green, 0.1f).SetLoops(2, LoopType.Yoyo);

        HealthUI.Instance?.UpdateHealth(CurrentHealth, maxHealth);
    }
}