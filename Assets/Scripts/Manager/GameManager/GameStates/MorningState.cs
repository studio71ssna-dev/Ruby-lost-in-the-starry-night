using UnityEngine;
public class MorningState : IGameState
{
    GameManager gm;

    public MorningState(GameManager manager)
    {
        gm = manager;
    }

    public void Enter()
    {
        Debug.Log("Morning Start");

        gm.tileGenerator.SpawnSingleMorningTile();

        // Immediately start day gameplay
        gm.ChangeState(gm.DayState);
    }

    public void Update() { }

    public void Exit() { }
}