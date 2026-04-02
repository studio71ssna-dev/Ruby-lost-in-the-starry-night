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

    [Header("Dynamic Grid Spawning")]
    [SerializeField] private GardenSlotUI gardenSlotPrefab; // Drag your GardenSlot prefab here
    [SerializeField] private Transform gardenGridContainer; // Drag the PlantingGrid here

    private ItemData currentlySelectedFlower;
    private List<GardenInventorySlotUI> activeInventorySlots = new List<GardenInventorySlotUI>();

    // NEW: Keep track of the physical dirt patches in the world
    private List<GardenSlotUI> activeGridSlots = new List<GardenSlotUI>();

    protected override void OnAwake()
    {
        sleepButton.onClick.AddListener(SleepAndClose);
    }

    public void OpenGarden()
    {
        Time.timeScale = 0f;
        currentlySelectedFlower = null;

        UpdateGridSize(); // Grow the garden if needed!
        RefreshInventoryUI();

        gardenPanel.SetActive(true);
    }

    public void ActivateAllChildren()
    {
        // Loop through every direct child of the GameObject this script is attached to
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    private void UpdateGridSize()
    {
        int currentDay = GameManager.Instance.DayCount;
        int targetSlotCount = 25 + ((currentDay / 3) * 5);

        while (activeGridSlots.Count < targetSlotCount)
        {
            GardenSlotUI newSlot = Instantiate(gardenSlotPrefab, gardenGridContainer);

           ActivateAllChildren(); // Ensure the new slot and all its children are active
            newSlot.gameObject.SetActive(true);

            activeGridSlots.Add(newSlot);
        }
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
        foreach (var slot in activeInventorySlots)
        {
            slot.UpdateHighlight();
        }
    }

    public void RefreshInventoryUI()
    {
        foreach (var slot in activeInventorySlots) Destroy(slot.gameObject);
        activeInventorySlots.Clear();

        if (InventoryManager.Instance == null) return;

        foreach (var kvp in InventoryManager.Instance.flowerInventory)
        {
            if (kvp.Value > 0)
            {
                GardenInventorySlotUI newSlot = Instantiate(inventorySlotPrefab, inventorySlotContainer);
                newSlot.Setup(kvp.Key, kvp.Value);
                activeInventorySlots.Add(newSlot);
            }
        }

        if (currentlySelectedFlower != null && (!InventoryManager.Instance.flowerInventory.ContainsKey(currentlySelectedFlower) || InventoryManager.Instance.flowerInventory[currentlySelectedFlower] <= 0))
        {
            SetSelectedFlower(null);
        }
        else
        {
            SetSelectedFlower(currentlySelectedFlower);
        }
    }
}