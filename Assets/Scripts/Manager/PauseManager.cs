using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;

    private void OnEnable()
    {
        // Subscribe to the InputHandler event
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnPause += TogglePause;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (InputHandler.Instance != null)
            InputHandler.Instance.OnPause -= TogglePause;
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Optional: Re-enable player controls or lock cursor here
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Make cursor visible to interact with buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}