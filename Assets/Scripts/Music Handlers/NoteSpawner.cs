using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Song Settings")]
    [SerializeField] private SongData song;
    [SerializeField] private MusicTimer musicTimer;
    [SerializeField] private float spawnLeadTime = 1f;

    [Header("References")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private RectTransform[] lanes;
    [SerializeField] private RectTransform[] hitCircles;
    [SerializeField] private Transform notesContainer;
    [SerializeField] private Color[] fretColors; // 0=open, 1=fret1, 2=fret2, 3=fret3

    [SerializeField] private Camera gameCamera;

    private int nextNoteIndex = 0;

    public MusicTimer MusicTimer => musicTimer;
    public Transform NotesContainer => notesContainer;

    void Update()
    {
        if (song == null) return;
        if (nextNoteIndex >= song.notes.Count) return;

        var note = song.notes[nextNoteIndex];

        if (musicTimer.SongTime >= note.time - spawnLeadTime)
        {
            Spawn(note);
            nextNoteIndex++;
        }
    }

    private void Spawn(NoteData note)
    {
        Vector3 spawnPos = UIToWorld(lanes[note.lane]);
        Vector3 hitPos = UIToWorld(hitCircles[note.lane]);

        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity, notesContainer);
        Color fretColor = fretColors[note.noteFret];

        obj.GetComponent<Note>().Init(
            note.time,
            note.time - spawnLeadTime,
            spawnPos,
            hitPos,
            musicTimer,
            fretColor,
            note.noteFret,
            note.lane
        );
    }

    public void LoadSong(SongData newSong)
    {
        song = newSong;
        nextNoteIndex = 0;

        foreach (Transform child in notesContainer)
            Destroy(child.gameObject);

        musicTimer.StartSong(newSong);
    }

    private Vector3 UIToWorld(RectTransform ui)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(gameCamera, ui.position);
        Vector3 worldPos = gameCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, gameCamera.nearClipPlane + 0.1f)
        );
        return worldPos;
    }
}