using UnityEngine;
using UnityEngine.UI;

public class GardenSlotUI : MonoBehaviour
{
    public Image flowerImage; // The visual representation of the planted flower
    public Button slotButton;

    private ItemData plantedFlower;

    private void Awake()
    {
        slotButton.onClick.AddListener(OnSlotClicked);
        flowerImage.gameObject.SetActive(false); // Hide image until a flower is planted
    }

    private void OnSlotClicked()
    {
        // 1. Check if the slot is empty
        if (plantedFlower != null) return;

        // 2. Get the flower the player currently wants to plant
        ItemData flowerToPlant = GardenUI.Instance.GetSelectedFlower();
        if (flowerToPlant == null) return;

        // 3. Try to remove it from the inventory
        if (InventoryManager.Instance.TryUseFlower(flowerToPlant))
        {
            plantedFlower = flowerToPlant;
            flowerImage.sprite = flowerToPlant.gardenSprite; // Use the top-down garden sprite
            flowerImage.gameObject.SetActive(true);

            // Refresh the UI to update flower counts
            GardenUI.Instance.RefreshInventoryUI();
        }
    }
}