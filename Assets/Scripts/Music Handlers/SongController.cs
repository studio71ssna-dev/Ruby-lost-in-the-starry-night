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
        currentSong = song;

        // Load notes and start audio BEFORE invoking OnSongStart
        // so any listeners (like ActivateBarUI) fire after everything is ready
        noteManager.LoadSong(song);
        player.Play(song.music);

        InputHandler.Instance.SwitchActionMap("Player_Music");

        isPlaying = true;

        // OnSongStart should ONLY have: WolfPressureManager.ActivateBarUI
        // Do NOT wire AudioClip-based calls here
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
        InputHandler.Instance.SwitchActionMap("Player_Movement");
    }

    // Call this from WolfPressureManager.WolfPressureUp event
    // AND from OnSongEnd to clean up
    public void EndEncounter()
    {
        StopSong();
    }
}