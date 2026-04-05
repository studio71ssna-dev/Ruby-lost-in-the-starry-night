using UnityEngine;

public class HurtState : PlayerState
{
    private float timer;

    public HurtState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Hurt", true);
        timer = 0.3f;

        Time.timeScale = 0.9f;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Time.timeScale = 1f;
            anim.Unlock();
            stateMachine.ChangeState(player.IdleState);
        }
    }
}