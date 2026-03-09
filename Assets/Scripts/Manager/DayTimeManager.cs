using UnityEngine;

public class DayTimeManager : MonoBehaviour
{
    public bool isDayTime = true;
    public int collectedFlowers = 0;

    void CollectFlower()
    {
        collectedFlowers++;
        Debug.Log("Flower collected! Total: " + collectedFlowers);
    }

    void Update()
    {
        CheckDayTimeTimer();
    }

    void CheckDayTimeTimer()
    {
        if (!isDayTime)
        {
            // 1. Transfer collected flowers to the persistent Inventory
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddFlowers(collectedFlowers);
            }

            // 2. Change state to Shop
            GameManager.Instance.gamestate = GameManager.GameState.Shop;
        }
    }
}