using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Button leaveButton;

    // To tell the GameManager when we are done shopping
    private UniTaskCompletionSource _shopTaskSource;

    private void Start()
    {
        UpdateUI();
        leaveButton.onClick.AddListener(() => _shopTaskSource?.TrySetResult());
    }

    public async UniTask StartShopping()
    {
        gameObject.SetActive(true); // Show Shop UI
        _shopTaskSource = new UniTaskCompletionSource();

        // The code execution stops here until leaveButton is pressed
        await _shopTaskSource.Task;

        gameObject.SetActive(false);
        Debug.Log("Exiting Shop. Transitioning to Night...");
        GameManager.Instance.TransitionToNight();
    }

    public void BuyGildedFlute() => BuyItem("GildedFlute");
    public void BuySilverStrings() => BuyItem("SilverStrings");
    public void BuyLogicLens() => BuyItem("LogicLens");


    public void BuyItem(string itemName)
    {
        int price = 0;

        // Set prices based on GDD
        switch (itemName)
        {
            case "GildedFlute": price = 12; break;
            case "SilverStrings": price = 8; break;
            case "LogicLens": price = 5; break;
        }

        // Call your InventoryManager
        bool success = InventoryManager.Instance.TryPurchaseItem(itemName, price);

        if (success)
        {
            UpdateUI();
            //PlayPurchaseSound(); // Optional: Add some juice!
        }
        else
        {
            Debug.Log("Not enough flowers or already owned!");
        }
    }

    private void UpdateUI()
    {
        currencyText.text = $"Flowers: {InventoryManager.Instance.stardustFlowers}";
    }
}