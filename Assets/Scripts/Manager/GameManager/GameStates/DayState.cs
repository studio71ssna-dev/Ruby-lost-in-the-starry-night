using UnityEngine;

public class DayState : IGameState
{
    GameManager gm;

    public DayState(GameManager manager)
    {
        gm = manager;
    }

    public void Enter()
    {
        Debug.Log("Day Start");

        gm.tileGenerator.SetTileSet(TileSetType.Day);
        gm.musicTimer.PlayDayMusic();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            gm.ChangeState(gm.ShopState);
        }
    }

    public void Exit() { }
}