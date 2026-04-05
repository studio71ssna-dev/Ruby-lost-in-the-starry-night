using UnityEngine;

public class FlowerInteractable : MonoBehaviour, IInteractable
{
    public FlowerData data;

    public void Interact(PlayerController player)
    {
        player.TriggerPickup(this);
    }
}