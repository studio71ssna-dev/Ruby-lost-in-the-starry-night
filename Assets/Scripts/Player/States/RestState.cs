using UnityEngine;

public class RestState : PlayerState
{
    private float regenTimer;

    public RestState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Rest", true); // LOCKED
        regenTimer = 0f;
    }

    public override void Update()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= 1f) // regen every 1 sec
        {
            regenTimer = 0f;
            player.RestoreHealth(1);
        }

        if (player.CurrentHealth >= player.maxHealth)
        {
            anim.Unlock();
            stateMachine.ChangeState(player.IdleState);
        }
    }
}