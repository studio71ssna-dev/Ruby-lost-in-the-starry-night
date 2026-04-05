using UnityEngine;

public class FlowerInteractable : MonoBehaviour, IInteractable
{
    public FlowerData data; // 👈 MUST be assigned in inspector

    public void Interact(PlayerController player)
    {
        player.TriggerPickup(this);
    }
}