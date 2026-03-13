using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private ItemData _data;

    private ShopManager _manager;

    public void Setup(ItemData item, ShopManager manager) // Add manager to parameters
    {
        _data = item;
        _manager = manager; // Store the reference

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.cost.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);

        RefreshState();
    }

    private void OnBuyClicked()
    {
        if (InventoryManager.Instance.TryPurchase(_data))
        {
            RefreshState();
            // CALL THE MANAGER TO UPDATE THE TEXT
            _manager.UpdateCurrencyUI();
        }
    }

    public void RefreshState()
    {
        // Disable button if already owned
        if (InventoryManager.Instance.HasItem(_data))
        {
            buyButton.interactable = false;
            priceText.text = "Sold";
        }
    }
}