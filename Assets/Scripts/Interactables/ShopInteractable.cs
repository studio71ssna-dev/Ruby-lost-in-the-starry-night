using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        ShopUI.Instance.OpenShop();
    }
}