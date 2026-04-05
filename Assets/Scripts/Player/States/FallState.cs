public class FallState : PlayerState
{
    public FallState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Jump"); // or "Fall" if you have animation
    }

    public override void Update()
    {
        if (player.IsGrounded())
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}