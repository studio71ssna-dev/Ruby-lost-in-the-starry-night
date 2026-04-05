using UnityEngine;

public class HouseInteractable : MonoBehaviour
{
    private bool playerInside = false;

    private void OnEnable()
    {
<<<<<<< HEAD
        if (GardenUI.Instance != null)
        {
            GardenUI.Instance.OpenGarden();
        }
        else
        {
            Debug.LogError("GardenUI is missing from the scene!");
            GameManager.Instance.SleepAndNextDay(); // Fallback just in case
        }
=======
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

        GameManager.Instance.SleepAndNextDay();
>>>>>>> parent of 73cf461 (.)
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