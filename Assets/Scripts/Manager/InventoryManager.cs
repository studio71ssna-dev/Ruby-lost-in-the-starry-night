using UnityEngine;
using Singleton;

public class InventoryManager : SingletonPersistent
{
    public static InventoryManager Instance => GetInstance<InventoryManager>();

    [Header("Currency")]
    public int stardustFlowers = 0;

    [Header("Upgrades Purchased")]
    public bool hasGildedFlute = false;   // Increases rhythm timing window
    public bool hasSilverStrings = false; // Lowers Chaos Meter faster
    public bool hasLogicLens = false;     // Provides hints during the Safety Quiz

    // Method to add flowers (called by DayTimeManager)
    public void AddFlowers(int amount)
    {
        stardustFlowers += amount;
        Debug.Log($"Inventory Updated: {stardustFlowers} Stardust Flowers total.");
    }

    // Method to handle purchases (called by the Merchant/Shop)
    public bool TryPurchaseItem(string itemName, int cost)
    {
        if (stardustFlowers >= cost)
        {
            stardustFlowers -= cost;

            switch (itemName)
            {
                case "GildedFlute": hasGildedFlute = true; break;
                case "SilverStrings": hasSilverStrings = true; break;
                case "LogicLens": hasLogicLens = true; break;
            }

            Debug.Log($"Purchased {itemName}! Flowers remaining: {stardustFlowers}");
            return true;
        }

        Debug.Log("Not enough Stardust Flowers!");
        return false;
    }
}