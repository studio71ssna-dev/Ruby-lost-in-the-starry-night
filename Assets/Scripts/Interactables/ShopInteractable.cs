using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        GameManager.Instance.OpenShop();
    }
}