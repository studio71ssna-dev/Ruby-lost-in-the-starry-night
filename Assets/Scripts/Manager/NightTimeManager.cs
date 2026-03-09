using UnityEngine;

public class NightTimeManager : MonoBehaviour
{
    public bool isWolfEncounter = false;

    void Update()
    {
        CheckWolfEncounter();
    }

    void CheckWolfEncounter()
    {
        if (isWolfEncounter)
        {
            // Accessing the Dictionary-based Singleton
            GameManager.Instance.gamestate = GameManager.GameState.GameOver;
        }
    }
}