using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        if (ShopUI.Instance == null)
        {
            Debug.LogError("ShopUI not found in scene!");
            return;
        }

        ShopUI.Instance.OpenShop();
    }
}