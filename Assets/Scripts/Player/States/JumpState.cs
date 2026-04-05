public class JumpState : PlayerState
{
    public JumpState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        player.Jump();
        anim.Play("Jump");
    }

    public override void Update()
    {
        // When velocity goes down → start falling
        if (player.GetRB().linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.FallState);
        }
    }
}