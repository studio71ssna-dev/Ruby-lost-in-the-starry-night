using UnityEngine;

public class ShopState : IGameState
{
    GameManager gm;

    public ShopState(GameManager manager) { gm = manager; }

    public void Enter()
    {
        Debug.Log("Shop State Active - Spawning Tile");
        // We only spawn the tile here. Time keeps running so the player can walk to it.
        gm.tileGenerator.SpawnShopTile();
    }

    public void Update() { }

    public void Exit()
    {
        // Time is managed by ShopUI now, but we can ensure it's normal when leaving the state
        Time.timeScale = 1f;
        ShopUI.Instance?.CloseShop();
    }

    public void RefreshShopUI() => ShopUI.Instance?.Refresh();
}