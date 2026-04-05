using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        InputHandler.Instance.OnInteract += HandleInteract;
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnInteract -= HandleInteract;
    }

    private void HandleInteract()
    {
        if (currentInteractable == null) return;

        if (player == null)
        {
            Debug.LogError("Player is NULL in PlayerInteraction");
            return;
        }

        currentInteractable.Interact(player);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var interactable = col.GetComponent<IInteractable>();
        if (interactable != null)
            currentInteractable = interactable;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        var interactable = col.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
            currentInteractable = null;
    }
}