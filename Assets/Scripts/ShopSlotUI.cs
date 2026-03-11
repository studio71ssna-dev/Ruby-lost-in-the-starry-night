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

    public void Setup(ItemData item)
    {
        _data = item;
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
            // Optional: Trigger a UI refresh on the ShopManager
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