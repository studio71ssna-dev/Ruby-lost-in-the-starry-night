using UnityEngine;

public class HurtState : PlayerState
{
    private float timer;

    public HurtState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Hurt", true); // lock animation
        timer = 0.4f;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            anim.Unlock();
            stateMachine.ChangeState(player.IdleState);
        }
    }
}