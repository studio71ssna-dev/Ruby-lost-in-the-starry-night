using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Singleton;

public class GardenUI : SingletonPersistent
{
    public static GardenUI Instance => GetInstance<GardenUI>();

    [SerializeField] private GameObject gardenPanel;
    [SerializeField] private Button sleepButton;

    [Header("Inventory List Spawning")]
    [SerializeField] private GardenInventorySlotUI inventorySlotPrefab;
    [SerializeField] private Transform inventorySlotContainer;

    private ItemData currentlySelectedFlower;
    private List<GardenInventorySlotUI> activeInventorySlots = new List<GardenInventorySlotUI>();

    protected override void OnAwake()
    {
        gardenPanel.SetActive(false);
        sleepButton.onClick.AddListener(SleepAndClose);
    }

    public void OpenGarden()
    {
        Time.timeScale = 0f;
        currentlySelectedFlower = null; // Clear selection when opening
        RefreshInventoryUI();
        gardenPanel.SetActive(true);
    }

    private void SleepAndClose()
    {
        Time.timeScale = 1f;
        gardenPanel.SetActive(false);
        GameManager.Instance.SleepAndNextDay();
    }

    public ItemData GetSelectedFlower() => currentlySelectedFlower;

    public void SetSelectedFlower(ItemData flower)
    {
        currentlySelectedFlower = flower;

        // Update the visual highlights so the player knows what is selected
        foreach (var slot in activeInventorySlots)
        {
            slot.UpdateHighlight();
        }
    }

    public void RefreshInventoryUI()
    {
        // 1. Clear out the old UI slots
        foreach (var slot in activeInventorySlots)
        {
            Destroy(slot.gameObject);
        }
        activeInventorySlots.Clear();

        if (InventoryManager.Instance == null) return;

        // 2. Loop through the player's dictionary of owned flowers
        foreach (var kvp in InventoryManager.Instance.flowerInventory)
        {
            ItemData flower = kvp.Key;
            int quantity = kvp.Value;

            // Only spawn a button if they actually have at least 1 of this flower
            if (quantity > 0)
            {
                GardenInventorySlotUI newSlot = Instantiate(inventorySlotPrefab, inventorySlotContainer);
                newSlot.Setup(flower, quantity);
                activeInventorySlots.Add(newSlot);
            }
        }

        // If the player used their last selected flower, clear the selection
        if (currentlySelectedFlower != null && (!InventoryManager.Instance.flowerInventory.ContainsKey(currentlySelectedFlower) || InventoryManager.Instance.flowerInventory[currentlySelectedFlower] <= 0))
        {
            SetSelectedFlower(null);
        }
        else
        {
            SetSelectedFlower(currentlySelectedFlower); // Re-apply highlights
        }
    }
}