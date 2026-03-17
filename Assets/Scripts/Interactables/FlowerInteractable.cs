using SingletonManagers;
using UnityEngine;

public class FlowerInteractable : MonoBehaviour, IInteractable
{
    public FlowerData data;

    public void Interact(PlayerController player)
    {
        if (data == null) return;

        
        InventoryManager.Instance.AddStardust(data.value);
        ParticleManager.Instance.PlayParticle("PickUp", transform.position, data.glowColor);
        AudioManager.Instance.Play("Pickup", transform.position);

        Destroy(gameObject, 0.25f);
    }
}