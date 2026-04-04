using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator anim;
    private static readonly int StateHash = Animator.StringToHash("State");

    // Define the states as an Enum here for global access
    public enum PlayerAnimState { Idle, Walk, Jump, Pickup, Hurt, Rest, Roll }

    private void Awake() => anim = GetComponent<Animator>();

    public void UpdateAnimation(PlayerAnimState newState)
    {
        // Use an Integer or Trigger in your Animator Controller
        anim.SetInteger(StateHash, (int)newState);
    }

    public void PlayTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
}