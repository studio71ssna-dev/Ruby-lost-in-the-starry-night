using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform notesParent;
    [SerializeField] private RectTransform[] lanes;
    [SerializeField] private RectTransform[] hitCircles;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private SongController songController;

    [Header("Settings")]
    [SerializeField] private float spawnLeadTime = 1f;
    [SerializeField] private float missWindow = 0.15f;

    private List<NoteData> songNotes;
    private int nextIndex;

    private Queue<Note>[] laneQueues = new Queue<Note>[3];

    public System.Action OnMiss;

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
            laneQueues[i] = new Queue<Note>();
    }

    public void LoadSong(SongData song)
    {
        songNotes = song.notes;
        nextIndex = 0;

        foreach (Transform child in notesParent)
            Destroy(child.gameObject);

        foreach (var q in laneQueues)
            q.Clear();
    }

    public void ProcessSpawning(double songTime)
    {
        while (nextIndex < songNotes.Count)
        {
            var note = songNotes[nextIndex];

            if (songTime >= note.time - spawnLeadTime)
            {
                Spawn(note);
                nextIndex++;
            }
            else break;
        }
    }

    private void Spawn(NoteData data)
    {
        Vector3 spawnPos = UIToWorld(lanes[data.lane]);
        Vector3 hitPos = UIToWorld(hitCircles[data.lane]);

        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity, notesParent);
        Note note = obj.GetComponent<Note>();

        note.Init(
            data.time,
            data.time - spawnLeadTime,
            spawnPos,
            hitPos,
            songController,
            data.noteFret,
            data.lane
        );

        laneQueues[data.lane].Enqueue(note);
    }

    public void CheckMisses(double songTime)
    {
        for (int lane = 0; lane < laneQueues.Length; lane++)
        {
            if (laneQueues[lane].Count == 0) continue;

            Note note = laneQueues[lane].Peek();

            if (songTime - note.HitTime > missWindow)
            {
                laneQueues[lane].Dequeue();
                Destroy(note.gameObject);

                OnMiss?.Invoke();
            }
        }
    }

    public Note PeekLane(int lane)
    {
        if (laneQueues[lane].Count == 0) return null;
        return laneQueues[lane].Peek();
    }

    public void RemoveNote(int lane)
    {
        if (laneQueues[lane].Count > 0)
        {
            var note = laneQueues[lane].Dequeue();
            Destroy(note.gameObject);
        }
    }

    public bool AllNotesCleared()
    {
        foreach (var q in laneQueues)
            if (q.Count > 0) return false;

        return nextIndex >= songNotes.Count;
    }

    private Vector3 UIToWorld(RectTransform ui)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(gameCamera, ui.position);
        return gameCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, gameCamera.nearClipPlane + 0.1f)
        );
    }
}