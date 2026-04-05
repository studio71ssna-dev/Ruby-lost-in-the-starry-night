using UnityEngine;

public class HouseInteractable : MonoBehaviour
{
    public void Interact(PlayerController player)
    {
        GameManager.Instance.SleepAndNextDay();
    }
}