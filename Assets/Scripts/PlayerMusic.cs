using SingletonManagers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMusic : MonoBehaviour
{
    [SerializeField] private NoteSpawner spawner;
    [SerializeField] private SongData song;
    [SerializeField] private ScoreManager scoreManager;

    private int activeFret = 0; // 0=open, 1=fret1, 2=fret2, 3=fret3

    private System.Action lane1Delegate;
    private System.Action lane2Delegate;
    private System.Action lane3Delegate;

    private System.Action<bool> fret1Delegate;
    private System.Action<bool> fret2Delegate;
    private System.Action<bool> fret3Delegate;

    private void OnEnable()
    {
        if (InputHandler.Instance == null) return;

        lane1Delegate = () => HandleLane(0);
        lane2Delegate = () => HandleLane(1);
        lane3Delegate = () => HandleLane(2);

        fret1Delegate = (p) => HandleFret(1, p);
        fret2Delegate = (p) => HandleFret(2, p);
        fret3Delegate = (p) => HandleFret(3, p);

        InputHandler.Instance.OnLane1 += lane1Delegate;
        InputHandler.Instance.OnLane2 += lane2Delegate;
        InputHandler.Instance.OnLane3 += lane3Delegate;

        InputHandler.Instance.OnFret1 += fret1Delegate;
        InputHandler.Instance.OnFret2 += fret2Delegate;
        InputHandler.Instance.OnFret3 += fret3Delegate;
    }

    private void OnDisable()
    {
        if (InputHandler.Instance == null) return;

        InputHandler.Instance.OnLane1 -= lane1Delegate;
        InputHandler.Instance.OnLane2 -= lane2Delegate;
        InputHandler.Instance.OnLane3 -= lane3Delegate;

        InputHandler.Instance.OnFret1 -= fret1Delegate;
        InputHandler.Instance.OnFret2 -= fret2Delegate;
        InputHandler.Instance.OnFret3 -= fret3Delegate;
    }

    private void Update()
    {
        // For placeholder testing: switch action map / load song
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            InputHandler.Instance.SwitchActionMap();
            scoreManager.ActivateBarUI();
            spawner.LoadSong(song);
        }
    }

    private void HandleFret(int fretIndex, bool isPressed)
    {
        activeFret = isPressed ? fretIndex : 0; // 0=open when released
    }

    private void HandleLane(int laneIndex)
    {
        Note closestNote = FindClosestNoteInLane(laneIndex);
        if (closestNote == null)
        {
            Debug.Log("Miss");
            scoreManager.Miss();
            return;
        }

        float currentTime = spawner.MusicTimer.SongTime;
        float diff = Mathf.Abs(closestNote.HitTime - currentTime);

        if (closestNote.NoteFret != activeFret)
        {
            scoreManager.Miss();
            return;
        }

        if (diff < 0.05f)
        {
            Debug.Log("Perfect");
            scoreManager.Perfect();
            ParticleManager.Instance.PlayParticle("NoteHit", closestNote.transform.position, closestNote.gameObject.GetComponent<SpriteRenderer>().color);
            AudioManager.Instance.Play("NoteHit", closestNote.transform.position);
            Destroy(closestNote.gameObject);
        }
        else if (diff < 0.1f)
        {
            Debug.Log("Good");
            scoreManager.Good();
            ParticleManager.Instance.PlayParticle("NoteHit", closestNote.transform.position, closestNote.gameObject.GetComponent<SpriteRenderer>().color);
            AudioManager.Instance.Play("NoteHit", closestNote.transform.position);
            Destroy(closestNote.gameObject);
        }
        else
        {
            scoreManager.Miss();
        }
    }

    private Note FindClosestNoteInLane(int lane)
    {
        Note closest = null;
        float minDiff = float.MaxValue;
        float currentTime = spawner.MusicTimer.SongTime;

        foreach (Transform child in spawner.NotesContainer)
        {
            Note note = child.GetComponent<Note>();
            if (note == null || note.Lane != lane) continue;

            float diff = Mathf.Abs(note.HitTime - currentTime);
            if (diff < minDiff && diff < 0.15f) // 150ms hit window
            {
                minDiff = diff;
                closest = note;
            }
        }

        return closest;
    }
}