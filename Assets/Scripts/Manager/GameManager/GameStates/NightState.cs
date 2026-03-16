using UnityEngine;

public class NightState : IGameState
{
    GameManager gm;

    public NightState(GameManager manager)
    {
        gm = manager;
    }

    public void Enter()
    {
        Debug.Log("Night Start");

        gm.tileGenerator.SetTileSet(TileSetType.Night);
        gm.musicTimer.PlayNightMusic();
    }

    public void Update()
    {
        // wolf gameplay active automatically
    }

    public void Exit() { }
}