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
        // Check if already owned
        bool alreadyOwned = itemName switch
        {
            "GildedFlute" => hasGildedFlute,
            "SilverStrings" => hasSilverStrings,
            "LogicLens" => hasLogicLens,
            _ => false
        };

        if (alreadyOwned)
        {
            Debug.Log("You already own this item!");
            return false;
        }

        if (stardustFlowers >= cost)
        {
            stardustFlowers -= cost;
            // ... (rest of your switch logic)
            return true;
        }
        return false;
    }


}