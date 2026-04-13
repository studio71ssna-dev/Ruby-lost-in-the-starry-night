using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;

    public void StartGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance.DayStartEvent.Invoke();
       GameManager.Instance.StartMorning();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}