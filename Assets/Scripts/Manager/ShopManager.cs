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

    // DECLARATION: This was the missing line causing your error
    private UniTaskCompletionSource _shopTaskSource;

    private void Start()
    {
        UpdateCurrencyUI();
        // Setup the leave button to trigger the completion of the task
        leaveButton.onClick.AddListener(() => _shopTaskSource?.TrySetResult());
    }

    public async UniTask StartShopping()
    {
        gameObject.SetActive(true);
        UpdateCurrencyUI();
        PopulateShop();

        _shopTaskSource = new UniTaskCompletionSource();

        // The game loop waits here until the leaveButton is pressed
        await _shopTaskSource.Task;

        gameObject.SetActive(false);
        Debug.Log("Exiting Shop. Transitioning to Night...");
        GameManager.Instance.TransitionToNight();
    }

    private void PopulateShop()
    {
        // Clear existing slots to prevent duplicates
        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        // Instantiate a slot for every ScriptableObject in the list
        foreach (var item in itemsToSell)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            slot.Setup(item);
        }
    }

    public void UpdateCurrencyUI()
    {
        if (currencyText != null)
            currencyText.text = InventoryManager.Instance.totalStardust.ToString();
    }
}