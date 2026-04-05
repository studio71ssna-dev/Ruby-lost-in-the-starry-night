using UnityEngine;
public class IdleState : PlayerState
{
    public IdleState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public override void Enter()
    {
        anim.Play("Idle");
    }

    public override void Update()
    {
        if (!player.IsGrounded())
        {
            stateMachine.ChangeState(player.JumpState);
            return;
        }

        if (Mathf.Abs(player.GetMoveInput()) > 0.1f)
        {
            stateMachine.ChangeState(player.WalkState);
        }
    }
}