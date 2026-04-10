using UnityEngine;
using SingletonManagers;

public class PickupState : PlayerState
{
    private FlowerInteractable target;
    private bool hasPickedUp;

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

        hasPickedUp = false;
        anim.Play("Pickup", true); // LOCK animation

        // We DO NOT call PerformPickup() here anymore! 
        // We wait for the Animation Event to call it.
    }

    // Changed to public so the PlayerController can trigger it
    public void PerformPickup()
    {
        if (target == null || hasPickedUp) return;

        hasPickedUp = true; // Prevent accidental double-triggering
        var data = target.data;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddStardust(data.value);
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateFlowerCount(InventoryManager.Instance.totalStardust);
            }
        }

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
  
        if (!IsAnimationPlaying("Pickup") && hasPickedUp)
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