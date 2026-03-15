using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public UnityEvent ToMainMenu;
    public UnityEvent OnPauseEvent;
    public UnityEvent OnResumeEvent;
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
        Time.timeScale = 1f;
        isPaused = false;
        
        OnResumeEvent.Invoke();

        // Optional: Re-enable player controls or lock cursor here
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;

        OnPauseEvent.Invoke();
        // Make cursor visible to interact with buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToMainMenu()
    {
        ToMainMenu.Invoke();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}