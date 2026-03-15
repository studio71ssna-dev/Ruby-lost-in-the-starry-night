using UnityEngine;
using UnityEngine.Events;
public class MusicTimer : MonoBehaviour
{
    [SerializeField] private SongData[] songs; // all available songs
    public NoteSpawner noteSpawner; // assign in inspector
    [SerializeField] private GameObject MusicUI; // assign in inspector
    [SerializeField] private ScoreManager scoreManager; // assign in inspector
    private AudioSource music;
    private SongData currentSong;
    public UnityEvent songFinished;
    private bool quizTriggered = false; // To prevent multiple triggers

    public float SongTime => music.time;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }

    public void PlayRandomSong()
    {
        if (songs == null || songs.Length == 0)
        {
            Debug.LogWarning("No songs assigned!");
            SongFinished();
            return;
        }

        int index = Random.Range(0, songs.Length);
        currentSong = songs[index];

        // Play the song
        music.clip = currentSong.music;
        music.time = 0f;
        music.Play();

        //ScoreManager : Activate the bar UI
        scoreManager.ActivateBarUI();

        // Tell NoteSpawner to load the same song
        if (noteSpawner != null)
        {
            noteSpawner.enabled = true;
            noteSpawner.LoadSong(currentSong);
        }

        // Activate the music UI
        if (MusicUI != null)
            MusicUI.SetActive(true);
        quizTriggered = false; // Reset the flag for the new song
        InputHandler.Instance.SwitchActionMap();
    }

    public void SongFinished()
    {
        if(quizTriggered) return; // Prevent multiple triggers if this method is called more than once

        if (music.clip != null && SongTime >= music.clip.length)
        {
            if (noteSpawner.NotesContainer.childCount == 0)
            {
                scoreManager.DeactivateBarUI();
                if (MusicUI != null)
                    MusicUI.SetActive(false);
                quizTriggered = true; // Set the flag to prevent further triggers
                noteSpawner.enabled = false;
                songFinished.Invoke();
            }
            
        }
    }
}