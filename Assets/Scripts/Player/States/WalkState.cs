using UnityEngine;
public class WalkState : PlayerState
{
    public WalkState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Walk");
    }

    public override void Update()
    {
        if (!player.IsGrounded())
        {
            stateMachine.ChangeState(player.JumpState);
            return;
        }

        if (Mathf.Abs(player.GetMoveInput()) < 0.1f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}