using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GardenInventorySlotUI : MonoBehaviour
{
    public Image flowerIcon;
    public TextMeshProUGUI quantityText;
    public Button selectButton;

    [Header("Optional Polish")]
    public GameObject highlightFrame; // Turn this on/off to show it's currently selected

    private ItemData myFlower;

    public void Setup(ItemData flower, int quantity)
    {
        myFlower = flower;
        flowerIcon.sprite = flower.icon;
        quantityText.text = "x" + quantity;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectClicked);

        UpdateHighlight();
    }

    private void OnSelectClicked()
    {
        // Tell the GardenUI manager that this is the flower we want to plant
        GardenUI.Instance.SetSelectedFlower(myFlower);
    }

    public void UpdateHighlight()
    {
        // If we have a highlight frame, turn it on only if this flower is the selected one
        if (highlightFrame != null)
        {
            highlightFrame.SetActive(GardenUI.Instance.GetSelectedFlower() == myFlower);
        }
    }
}