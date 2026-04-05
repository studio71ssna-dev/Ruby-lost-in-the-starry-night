using UnityEngine;

public class ShopInteractable : MonoBehaviour
{
    public void Interact(PlayerController player)
    {
        ShopUI.Instance.OpenShop();
    }
}