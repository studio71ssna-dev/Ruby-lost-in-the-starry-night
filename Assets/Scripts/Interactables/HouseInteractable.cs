using UnityEngine;

public class HouseInteractable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        if (GardenUI.Instance != null)
        {
            GardenUI.Instance.OpenGarden();
        }
        else
        {
            Debug.LogError("GardenUI is missing from the scene!");
            GameManager.Instance.SleepAndNextDay(); // Fallback just in case
        }
    }
}