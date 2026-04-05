using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GardenInventorySlotUI : MonoBehaviour
{
    public Image flowerIcon;
    public TextMeshProUGUI quantityText;
    public Button selectButton;

    [Header("Optional Polish")]
    public GameObject highlightFrame;

    private ItemData myFlower;

    public void Setup(ItemData flower, int quantity)
    {
        myFlower = flower;

        if (flowerIcon != null)
            flowerIcon.sprite = flower.icon;

        if (quantityText != null)
            quantityText.text = "x" + quantity;

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
        }

        UpdateHighlight();
    }

    private void OnSelectClicked()
    {
        GardenUI.Instance.SetSelectedFlower(myFlower);
    }

    public void UpdateHighlight()
    {
        if (highlightFrame != null)
        {
            highlightFrame.SetActive(
                GardenUI.Instance.GetSelectedFlower() == myFlower
            );
        }
    }
}