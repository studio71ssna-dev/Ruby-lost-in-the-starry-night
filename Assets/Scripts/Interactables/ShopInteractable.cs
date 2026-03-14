using UnityEngine;

public class ShopInteractable : MonoBehaviour
{
    private bool playerInside = false;

    private void OnEnable()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnInteract -= HandleInteract;
    }

    private void HandleInteract()
    {
        if (!playerInside) return;

        GameManager.Instance.OpenShop();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            playerInside = false;
    }
}