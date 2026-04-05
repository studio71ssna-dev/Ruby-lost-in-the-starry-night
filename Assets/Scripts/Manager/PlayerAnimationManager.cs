using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private float crossFadeDuration = 0.15f;

    private bool isLocked;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Play(string stateName, bool lockState = false)
    {
        if (isLocked) return;

        anim.CrossFade(stateName, crossFadeDuration);

        if (lockState)
            isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }
}