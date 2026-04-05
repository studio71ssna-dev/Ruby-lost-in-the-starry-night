using UnityEngine;

public class HouseInteractable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController player)
    {
        GameManager.Instance.SleepAndNextDay();
    }
}