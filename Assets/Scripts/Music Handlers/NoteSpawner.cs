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
    [SerializeField] private Transform[] lanes;
    [SerializeField] private Transform[] hitCircles;
    [SerializeField] private Transform notesContainer; // parent for all spawned notes

    private int nextNoteIndex = 0;

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
        Vector3 spawnPos = lanes[note.lane].position;
        Vector3 hitPos = hitCircles[note.lane].position;

        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity, notesContainer);
        obj.GetComponent<Note>().Init(
            note.time,
            note.time - spawnLeadTime,
            spawnPos,
            hitPos,
            musicTimer
        );
    }

    /// <summary>
    /// Loads a new song, clears old notes, and starts music immediately.
    /// </summary>
    public void LoadSong(SongData newSong)
    {
        song = newSong;
        nextNoteIndex = 0;

        // Clear all previously spawned notes
        foreach (Transform child in notesContainer)
            Destroy(child.gameObject);

        // Start music
        musicTimer.StartSong(newSong);
    }
}