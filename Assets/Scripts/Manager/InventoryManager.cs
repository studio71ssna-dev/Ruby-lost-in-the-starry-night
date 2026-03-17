using Singleton;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonPersistent
{
    public static InventoryManager Instance => GetInstance<InventoryManager>();

    public int totalStardust { get; private set; }

    private HashSet<ItemData> ownedUpgrades = new HashSet<ItemData>();
    private HashSet<ItemData> dailyPurchasedUpgrades = new HashSet<ItemData>();

    // NEW: Track how many of each flower the player owns
    public Dictionary<ItemData, int> flowerInventory = new Dictionary<ItemData, int>();

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayChanged += ResetDailyPurchases;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayChanged -= ResetDailyPurchases;
    }

    public void AddStardust(int amount) => totalStardust += amount;

    public bool HasBoughtUpgradeToday(ItemData item) => dailyPurchasedUpgrades.Contains(item);

    public bool TryPurchase(ItemData item)
    {
        if (totalStardust < item.cost) return false;
        if (item.itemType == ItemType.Upgrade && HasBoughtUpgradeToday(item)) return false;

        totalStardust -= item.cost;

        if (item.itemType == ItemType.Upgrade)
        {
            dailyPurchasedUpgrades.Add(item);
            ownedUpgrades.Add(item);
        }
        else if (item.itemType == ItemType.Flower)
        {
            // Add or increment the flower count
            if (flowerInventory.ContainsKey(item))
                flowerInventory[item]++;
            else
                flowerInventory[item] = 1;
        }

        return true;
    }

    // NEW: Call this when placing a flower in the garden
    public bool TryUseFlower(ItemData flower)
    {
        if (flowerInventory.ContainsKey(flower) && flowerInventory[flower] > 0)
        {
            flowerInventory[flower]--;
            return true;
        }
        return false;
    }

    private void ResetDailyPurchases(int day) => dailyPurchasedUpgrades.Clear();
}