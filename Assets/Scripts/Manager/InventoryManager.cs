using Singleton;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonPersistent
{
    public static InventoryManager Instance => GetInstance<InventoryManager>();

    public int totalStardust { get; private set; }

    private HashSet<ItemData> ownedItems = new HashSet<ItemData>();

    public void AddStardust(int amount)
    {
        totalStardust += amount;
    }

    public bool HasItem(ItemData item) => ownedItems.Contains(item);

    public bool TryPurchase(ItemData item)
    {
        if (HasItem(item)) return false;
        if (totalStardust < item.cost) return false;

        totalStardust -= item.cost;
        ownedItems.Add(item);
        return true;
    }
}