public class JumpState : PlayerState
{
    public JumpState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Jump");
    }

    public override void Update()
    {
        if (player.IsGrounded())
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}