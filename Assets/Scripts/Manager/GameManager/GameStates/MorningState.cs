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

        gm.tileGenerator.SetTileSet(TileSetType.Morning);
        gm.tileGenerator.ResetWorld();

        gm.musicTimer.PlayMorningMusic();
    }

    public void Update()
    {
        // transition example
        //if (Input.GetKeyDown(KeyCode.N))
        {
            gm.ChangeState(gm.DayState);
        }//
    }

    public void Exit()
    {
        Debug.Log("Leaving Morning");
    }
}