using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject gardenGalleryPanel;
    public Transform gardenGridContainer;

    public void StartGame()
    {
        // Replace "GameScene" with the actual name of your level
        SceneManager.LoadScene("GameScene");
        GameManager.Instance.DayStartEvent.Invoke(); // Start the game in the morning state
    }

    public void OpenGardenGallery()
    {
        mainPanel.SetActive(false);
        gardenGalleryPanel.SetActive(true);

        // Use the Singleton to build the 5x5 grid
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.ConstructUI(gardenGridContainer);
        }
    }

    public void CloseGardenGallery()
    {
        gardenGalleryPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}