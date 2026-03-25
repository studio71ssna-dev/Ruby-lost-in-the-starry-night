using UnityEngine;

public class NightState : IGameState
{
    GameManager gm;

    public NightState(GameManager manager) { gm = manager; }

    public void Enter()
    {
        Debug.Log("Night Start");
        gm.tileGenerator.SetTileSet(TileSetType.Night);
    }

    public void Update() { }
    public void Exit() { }
}