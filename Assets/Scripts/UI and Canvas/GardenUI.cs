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
    [SerializeField] private GardenSlotUI gardenSlotPrefab;
    [SerializeField] private Transform gardenGridContainer;

    private ItemData currentlySelectedFlower;

    private List<GardenInventorySlotUI> activeInventorySlots = new();
    private List<GardenSlotUI> activeGridSlots = new();

    protected override void OnAwake()
    {
        if (sleepButton != null)
            sleepButton.onClick.AddListener(SleepAndClose);
    }

    public void OpenGarden()
    {
        Time.timeScale = 0f;
        currentlySelectedFlower = null;

        gardenPanel.SetActive(true);
        UpdateGridSize();
        RefreshInventoryUI();
    }

    private void UpdateGridSize()
    {
        if (!gardenGridContainer.gameObject.activeSelf)
            gardenGridContainer.gameObject.SetActive(true);

        int currentDay = GameManager.Instance.DayCount;
        int targetSlotCount = 25 + ((currentDay / 3) * 5);

        while (activeGridSlots.Count < targetSlotCount)
        {

            GardenSlotUI newSlot = Instantiate(gardenSlotPrefab, gardenGridContainer);
            activeGridSlots.Add(newSlot);
            ActivateAllChildren();
        }


        while (activeGridSlots.Count > targetSlotCount)
        {
            var slot = activeGridSlots[^1];
            activeGridSlots.RemoveAt(activeGridSlots.Count - 1);
            Destroy(slot.gameObject);
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
            slot.UpdateHighlight();
    }

    public void ActivateAllChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void RefreshInventoryUI()
    {
        foreach (var slot in activeInventorySlots)
            Destroy(slot.gameObject);

        activeInventorySlots.Clear();

        if (InventoryManager.Instance == null) return;

        foreach (var kvp in InventoryManager.Instance.flowerInventory)
        {
            if (kvp.Value > 0)
            {
                var newSlot = Instantiate(inventorySlotPrefab, inventorySlotContainer);
                newSlot.Setup(kvp.Key, kvp.Value);
                activeInventorySlots.Add(newSlot);
            }
        }

        if (currentlySelectedFlower != null &&
            (!InventoryManager.Instance.flowerInventory.ContainsKey(currentlySelectedFlower) ||
             InventoryManager.Instance.flowerInventory[currentlySelectedFlower] <= 0))
        {
            SetSelectedFlower(null);
        }
        else
        {
            SetSelectedFlower(currentlySelectedFlower);
        }
    }
}