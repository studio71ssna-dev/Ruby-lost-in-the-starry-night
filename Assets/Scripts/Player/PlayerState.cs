public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;
    protected PlayerAnimationManager anim;

    public PlayerState(PlayerController player, PlayerStateMachine stateMachine, PlayerAnimationManager anim)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.anim = anim;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}