using UnityEngine;
using UnityEngine.UI;
using Singleton;

public class ShopUI : SingletonPersistent
{
    public static ShopUI Instance => GetInstance<ShopUI>();

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private ShopSlotUI[] slots;
    [SerializeField] private ItemData[] shopItems;   // assign in inspector
    [SerializeField] private Button proceedButton;

    protected override void OnAwake()
    {
        shopPanel.SetActive(false);
        proceedButton.onClick.AddListener(OnProceedClicked);
    }

    public void OpenShop()
    {
        Refresh();
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length && i < shopItems.Length; i++)
            slots[i].Setup(shopItems[i]);
    }

    private void OnProceedClicked()
    {
        GameManager.Instance.ProceedFromShop(); // -> NightState
    }
}