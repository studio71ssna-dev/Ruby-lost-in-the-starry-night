/*using UnityEngine;

public class MusicTimer : MonoBehaviour
{
    [SerializeField] SongData[] songs;
    private AudioSource music;

    public float SongTime => music.time;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }
    public void StartSong(SongData song)
    {

        music.clip = song.music;
        music.time = 0f;
        music.Play();
    }
}*/
using UnityEngine;
public class MusicTimer : MonoBehaviour
{
    [SerializeField] private SongData[] songs; // all available songs
    public NoteSpawner noteSpawner; // assign in inspector
    private AudioSource music;
    private SongData currentSong;

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

        // Tell NoteSpawner to load the same song
        if (noteSpawner != null)
            noteSpawner.LoadSong(currentSong);
    }
}