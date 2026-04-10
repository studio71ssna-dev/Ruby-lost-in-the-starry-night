using UnityEngine;

public class MorningState : IGameState
{
    GameManager gm;

    public MorningState(GameManager manager) { gm = manager; }

    public void Enter()
    {
        Debug.Log("Morning Start - Back to Procedural");

        // 1. Turn OFF the static night environment
        if (gm.staticNightEnvironment != null)
        {
            gm.staticNightEnvironment.SetActive(false);
        }

        // 2. Teleport Ruby back to the start line for the new day
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null && gm.morningSpawnPoint != null)
        {
            player.transform.position = gm.morningSpawnPoint.position;
            player.GetRB().linearVelocity = Vector2.zero;
        }

        // 3. Start Procedural Generation
        gm.tileGenerator.SpawnSingleMorningTile();

        // 4. Change Parallax Background back to Morning
        ParallaxBackground background = Object.FindFirstObjectByType<ParallaxBackground>();
        if (background != null) background.SetMorning();

        // Immediately start day gameplay
        gm.ChangeState(gm.DayState);
    }

    public void Update() { }
    public void Exit() { }
}