using UnityEngine;

public class WolfInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private MusicTimer musicTimer;

    public void Interact(PlayerController player)
    {
        musicTimer.PlayRandomSong();
    }
}