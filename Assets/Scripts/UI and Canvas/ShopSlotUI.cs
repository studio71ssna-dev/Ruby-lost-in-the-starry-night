using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    [Header("Buttons")]
    public Button buyButton;
    public Button selectButton; // The background button of the slot

    private ItemData _data;

    public void Setup(ItemData item)
    {
        _data = item;

        icon.sprite = item.icon;
        nameText.text = item.itemName;

        // Hook up the Buy Button
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);

        // Hook up the Select Button (Clicking anywhere else on the slot)
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
        }

        RefreshState();
    }

    private void OnSelectClicked()
    {
        // Send this item's data to the main ShopUI description panel
        ShopUI.Instance.ShowItemDescription(_data);
    }

    private void OnBuyClicked()
    {
        if (InventoryManager.Instance.TryPurchase(_data))
        {
            RefreshState();

            // Refreshes the Stardust counter and all other slots
            ShopUI.Instance.Refresh();
        }
    }

    public void RefreshState()
    {
        if (_data == null) return;

        bool isSoldOut = _data.itemType == ItemType.Upgrade && InventoryManager.Instance.HasBoughtUpgradeToday(_data);
        bool canAfford = InventoryManager.Instance.totalStardust >= _data.cost;

        if (isSoldOut)
        {
            buyButton.interactable = false;
            priceText.text = "Sold Out";
            priceText.color = Color.gray;
        }
        else
        {
            buyButton.interactable = canAfford;
            priceText.text = _data.cost.ToString();
            priceText.color = canAfford ? Color.darkOliveGreen : Color.red;
        }
    }
}