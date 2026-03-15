using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Transform slotContainer;

    [Header("Prefabs & Data")]
    [SerializeField] private ShopSlotUI slotPrefab;
    [SerializeField] private List<ItemData> itemsToSell; // ScriptableObjects go here

    // Task source used to await the player leaving the shop
    private UniTaskCompletionSource _shopTaskSource;

    private void Start()
    {
        UpdateCurrencyUI();
        // keep Start lightweight - listener is added in OnEnable so it works when this GameObject is activated later
    }

    private void OnEnable()
    {
        if (leaveButton != null)
        {
            leaveButton.onClick.AddListener(OnLeaveButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (leaveButton != null)
        {
            leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
        }
    }

    private void OnLeaveButtonClicked()
    {
        Debug.Log("[ShopManager] Leave button clicked");
        _shopTaskSource?.TrySetResult();
    }

    public async UniTask StartShopping()
    {
        Debug.Log("[ShopManager] StartShopping called - activating shop UI");

        gameObject.SetActive(true);
        UpdateCurrencyUI();
        PopulateShop();

        _shopTaskSource = new UniTaskCompletionSource();

        Debug.Log("[ShopManager] Waiting for player to leave the shop...");
        await _shopTaskSource.Task;

        gameObject.SetActive(false);

        Debug.Log("[ShopManager] Exiting Shop. Transitioning to Night...");

    }

    // ShopManager.cs
    private void PopulateShop()
    {
        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        foreach (var item in itemsToSell)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            // Pass 'this' so the slot can talk back to the manager
            slot.Setup(item, this);
        }
    }

    public void UpdateCurrencyUI()
    {
        if (currencyText != null && InventoryManager.Instance != null)
            currencyText.text = InventoryManager.Instance.totalStardust.ToString();
    }
}