using UnityEngine;
using UnityEngine.Events;

public class SongController : MonoBehaviour
{
    [SerializeField] private MusicPlayer player;
    [SerializeField] private NoteManager noteManager;

    public UnityEvent OnSongStart;
    public UnityEvent OnSongEnd;

    private SongData currentSong;
    private bool isPlaying = false;

    public double SongTime => player.CurrentDSPTime;

    public void StartSong(SongData song)
    {
        InputHandler.Instance.SwitchActionMap("Player_Music");
        currentSong = song;

        noteManager.LoadSong(song);
        player.Play(song.music);

        isPlaying = true;
        OnSongStart?.Invoke();
    }

    private void Update()
    {
        if (!isPlaying) return;

        noteManager.ProcessSpawning(SongTime);
        noteManager.CheckMisses(SongTime);

        if (SongTime >= player.ClipLength)
        {
            if (noteManager.AllNotesCleared())
            {
                isPlaying = false;
                OnSongEnd?.Invoke();
            }
        }
    }

    public void StopSong()
    {
        isPlaying = false;
        player.Stop();
    }
}