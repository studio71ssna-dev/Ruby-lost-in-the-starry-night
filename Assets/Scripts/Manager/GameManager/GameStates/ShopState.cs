using UnityEngine;

public class ShopState : IGameState
{
    GameManager gm;

    public ShopState(GameManager manager) { gm = manager; }

    public void Enter()
    {
        Debug.Log("Shop Open");
        Time.timeScale = 0f;
        gm.tileGenerator.SpawnShopTile();
        // Panel is opened by ShopUI when player interacts with ShopInteractable
        // ShopInteractable.HandleInteract -> GameManager.OpenShop -> this Enter()
        // So open the panel here since we only arrive via interaction
        ShopUI.Instance?.OpenShop();
    }

    public void Update() { }

    public void Exit()
    {
        Time.timeScale = 1f;
        ShopUI.Instance?.CloseShop();
    }

    public void RefreshShopUI() => ShopUI.Instance?.Refresh();
}