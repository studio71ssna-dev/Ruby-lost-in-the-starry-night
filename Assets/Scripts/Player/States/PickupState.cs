using UnityEngine;
using SingletonManagers;

public class PickupState : PlayerState
{
    private FlowerInteractable target;

    public PickupState(PlayerController player, PlayerStateMachine sm, PlayerAnimationManager anim)
        : base(player, sm, anim) { }

    public void SetTarget(FlowerInteractable flower)
    {
        target = flower;
    }

    public override void Enter()
    {
        if (target == null)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        anim.Play("Pickup", true); // LOCK animation

       
        PerformPickup();

    }

    private void PerformPickup()
    {
        if (target == null) return;

        var data = target.data;

        // Add to session
        //player.DayTimeManager.AddFlowerToSession(data);

        // Effects
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.PlayParticle(
                "PickUp",
                target.transform.position,
                data.glowColor
            );
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("Pickup", target.transform.position);
        }

        GameObject.Destroy(target.gameObject);

        target = null;
    }

    public override void Exit()
    {
        anim.Unlock();
    }

    public override void Update()
    {
        // Wait until animation finishes
        if (!IsAnimationPlaying("Pickup"))
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    private bool IsAnimationPlaying(string stateName)
    {
        Animator animator = anim.GetComponent<Animator>();
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName);
    }
}