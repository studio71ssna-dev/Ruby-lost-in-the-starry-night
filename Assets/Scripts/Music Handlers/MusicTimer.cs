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
            noteSpawner.LoadSong(currentSong);

        // Activate the music UI
        if (MusicUI != null)
            MusicUI.SetActive(true);

        InputHandler.Instance.SwitchActionMap();
    }

    public void SongFinished()
    {
        Debug.Log("Checking if song finished: " + music.clip?.name);
        if (!music.isPlaying && music.clip != null)
        {
            Debug.Log("Song finished: " + currentSong.music.name);
            scoreManager.DeactivateBarUI();
                if (MusicUI != null)
                    MusicUI.SetActive(false);
                songFinished.Invoke();

            
        }
    }
}