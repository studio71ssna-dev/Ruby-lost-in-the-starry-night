using UnityEngine;

public class PickupState : PlayerState
{
    private float timer;

    public PickupState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Pickup", true); // LOCK animation
        timer = 0.5f;

        player.PerformPickup(); // actual logic
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