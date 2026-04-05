using UnityEngine;

public class WolfInteractable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        player.musicTimer.PlayRandomSong();
    }
}