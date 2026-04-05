using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Singleton;
using TMPro; // Make sure to include this for TextMeshPro

public class ShopUI : SingletonPersistent
{
    public static ShopUI Instance => GetInstance<ShopUI>();

    [SerializeField] private GameObject shopPanel;

    [Header("Dynamic Spawning")]
    [SerializeField] private ShopSlotUI slotPrefab;
    [SerializeField] private Transform slotContainer;

    [Header("Shop Data")]
    [SerializeField] private ItemData[] shopItems;
    [SerializeField] private Button proceedButton;

    [Header("Top Right UI")]
    [SerializeField] private TextMeshProUGUI totalStardustText; // Drag your Stardust text here

    [Header("Description Panel")]
    [SerializeField] private TextMeshProUGUI descriptionNameText; // Optional: Item name in the panel
    [SerializeField] private TextMeshProUGUI descriptionBodyText; // Drag your Description text here
    [SerializeField] private Image descriptionIcon; // Optional: Show a larger icon here

    private List<ShopSlotUI> activeSlots = new List<ShopSlotUI>();

    protected override void OnAwake()
    {
        shopPanel.SetActive(false);
        proceedButton.onClick.AddListener(OnProceedClicked);
    }

    public void OpenShop()
    {
        Time.timeScale = 0f;
        Refresh();

        // Clear the description panel when first opening the shop
        ClearDescriptionPanel();

        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        Time.timeScale = 1f;
        shopPanel.SetActive(false);
    }

    public void Refresh()
    {
        // Update the total stardust at the top right
        UpdateStardustUI();

        foreach (var slot in activeSlots)
        {
            Destroy(slot.gameObject);
        }
        activeSlots.Clear();

        foreach (ItemData item in shopItems)
        {
            ShopSlotUI newSlot = Instantiate(slotPrefab, slotContainer);
            newSlot.Setup(item);
            activeSlots.Add(newSlot);
        }
    }

    // Call this from the individual slots when they are clicked/selected
    public void ShowItemDescription(ItemData item)
    {
        if (descriptionNameText != null) descriptionNameText.text = item.itemName;
        if (descriptionBodyText != null) descriptionBodyText.text = item.description;
        if (descriptionIcon != null)
        {
            descriptionIcon.sprite = item.icon;
            descriptionIcon.gameObject.SetActive(true);
        }
    }

    private void ClearDescriptionPanel()
    {
        if (descriptionNameText != null) descriptionNameText.text = "Select an item";
        if (descriptionBodyText != null) descriptionBodyText.text = "";
        if (descriptionIcon != null) descriptionIcon.gameObject.SetActive(false);
    }

    public void UpdateStardustUI()
    {
        if (totalStardustText != null && InventoryManager.Instance != null)
        {
            totalStardustText.text = InventoryManager.Instance.totalStardust.ToString();
        }
    }

    private void OnProceedClicked()
    {
        CloseShop();
        GameManager.Instance.ProceedFromShop();
    }
}