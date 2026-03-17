using Singleton;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonPersistent
{
    public static InventoryManager Instance => GetInstance<InventoryManager>();

    public int totalStardust { get; private set; }

    private HashSet<ItemData> ownedItems = new HashSet<ItemData>();
    private HashSet<ItemData> dailyPurchasedUpgrades = new HashSet<ItemData>(); // Track daily buys

    private void Start()
    {
        // Listen for the day changing to reset our daily shop limits
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDayChanged += ResetDailyPurchases;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDayChanged -= ResetDailyPurchases;
        }
    }

    public void AddStardust(int amount)
    {
        totalStardust += amount;
    }

    public bool HasBoughtUpgradeToday(ItemData item) => dailyPurchasedUpgrades.Contains(item);

    public bool TryPurchase(ItemData item)
    {
        // 1. Check if they have enough money
        if (totalStardust < item.cost) return false;

        // 2. Check if it's an upgrade that was already bought today
        if (item.itemType == ItemType.Upgrade && HasBoughtUpgradeToday(item)) return false;

        // Process Purchase
        totalStardust -= item.cost;

        if (item.itemType == ItemType.Upgrade)
        {
            dailyPurchasedUpgrades.Add(item);
            ownedItems.Add(item); // Keep permanent track of the upgrade
        }
        else if (item.itemType == ItemType.Flower)
        {
            // You can add logic here to track how many flowers the player has in their inventory
        }

        return true;
    }

    private void ResetDailyPurchases(int day)
    {
        dailyPurchasedUpgrades.Clear();
    }
}