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

    private double songStartDspTime = -1.0; // DSP start time for precise end detection

    public float SongTime => music.time;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Only check for end if a song was started
        if (songStartDspTime < 0) return;
        SongFinished();
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

        // record precise start time
        songStartDspTime = AudioSettings.dspTime;

        // ScoreManager : Activate the bar UI
        if (scoreManager != null)
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
        if (quizTriggered) return; // Prevent multiple triggers if this method is called more than once
        if (music == null || music.clip == null) return;
        if (songStartDspTime < 0) return; // haven't started

        // Use DSP time for robust end detection (avoid floating rounding and playback quirks)
        double elapsed = AudioSettings.dspTime - songStartDspTime;
        const double epsilon = 0.05; // tolerance in seconds

        if (elapsed + epsilon >= music.clip.length)
        {
            Debug.Log("Song finished (detected by DSP time)!");
            if (noteSpawner == null || noteSpawner.NotesContainer.childCount == 0)
            {
                Debug.Log("All notes cleared (or no spawner), triggering quiz!");
                if (scoreManager != null)
                    scoreManager.DeactivateBarUI();
                if (MusicUI != null)
                    MusicUI.SetActive(false);

                quizTriggered = true; // Set the flag to prevent further triggers
                if (noteSpawner != null) noteSpawner.enabled = false;
                songFinished.Invoke();

                // reset start time so Update stops checking until next PlayRandomSong
                songStartDspTime = -1.0;
            }
        }
    }
}