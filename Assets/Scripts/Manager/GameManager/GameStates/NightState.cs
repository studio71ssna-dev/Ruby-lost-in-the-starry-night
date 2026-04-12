using UnityEngine;

public class NightState : IGameState
{
    GameManager gm;

    public NightState(GameManager manager) { gm = manager; }

    public void Enter()
    {
        Debug.Log("Night Start - Teleporting to Static Camp");

        // 1. Erase the procedural world left behind
        gm.tileGenerator.ResetWorld();
        gm.Wolf.SetActive(true);

        // 2. Turn ON the permanent night environment
        if (gm.staticNightEnvironment != null)
        {
            gm.staticNightEnvironment.SetActive(true);
        }

        // 3. Teleport Ruby and kill her momentum
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null && gm.nightSpawnPoint != null)
        {
            player.transform.position = gm.nightSpawnPoint.position;
            player.GetRB().linearVelocity = Vector2.zero; // Stop her from sliding after teleport
        }

        // 4. Change Parallax Background
        ParallaxBackground background = Object.FindFirstObjectByType<ParallaxBackground>();
        if (background != null) background.SetNight();
    }

    public void Update() { }
    public void Exit() { }
}