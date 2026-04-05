using UnityEngine;

public class RestState : PlayerState
{
    private float regenTimer;
    private PlayerHealth health;

    public RestState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Rest", true);
        regenTimer = 0f;

        health = player.GetComponent<PlayerHealth>(); // ✅ FIX
    }

    public override void Update()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= 1f)
        {
            regenTimer = 0f;
            health.Restore(1); // ✅ FIX
        }

        if (health.CurrentHealth >= health.maxHealth) // ✅ FIX
        {
            anim.Unlock();
            stateMachine.ChangeState(player.IdleState);
        }
    }
}