using UnityEngine;

public class ShopState : IGameState
{
    GameManager gm;

    public ShopState(GameManager manager)
    {
        gm = manager;
    }

    public void Enter()
    {
        Debug.Log("Shop Open");

        gm.tileGenerator.SpawnShopTile();
        Time.timeScale = 0f;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            gm.ChangeState(gm.NightState);
        }
    }

    public void RefreshShopUI()
    {
        // temporary stub
    }

    public void Exit()
    {
        Time.timeScale = 1f;
    }
}